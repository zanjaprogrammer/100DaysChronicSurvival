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

        private Vector2 wanderTarget;
        private float wanderTimer;
        [SerializeField] protected float wanderInterval = 3f;
        [SerializeField] protected float wanderRadius = 2f;

        private bool usesAutonomousCombat = true;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsDead => isDead;
        public UnitTeam Team => team;
        public Unit CurrentTarget => currentTarget;
        public float MoveSpeed => moveSpeed;
        public float AttackRange => attackRange;
        public float DetectionRange => detectionRange;
        public bool UsesAutonomousCombat => usesAutonomousCombat;

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

        protected virtual void Update()
        {
            if (isDead) return;

            attackCooldown -= Time.deltaTime;

            if (!usesAutonomousCombat)
            {
                return;
            }

            FindTarget();

            if (currentTarget != null)
            {
                MoveTowardsTarget();
                TryAttack();
            }
            else
            {
                WanderIdle();
            }
        }

        public void SetAutonomousCombat(bool enabled)
        {
            usesAutonomousCombat = enabled;
            if (enabled) return;

            ClearTarget();
            StopMovement();
        }

        public void SetTarget(Unit target)
        {
            currentTarget = target != null && !target.IsDead ? target : null;
        }

        public void ClearTarget()
        {
            currentTarget = null;
            currentPath = null;
            currentPathIndex = 0;
        }

        public void StopMovement()
        {
            currentPath = null;
            currentPathIndex = 0;
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }

        public void MoveToPoint(Vector2 destination, float stoppingDistance = 0.25f, float speedMultiplier = 1f)
        {
            if (isDead || rb == null)
            {
                return;
            }

            pathRecalculateTimer -= Time.deltaTime;
            if (currentPath == null || currentPath.Count == 0 || pathRecalculateTimer <= 0f)
            {
                currentPath = CalculatePathTo(destination);
                currentPathIndex = 0;
                pathRecalculateTimer = pathRecalculateInterval + Random.Range(-0.05f, 0.05f);
            }

            if (AdvancePath(ref destination, stoppingDistance))
            {
                return;
            }

            Vector2 direction = (destination - (Vector2)transform.position).normalized;
            MoveInDirection(direction, speedMultiplier);
        }

        public void MoveDirectly(Vector2 direction, float speedMultiplier = 1f)
        {
            currentPath = null;
            currentPathIndex = 0;
            MoveInDirection(direction, speedMultiplier);
        }

        public bool IsWithinAttackRange(Unit target)
        {
            return target != null && !target.IsDead && Vector2.Distance(transform.position, target.transform.position) <= attackRange;
        }

        public bool TryAttackAssignedTarget()
        {
            if (currentTarget == null || currentTarget.IsDead)
            {
                ClearTarget();
                return false;
            }

            if (attackCooldown > 0f)
            {
                return false;
            }

            if (!IsWithinAttackRange(currentTarget))
            {
                return false;
            }

            Attack(currentTarget);
            attackCooldown = 1f / attackSpeed;
            return true;
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
                MoveToPoint(currentTarget.transform.position, attackRange * 0.85f);
            }
            else
            {
                StopMovement();
            }
        }

        protected virtual void WanderIdle()
        {
            wanderTimer -= Time.deltaTime;

            if (wanderTimer <= 0f)
            {
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
                MoveToPoint(wanderTarget, 0.25f, 0.4f);
            }
            else if (rb != null)
            {
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.deltaTime * 3f);
            }
        }

        protected virtual void TryAttack()
        {
            TryAttackAssignedTarget();
        }

        protected virtual void Attack(Unit target)
        {
            target.TakeDamage(damage, this);
            OnAttackPerformed();
        }

        protected virtual void OnAttackPerformed()
        {
            if (blobPhysics != null && currentTarget != null)
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

        public void ApplyCombatMultipliers(float attackSpeedMult, float damageMult, float maxHealthMult)
        {
            if (attackSpeedMult <= 0f || damageMult <= 0f || maxHealthMult <= 0f)
            {
                return;
            }

            attackSpeed *= attackSpeedMult;
            damage *= damageMult;

            float healthRatio = maxHealth > 0f ? currentHealth / maxHealth : 1f;
            maxHealth *= maxHealthMult;
            currentHealth = Mathf.Min(maxHealth, currentHealth * healthRatio * maxHealthMult);
        }

        protected virtual void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        private List<Vector2> CalculatePathTo(Vector2 destination)
        {
            if (BloodstreamPathfinder.Instance != null)
            {
                return BloodstreamPathfinder.Instance.FindPath(transform.position, destination);
            }

            return new List<Vector2> { destination };
        }

        private bool AdvancePath(ref Vector2 destination, float stoppingDistance)
        {
            if (currentPath == null || currentPathIndex >= currentPath.Count)
            {
                return false;
            }

            Vector2 targetWaypoint = currentPath[currentPathIndex];
            if (Vector2.Distance(transform.position, targetWaypoint) < 0.4f)
            {
                currentPathIndex++;
                if (currentPathIndex < currentPath.Count)
                {
                    targetWaypoint = currentPath[currentPathIndex];
                }
                else
                {
                    targetWaypoint = destination;
                }
            }

            destination = targetWaypoint;
            if (Vector2.Distance(transform.position, destination) <= stoppingDistance)
            {
                StopMovement();
                return true;
            }

            return false;
        }

        private void MoveInDirection(Vector2 direction, float speedMultiplier)
        {
            if (rb == null)
            {
                return;
            }

            if (direction.sqrMagnitude <= 0.0001f)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }

            Vector2 normalized = direction.normalized;
            Vector2 desiredPos = (Vector2)transform.position + normalized * moveSpeed * Mathf.Max(speedMultiplier, 0f) * Time.deltaTime;

            if (ArenaWalkableMask.Instance != null)
            {
                desiredPos = ArenaWalkableMask.Instance.ConstrainMovement(transform.position, desiredPos);
                normalized = desiredPos - (Vector2)transform.position;
                if (normalized.sqrMagnitude > 0.0001f)
                {
                    normalized.Normalize();
                }
            }

            rb.linearVelocity = normalized * moveSpeed * Mathf.Max(speedMultiplier, 0f);
        }
    }

    public enum UnitTeam
    {
        Immune,
        Disease
    }
}
