using UnityEngine;

namespace ChronicSurvival.Core
{
    public class CameraController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float edgeScrollSpeed = 10f;
        [SerializeField] private float edgeScrollBorder = 50f;
        [SerializeField] private bool enableEdgeScroll = true;
        [SerializeField] private bool enableKeyboardMove = true;

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

        private void Start()
        {
            cam = GetComponent<Camera>();
            targetPosition = transform.position;
            targetZoom = cam.orthographicSize;
        }

        private void Update()
        {
            HandleMovement();
            HandleZoom();
            ApplyMovement();
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

                if (useBounds)
                {
                    targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
                    targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);
                }
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

        public void SetPosition(Vector3 position)
        {
            targetPosition = position;
            targetPosition.z = -10f;
        }

        public void SetZoom(float zoom)
        {
            targetZoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        }
    }
}
