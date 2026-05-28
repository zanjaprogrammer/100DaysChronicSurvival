using System.Collections.Generic;
using UnityEngine;
using ChronicSurvival.Arena;
using ChronicSurvival.Units;

namespace ChronicSurvival.Battle
{
    public class ImmuneSquadController : MonoBehaviour
    {
        private const int OfficerCount = 10;

        [Header("Movement")]
        [SerializeField] private float idleFormationRadius = 2.4f;
        [SerializeField] private float movingFormationRadius = 3.2f;
        [SerializeField] private float formationSpacingJitter = 0.18f;
        [SerializeField] private float leaderInputDeadZone = 0.12f;
        [SerializeField] private float attackSearchRadius = 14f;
        [SerializeField] private float retargetInterval = 0.4f;

        private readonly List<ImmuneCell> officers = new List<ImmuneCell>();
        private readonly List<Enemy> cachedTargets = new List<Enemy>();
        private readonly Dictionary<ImmuneCell, Vector2> formationOffsets = new Dictionary<ImmuneCell, Vector2>();

        private ImmuneCell leader;
        private Vector2 moveInput;
        private SquadCommandMode mode = SquadCommandMode.Follow;
        private float retargetTimer;

        public ImmuneCell Leader => leader;
        public IReadOnlyList<ImmuneCell> Officers => officers;
        public Vector2 MoveInput => moveInput;
        public SquadCommandMode Mode => mode;
        public bool HasSquad => leader != null;

        public void BuildSquad(Vector2 origin)
        {
            ClearSquad();

            if (BattleManager.Instance == null)
            {
                return;
            }

            leader = BattleManager.Instance.SpawnSquadImmuneCell(ImmuneSquadRole.Leader, origin);
            if (leader == null)
            {
                return;
            }

            for (int i = 0; i < OfficerCount; i++)
            {
                Vector2 spawn = origin + Random.insideUnitCircle * 1.6f;
                if (ArenaWalkableMask.Instance != null)
                {
                    spawn = ArenaWalkableMask.Instance.GetNearestWalkablePosition(spawn, 6f);
                }

                ImmuneCell officer = BattleManager.Instance.SpawnSquadImmuneCell(ImmuneSquadRole.Officer, spawn);
                if (officer != null)
                {
                    officers.Add(officer);
                }
            }

            RebuildFormationOffsets();
            SetFollowMode();
        }

        public void ClearSquad()
        {
            leader = null;
            officers.Clear();
            cachedTargets.Clear();
            formationOffsets.Clear();
            moveInput = Vector2.zero;
            mode = SquadCommandMode.Follow;
        }

        public void SetMoveInput(Vector2 input)
        {
            moveInput = Vector2.ClampMagnitude(input, 1f);
            if (mode == SquadCommandMode.Attack && moveInput.sqrMagnitude > leaderInputDeadZone * leaderInputDeadZone)
            {
                SetFollowMode();
            }
        }

        public void RequestAttackMode()
        {
            if (leader == null || leader.IsDead)
            {
                return;
            }

            if (mode == SquadCommandMode.Attack)
            {
                return;
            }

            mode = SquadCommandMode.Attack;
            retargetTimer = 0f;
            AssignTargets();
        }

        public void SetFollowMode()
        {
            mode = SquadCommandMode.Follow;
            cachedTargets.Clear();

            if (leader != null)
            {
                leader.ClearTarget();
            }

            for (int i = officers.Count - 1; i >= 0; i--)
            {
                if (officers[i] == null || officers[i].IsDead)
                {
                    officers.RemoveAt(i);
                    continue;
                }

                officers[i].ClearTarget();
            }
        }

        private void Update()
        {
            if (leader == null || leader.IsDead)
            {
                return;
            }

            CleanupMembers();

            if (mode == SquadCommandMode.Attack)
            {
                UpdateAttackMode();
            }
            else
            {
                UpdateFollowMode();
            }
        }

        private void UpdateFollowMode()
        {
            float moveMagnitude = moveInput.magnitude;
            if (moveMagnitude > leaderInputDeadZone)
            {
                leader.MoveDirectly(moveInput, 1f);
            }
            else
            {
                leader.StopMovement();
            }

            float formationRadius = moveMagnitude > leaderInputDeadZone ? movingFormationRadius : idleFormationRadius;
            Vector2 forward = moveMagnitude > leaderInputDeadZone ? moveInput.normalized : Vector2.up;
            UpdateFormationAroundLeader(formationRadius, forward);
        }

        private void UpdateAttackMode()
        {
            retargetTimer -= Time.deltaTime;
            if (retargetTimer <= 0f)
            {
                AssignTargets();
                retargetTimer = retargetInterval;
            }

            EngageAssignedTarget(leader);
            for (int i = officers.Count - 1; i >= 0; i--)
            {
                EngageAssignedTarget(officers[i]);
            }

            if (cachedTargets.Count == 0)
            {
                SetFollowMode();
            }
        }

        private void EngageAssignedTarget(ImmuneCell cell)
        {
            if (cell == null || cell.IsDead)
            {
                return;
            }

            Unit target = cell.CurrentTarget;
            if (target == null || target.IsDead)
            {
                return;
            }

            if (!cell.IsWithinAttackRange(target))
            {
                cell.MoveToPoint(target.transform.position, cell.AttackRange * 0.8f);
                return;
            }

            cell.StopMovement();
            cell.TryAttackAssignedTarget();
        }

        private void AssignTargets()
        {
            cachedTargets.Clear();
            Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int i = 0; i < allEnemies.Length; i++)
            {
                Enemy enemy = allEnemies[i];
                if (enemy == null || enemy.IsDead)
                {
                    continue;
                }

                if (Vector2.Distance(leader.transform.position, enemy.transform.position) <= attackSearchRadius)
                {
                    cachedTargets.Add(enemy);
                }
            }

            cachedTargets.Sort((a, b) =>
                Vector2.Distance(leader.transform.position, a.transform.position)
                    .CompareTo(Vector2.Distance(leader.transform.position, b.transform.position)));

            if (cachedTargets.Count == 0)
            {
                leader.ClearTarget();
                for (int i = 0; i < officers.Count; i++)
                {
                    officers[i]?.ClearTarget();
                }
                return;
            }

            leader.SetTarget(cachedTargets[0]);
            for (int i = 0; i < officers.Count; i++)
            {
                if (officers[i] == null || officers[i].IsDead)
                {
                    continue;
                }

                Enemy target = cachedTargets[i % cachedTargets.Count];
                officers[i].SetTarget(target);
            }
        }

        private void UpdateFormationAroundLeader(float formationRadius, Vector2 forward)
        {
            if (officers.Count == 0)
            {
                return;
            }

            if (formationOffsets.Count != officers.Count)
            {
                RebuildFormationOffsets();
            }

            float facingAngle = Mathf.Atan2(forward.y, forward.x);
            Quaternion rotation = Quaternion.Euler(0f, 0f, facingAngle * Mathf.Rad2Deg - 90f);

            for (int i = 0; i < officers.Count; i++)
            {
                ImmuneCell officer = officers[i];
                if (officer == null || officer.IsDead)
                {
                    continue;
                }

                Vector2 offset = formationOffsets.TryGetValue(officer, out Vector2 value) ? value : Vector2.zero;
                Vector2 rotated = rotation * (offset * formationRadius);
                Vector2 destination = (Vector2)leader.transform.position + rotated;
                if (ArenaWalkableMask.Instance != null)
                {
                    destination = ArenaWalkableMask.Instance.GetNearestWalkablePosition(destination, formationRadius + 2f);
                }

                officer.MoveToPoint(destination, 0.35f, 0.94f);
            }
        }

        private void RebuildFormationOffsets()
        {
            formationOffsets.Clear();
            if (officers.Count == 0)
            {
                return;
            }

            float step = Mathf.PI * 2f / officers.Count;
            for (int i = 0; i < officers.Count; i++)
            {
                float angle = step * i;
                Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                offset *= 1f + Random.Range(-formationSpacingJitter, formationSpacingJitter);
                formationOffsets[officers[i]] = offset;
            }
        }

        private void CleanupMembers()
        {
            for (int i = officers.Count - 1; i >= 0; i--)
            {
                if (officers[i] == null || officers[i].IsDead)
                {
                    officers.RemoveAt(i);
                }
            }

            cachedTargets.RemoveAll(enemy => enemy == null || enemy.IsDead);
        }
    }

    public enum SquadCommandMode
    {
        Follow,
        Attack
    }
}
