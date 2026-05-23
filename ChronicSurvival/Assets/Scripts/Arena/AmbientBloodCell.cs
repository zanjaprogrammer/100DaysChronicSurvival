using UnityEngine;

namespace ChronicSurvival.Arena
{
    public class AmbientBloodCell : MonoBehaviour
    {
        private float speed = 0.25f;
        private float driftRadius = 3f;
        private Vector2 target;

        public void Configure(float moveSpeed, float radius)
        {
            speed = moveSpeed;
            driftRadius = radius;
            PickTarget();
        }

        private void Update()
        {
            if (Vector2.Distance(transform.position, target) < 0.2f)
            {
                PickTarget();
            }

            Vector2 next = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
            if (ArenaWalkableMask.Instance != null)
            {
                next = ArenaWalkableMask.Instance.ConstrainMovement(transform.position, next);
            }
            transform.position = new Vector3(next.x, next.y, transform.position.z);
        }

        private void PickTarget()
        {
            Vector2 origin = transform.position;
            Vector2 candidate = origin + Random.insideUnitCircle * driftRadius;
            if (ArenaWalkableMask.Instance != null)
            {
                candidate = ArenaWalkableMask.Instance.GetNearestWalkablePosition(candidate, driftRadius + 6f);
            }
            target = candidate;
        }
    }
}
