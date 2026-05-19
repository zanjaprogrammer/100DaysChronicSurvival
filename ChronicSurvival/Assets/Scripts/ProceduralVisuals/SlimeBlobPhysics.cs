using UnityEngine;

namespace ChronicSurvival.ProceduralVisuals
{
    /// <summary>
    /// Adds slime-like physics to blob for dynamic, organic movement
    /// Simulates jiggle, squash, and stretch effects
    /// </summary>
    [RequireComponent(typeof(BlobGenerator))]
    public class SlimeBlobPhysics : MonoBehaviour
    {
        [Header("Jiggle Settings")]
        [SerializeField] private float jiggleAmount = 0.1f;
        [SerializeField] private float jiggleSpeed = 5f;
        [SerializeField] private float jiggleDamping = 0.8f;

        [Header("Squash & Stretch")]
        [SerializeField] private bool enableSquashStretch = true;
        [SerializeField] private float squashStretchAmount = 0.2f;
        [SerializeField] private float squashStretchSpeed = 10f;

        [Header("Movement Response")]
        [SerializeField] private float movementInfluence = 1f;
        [SerializeField] private float rotationInfluence = 0.5f;

        private Vector3 lastPosition;
        private Vector3 velocity;
        private Vector3 jiggleVelocity;
        private Vector3 jiggleOffset;
        private float squashStretchValue;
        private BlobGenerator blobGenerator;

        private void Start()
        {
            blobGenerator = GetComponent<BlobGenerator>();
            lastPosition = transform.position;
        }

        private void Update()
        {
            CalculateVelocity();
            ApplyJiggle();
            ApplySquashStretch();
        }

        private void CalculateVelocity()
        {
            // Calculate movement velocity
            velocity = (transform.position - lastPosition) / Time.deltaTime;
            lastPosition = transform.position;
        }

        private void ApplyJiggle()
        {
            // Spring physics for jiggle
            Vector3 targetJiggle = velocity * jiggleAmount * movementInfluence;
            
            // Spring force
            Vector3 springForce = (targetJiggle - jiggleOffset) * jiggleSpeed;
            jiggleVelocity += springForce * Time.deltaTime;
            
            // Damping
            jiggleVelocity *= jiggleDamping;
            
            // Update offset
            jiggleOffset += jiggleVelocity * Time.deltaTime;
            
            // Apply jiggle to scale (subtle)
            float jiggleMagnitude = jiggleOffset.magnitude;
            Vector3 jiggleScale = Vector3.one + Vector3.one * jiggleMagnitude * 0.1f;
            
            // Apply to transform (subtle wobble)
            transform.localScale = Vector3.Lerp(transform.localScale, jiggleScale, Time.deltaTime * 10f);
        }

        private void ApplySquashStretch()
        {
            if (!enableSquashStretch) return;

            // Calculate squash/stretch based on velocity
            float velocityMagnitude = velocity.magnitude;
            float targetSquashStretch = velocityMagnitude * squashStretchAmount;
            
            // Smooth transition
            squashStretchValue = Mathf.Lerp(squashStretchValue, targetSquashStretch, Time.deltaTime * squashStretchSpeed);
            
            // Apply squash & stretch
            if (velocityMagnitude > 0.01f)
            {
                // Direction of movement
                Vector3 moveDir = velocity.normalized;
                
                // Stretch in direction of movement, squash perpendicular
                Vector3 stretchScale = Vector3.one;
                stretchScale += moveDir * squashStretchValue; // Stretch
                stretchScale += Vector3.Cross(moveDir, Vector3.forward) * -squashStretchValue * 0.5f; // Squash
                
                transform.localScale = Vector3.Lerp(transform.localScale, stretchScale, Time.deltaTime * 5f);
            }
            else
            {
                // Return to normal scale
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * 5f);
            }
        }

        // Public methods
        public void AddImpulse(Vector2 impulse)
        {
            jiggleVelocity += (Vector3)impulse * jiggleAmount;
        }

        public void SetJiggleAmount(float amount)
        {
            jiggleAmount = amount;
        }

        public void SetSquashStretchAmount(float amount)
        {
            squashStretchAmount = amount;
        }
    }
}
