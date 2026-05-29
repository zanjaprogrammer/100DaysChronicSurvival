using UnityEngine;
using ChronicSurvival.Battle;

namespace ChronicSurvival.Core
{
    public class CameraController : MonoBehaviour
    {
        [Header("Follow")]
        [SerializeField] private bool followLeader = true;
        [SerializeField] private bool autoFindLeader = true;
        [SerializeField] private Vector3 followOffset = new Vector3(0f, 0f, -10f);

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 7f;
        [SerializeField] private float edgeScrollSpeed = 10f;
        [SerializeField] private float edgeScrollBorder = 50f;
        [SerializeField] private bool enableEdgeScroll = true;
        [SerializeField] private bool enableKeyboardMove = true;
        [SerializeField] private bool enableDragPan = true;
        [SerializeField] private int dragMouseButton = 2;

        [Header("Zoom")]
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float minZoom = 3f;
        [SerializeField] private float maxZoom = 10f;
        [SerializeField] private bool enableZoom = true;

        [Header("Bounds")]
        [SerializeField] private bool useBounds = true;
        [SerializeField] private Vector2 minBounds = new Vector2(-10, -10);
        [SerializeField] private Vector2 maxBounds = new Vector2(10, 10);

        [Header("Smoothing")]
        [SerializeField] private bool useSmoothing = true;
        [SerializeField] private float smoothSpeed = 5f;

        private Camera cam;
        private Vector3 targetPosition;
        private float targetZoom;
        private Vector3 lastDragWorld;
        private Transform followTarget;

        private void Start()
        {
            cam = GetComponent<Camera>();
            targetPosition = transform.position;
            targetZoom = cam.orthographicSize;
        }

        private void Update()
        {
            ResolveFollowTarget();
            if (followLeader && followTarget != null)
            {
                targetPosition = followTarget.position + followOffset;
                ClampTargetPosition();
            }
            else
            {
                HandleMovement();
                HandleDragPan();
            }
            HandleZoom();
            ApplyMovement();
        }

        private void ResolveFollowTarget()
        {
            if (!followLeader || !autoFindLeader || followTarget != null)
            {
                return;
            }

            ImmuneSquadController squad = FindFirstObjectByType<ImmuneSquadController>(FindObjectsInactive.Include);
            if (squad == null || squad.Leader == null || squad.Leader.IsDead)
            {
                return;
            }

            followTarget = squad.Leader.transform;
        }

        private void HandleMovement()
        {
            Vector3 moveDirection = Vector3.zero;

            if (enableKeyboardMove)
            {
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                    moveDirection.y += 1;
                if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                    moveDirection.y -= 1;
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                    moveDirection.x -= 1;
                if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                    moveDirection.x += 1;
            }

            if (enableEdgeScroll)
            {
                Vector3 mousePos = Input.mousePosition;

                if (mousePos.x < edgeScrollBorder)
                    moveDirection.x -= 1;
                if (mousePos.x > Screen.width - edgeScrollBorder)
                    moveDirection.x += 1;
                if (mousePos.y < edgeScrollBorder)
                    moveDirection.y -= 1;
                if (mousePos.y > Screen.height - edgeScrollBorder)
                    moveDirection.y += 1;
            }

            if (moveDirection != Vector3.zero)
            {
                moveDirection.Normalize();
                targetPosition += moveDirection * moveSpeed * Time.deltaTime;

                ClampTargetPosition();
            }
        }

        private void HandleDragPan()
        {
            if (!enableDragPan || cam == null) return;

            if (Input.GetMouseButtonDown(dragMouseButton))
            {
                lastDragWorld = cam.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButton(dragMouseButton))
            {
                Vector3 currentDragWorld = cam.ScreenToWorldPoint(Input.mousePosition);
                Vector3 delta = lastDragWorld - currentDragWorld;
                delta.z = 0f;
                targetPosition += delta;
                ClampTargetPosition();
            }
        }

        private void HandleZoom()
        {
            if (!enableZoom) return;

            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll != 0)
            {
                targetZoom -= scroll * zoomSpeed;
                targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            }
        }

        private void ApplyMovement()
        {
            if (useSmoothing)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
                cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, smoothSpeed * Time.deltaTime);
            }
            else
            {
                transform.position = targetPosition;
                cam.orthographicSize = targetZoom;
            }

            Vector3 pos = transform.position;
            pos.z = -10f;
            transform.position = pos;
        }

        private void ClampTargetPosition()
        {
            if (!useBounds) return;

            float halfHeight = targetZoom;
            float halfWidth = targetZoom * (cam != null ? cam.aspect : 16f / 9f);
            float minX = minBounds.x + halfWidth;
            float maxX = maxBounds.x - halfWidth;
            float minY = minBounds.y + halfHeight;
            float maxY = maxBounds.y - halfHeight;

            targetPosition.x = minX <= maxX ? Mathf.Clamp(targetPosition.x, minX, maxX) : (minBounds.x + maxBounds.x) * 0.5f;
            targetPosition.y = minY <= maxY ? Mathf.Clamp(targetPosition.y, minY, maxY) : (minBounds.y + maxBounds.y) * 0.5f;
        }

        public void SetPosition(Vector3 position)
        {
            targetPosition = position;
            targetPosition.z = -10f;
            ClampTargetPosition();
        }

        public void SetZoom(float zoom)
        {
            targetZoom = Mathf.Clamp(zoom, minZoom, maxZoom);
            ClampTargetPosition();
        }

        public void SetZoomLimits(float min, float max)
        {
            minZoom = min;
            maxZoom = Mathf.Max(minZoom, max);
            targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
            ClampTargetPosition();
        }

        public void SetBounds(Bounds bounds, float padding = 0f)
        {
            useBounds = true;
            minBounds = new Vector2(bounds.min.x + padding, bounds.min.y + padding);
            maxBounds = new Vector2(bounds.max.x - padding, bounds.max.y - padding);
            ClampTargetPosition();
        }

        public void SetFollowTarget(Transform target, bool enableFollow = true)
        {
            followTarget = target;
            followLeader = enableFollow;
        }
    }
}
