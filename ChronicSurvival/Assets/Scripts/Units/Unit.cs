using UnityEngine;
using System.Collections.Generic;
using ChronicSurvival.ProceduralVisuals;
using ChronicSurvival.Core;
using ChronicSurvival.Arena;

namespace ChronicSurvival.Units
{
    [RequireComponent(typeof(BlobGenerator))]
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class Unit : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] protected float maxHealth = 100f;
        [SerializeField] protected float damage = 10f;
        [SerializeField] protected float attackSpeed = 1f;
        [SerializeField] protected float moveSpeed = 2f;
        [SerializeField] protected float detectionRange = 5f;
        [SerializeField] protected float attackRange = 1f;

        [Header("Team")]
        [SerializeField] protected UnitTeam team;

        [Header("Pathfinding")]
        [SerializeField] protected float pathRecalculateInterval = 0.5f;
        protected List<Vector2> currentPath;
        protected int currentPathIndex;
        protected float pathRecalculateTimer;

        protected float currentHealth;
        protected Unit currentTarget;
        protected float attackCooldown;
        protected bool isDead = false;

        protected BlobGenerator blobVisual;
        protected SlimeBlobPhysics blobPhysics;
        protected Rigidbody2D rb;
        protected CircleCollider2D col;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsDead => isDead;
        public UnitTeam Team => team;
        public Unit CurrentTarget => currentTarget;

        public System.Action<Unit> OnDeath;
        public System.Action<float> OnDamaged;

        protected virtual void Awake()
        {
            blobVisual = GetComponent<BlobGenerator>();
            blobPhysics = GetComponent<SlimeBlobPhysics>();
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<CircleCollider2D>();

            rb.gravityScale = 0f;
            rb.linearDamping = 2f;
            col.isTrigger = false;
        }

        protected virtual void Start()
        {
            currentHealth = maxHealth;
            attackCooldown = 0f;
        }

        // Idle wander
        private Vector2 wanderTarget;
        private float wanderTimer;
        [SerializeField] protected float wanderInterval = 3f;
        [SerializeField] protected float wanderRadius = 2f;

        protected virtual void Update()
        {
            if (isDead) return;

            attackCooldown -= Time.deltaTime;

            FindTarget();
            
            if (currentTarget != null)
            {
                MoveTowardsTarget();
                TryAttack();
            }
            else
            {
                // Wander idle within blood vessels
                WanderIdle();
            }
        }

        protected virtual void FindTarget()
        {
            if (currentTarget != null && !currentTarget.IsDead)
            {
                float distance = Vector2.Distance(transform.position, currentTarget.transform.position);
                if (distance <= detectionRange)
                    return;
            }

            currentTarget = null;
            float closestDistance = detectionRange;

            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, detectionRange);
            
            foreach (var col in colliders)
            {
                Unit unit = col.GetComponent<Unit>();
                if (unit != null && unit.Team != team && !unit.IsDead)
                {
                    float distance = Vector2.Distance(transform.position, unit.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        currentTarget = unit;
                    }
                }
            }
        }

        protected virtual void MoveTowardsTarget()
        {
            if (currentTarget == null) return;

            float distance = Vector2.Distance(transform.position, currentTarget.transform.position);
            
            if (distance > attackRange)
            {
                pathRecalculateTimer -= Time.deltaTime;

                // Request new path periodically or if we don't have one
                if (currentPath == null || currentPath.Count == 0 || pathRecalculateTimer <= 0f)
                {
                    if (BloodstreamPathfinder.Instance != null)
                    {
                        currentPath = BloodstreamPathfinder.Instance.FindPath(transform.position, currentTarget.transform.position);
                        currentPathIndex = 0;
                    }
                    else
                    {
                        currentPath = new List<Vector2> { currentTarget.transform.position };
                        currentPathIndex = 0;
                    }
                    pathRecalculateTimer = pathRecalculateInterval + Random.Range(-0.05f, 0.05f);
                }

                // Follow the path
                if (currentPath != null && currentPathIndex < currentPath.Count)
                {
                    Vector2 targetWaypoint = currentPath[currentPathIndex];
                    
                    // If we reached the waypoint, advance to next
                    if (Vector2.Distance(transform.position, targetWaypoint) < 0.4f)
                    {
                        currentPathIndex++;
                        if (currentPathIndex < currentPath.Count)
                        {
                            targetWaypoint = currentPath[currentPathIndex];
                        }
                    }

                    Vector2 direction = (targetWaypoint - (Vector2)transform.position).normalized;
                    Vector2 desiredPos = (Vector2)transform.position + direction * moveSpeed * Time.deltaTime;

                    if (ArenaWalkableMask.Instance != null)
                    {
                        desiredPos = ArenaWalkableMask.Instance.ConstrainMovement(transform.position, desiredPos);
                        direction = (desiredPos - (Vector2)transform.position).normalized;
                    }

                    rb.linearVelocity = direction * moveSpeed;
                }
                else
                {
                    // Fallback to straight movement
                    Vector2 direction = ((Vector2)currentTarget.transform.position - (Vector2)transform.position).normalized;
                    rb.linearVelocity = direction * moveSpeed;
                }
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                currentPath = null;
            }
        }

        protected virtual void WanderIdle()
        {
            wanderTimer -= Time.deltaTime;

            if (wanderTimer <= 0f)
            {
                // Pick a new random walkable wander target
                Vector2 randomOffset = Random.insideUnitCircle * wanderRadius;
                Vector2 candidate = (Vector2)transform.position + randomOffset;

                if (ArenaWalkableMask.Instance != null)
                {
                    candidate = ArenaWalkableMask.Instance.GetNearestWalkablePosition(candidate, wanderRadius);
                }

                wanderTarget = candidate;
                wanderTimer = wanderInterval + Random.Range(-0.5f, 0.5f);
            }

            float distToWander = Vector2.Distance(transform.position, wanderTarget);
            if (distToWander > 0.3f)
            {
                Vector2 direction = (wanderTarget - (Vector2)transform.position).normalized;
                rb.linearVelocity = direction * moveSpeed * 0.4f; // Slower when wandering
            }
            else
            {
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.deltaTime * 3f);
            }
        }

        protected virtual void TryAttack()
        {
            if (currentTarget == null) return;
            if (attackCooldown > 0) return;

            float distance = Vector2.Distance(transform.position, currentTarget.transform.position);
            
            if (distance <= attackRange)
            {
                Attack(currentTarget);
                attackCooldown = 1f / attackSpeed;
            }
        }

        protected virtual void Attack(Unit target)
        {
            target.TakeDamage(damage, this);
            OnAttackPerformed();
        }

        protected virtual void OnAttackPerformed()
        {
            if (blobPhysics != null)
            {
                Vector2 impulse = (transform.position - currentTarget.transform.position).normalized * 0.5f;
                blobPhysics.AddImpulse(impulse);
            }
        }

        public virtual void TakeDamage(float damageAmount, Unit attacker)
        {
            if (isDead) return;

            currentHealth -= damageAmount;
            OnDamaged?.Invoke(damageAmount);

            if (blobPhysics != null)
            {
                Vector2 knockback = ((Vector2)transform.position - (Vector2)attacker.transform.position).normalized * 0.3f;
                blobPhysics.AddImpulse(knockback);
            }

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            if (isDead) return;

            isDead = true;
            currentHealth = 0;

            OnDeath?.Invoke(this);
            EventManager.TriggerEvent(team == UnitTeam.Immune ? GameEvents.UNIT_DIED : GameEvents.ENEMY_DIED, this);

            Destroy(gameObject, 0.5f);
        }

        public void Heal(float amount)
        {
            if (isDead) return;

            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }

    public enum UnitTeam
    {
        Immune,
        Disease
    }
}
