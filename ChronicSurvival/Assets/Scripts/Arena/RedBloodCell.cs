using UnityEngine;

namespace ChronicSurvival.Arena
{
    public class RedBloodCell : MonoBehaviour
    {
        private Rigidbody2D body;
        private float flowSpeed;
        private float verticalDrift;
        private float exitX;
        private Camera targetCamera;
        private float laneOffset;
        private float flowPhase;
        private float laneReturnStrength;

        public void Configure(float speed, float drift, float destroyAfterX, Camera cameraRef, float laneCenter)
        {
            body = GetComponent<Rigidbody2D>();
            flowSpeed = speed;
            verticalDrift = drift;
            exitX = destroyAfterX;
            targetCamera = cameraRef;
            laneOffset = laneCenter;
            flowPhase = Random.Range(0f, Mathf.PI * 2f);
            laneReturnStrength = Random.Range(0.05f, 0.11f);
        }

        private void Awake()
        {
            body = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (body == null) return;

            float waveY = laneOffset + Mathf.Sin(Time.time * 1.8f + body.position.x * 0.35f + flowPhase) * verticalDrift;
            float laneForce = (waveY - body.position.y) * laneReturnStrength;
            float currentForwardSpeed = Mathf.Max(body.linearVelocity.x, 0f);
            float forwardPush = Mathf.Max(flowSpeed - currentForwardSpeed, 0f) * body.mass;
            Vector2 flowForce = new Vector2(forwardPush, laneForce * body.mass);
            body.AddForce(flowForce, ForceMode2D.Force);

            Vector2 velocity = body.linearVelocity;
            if (velocity.x < 0.6f)
            {
                velocity.x = 0.6f;
            }
            body.linearVelocity = velocity;

            if (ArenaWalkableMask.Instance == null) return;

            Vector2 currentPosition = body.position;
            Vector2 desiredPosition = currentPosition + body.linearVelocity * Time.fixedDeltaTime;
            Vector2 constrainedPosition = ArenaWalkableMask.Instance.ConstrainMovement(currentPosition, desiredPosition);

            if ((constrainedPosition - desiredPosition).sqrMagnitude <= 0.0001f) return;

            body.position = constrainedPosition;
            body.linearVelocity = new Vector2(
                Mathf.Max(body.linearVelocity.x, flowSpeed * 0.82f),
                body.linearVelocity.y * 0.72f
            );
        }

        private void Update()
        {
            if (transform.position.x <= exitX) return;
            if (targetCamera != null && IsVisibleToCamera()) return;

            Destroy(gameObject);
        }

        private bool IsVisibleToCamera()
        {
            Vector3 viewport = targetCamera.WorldToViewportPoint(transform.position);
            return viewport.z > 0f && viewport.x > -0.08f && viewport.x < 1.08f && viewport.y > -0.08f && viewport.y < 1.08f;
        }
    }
}
