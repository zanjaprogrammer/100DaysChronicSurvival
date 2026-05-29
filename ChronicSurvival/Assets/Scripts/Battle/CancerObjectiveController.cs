using UnityEngine;
using ChronicSurvival.Core;
using ChronicSurvival.Units;
using ChronicSurvival.Arena;
using ChronicSurvival.UI;

namespace ChronicSurvival.Battle
{
    public class CancerObjectiveController : MonoBehaviour
    {
        [Header("Spawn")]
        [SerializeField] private int minInitialCancerCells = 10;
        [SerializeField] private int maxInitialCancerCells = 15;
        [SerializeField] private int maxAliveCancerCells = 28;
        [SerializeField] private float spawnSpacing = 1.4f;
        [SerializeField] private float spawnAheadDistance = 20f;
        [SerializeField] private float spawnDepthRange = 10f;

        private readonly System.Collections.Generic.List<Enemy> trackedEnemies = new System.Collections.Generic.List<Enemy>();

        private bool objectiveAssigned;
        private bool objectiveCompleted;
        private bool isRefreshingHud;
        private int initialTargetCount;

        public bool ObjectiveAssigned => objectiveAssigned;
        public bool ObjectiveCompleted => objectiveCompleted;
        public int RemainingCancerCells
        {
            get
            {
                CleanupTrackedEnemies();
                return trackedEnemies.Count;
            }
        }
        public int InitialTargetCount => initialTargetCount;
        public float Progress01 => initialTargetCount <= 0 ? 0f : 1f - Mathf.Clamp01((float)RemainingCancerCells / initialTargetCount);

        private void Start()
        {
            AssignObjectiveIfNeeded();
        }

        private void Update()
        {
            if (!objectiveAssigned || objectiveCompleted)
            {
                return;
            }

            CleanupTrackedEnemies();
            if (trackedEnemies.Count == 0)
            {
                CompleteObjective();
            }
        }

        public void AssignObjectiveIfNeeded()
        {
            if (objectiveAssigned)
            {
                return;
            }

            objectiveAssigned = true;
            objectiveCompleted = false;
            initialTargetCount = Random.Range(minInitialCancerCells, maxInitialCancerCells + 1);

            Vector2 anchor = ResolveSpawnAnchor();
            SpawnInitialCancerWave(anchor, initialTargetCount);

            ObjectiveHUDController hud = FindFirstObjectByType<ObjectiveHUDController>(FindObjectsInactive.Include);
            hud?.ShowObjective("Hancurkan Sel Kanker", "Eliminasi koloni kanker di segmen berikutnya.");
            hud?.SetProgress(0f, $"0 / {initialTargetCount} dibersihkan");
        }

        public bool CanSpawnDivision()
        {
            CleanupTrackedEnemies();
            return objectiveAssigned && !objectiveCompleted && trackedEnemies.Count < maxAliveCancerCells;
        }

        public Enemy RegisterDivision(Enemy source, Vector2 nearPosition)
        {
            if (!CanSpawnDivision() || BattleManager.Instance == null)
            {
                return null;
            }

            Vector2 candidate = nearPosition + Random.insideUnitCircle * 1.25f;
            if (ArenaWalkableMask.Instance != null)
            {
                candidate = ArenaWalkableMask.Instance.GetNearestWalkablePosition(candidate, 5f);
            }

            Enemy spawned = BattleManager.Instance.SpawnEnemy(EnemyType.Basic, DiseaseType.Cancer, candidate);
            if (spawned == null)
            {
                return null;
            }

            TrackEnemy(spawned);
            return spawned;
        }

        private void SpawnInitialCancerWave(Vector2 anchor, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 position = anchor + Random.insideUnitCircle * (spawnSpacing * 3f);
                if (ArenaWalkableMask.Instance != null)
                {
                    position = ArenaWalkableMask.Instance.GetRandomWalkablePosition();
                    position = ArenaWalkableMask.Instance.GetNearestWalkablePosition(position, 10f);
                }

                Enemy enemy = BattleManager.Instance.SpawnEnemy(EnemyType.Basic, DiseaseType.Cancer, position);
                if (enemy != null)
                {
                    TrackEnemy(enemy);
                }
            }
        }

        private Vector2 ResolveSpawnAnchor()
        {
            if (ArenaWalkableMask.Instance != null)
            {
                return ArenaWalkableMask.Instance.GetRandomWalkablePosition();
            }

            ImmuneSquadController squad = FindFirstObjectByType<ImmuneSquadController>(FindObjectsInactive.Include);
            Vector2 basePos = squad != null && squad.Leader != null
                ? (Vector2)squad.Leader.transform.position
                : Vector2.zero;

            Vector2 candidate = basePos + Vector2.right * spawnAheadDistance;
            candidate += new Vector2(0f, Random.Range(-spawnDepthRange, spawnDepthRange));

            if (ArenaWalkableMask.Instance != null)
            {
                candidate = ArenaWalkableMask.Instance.GetNearestWalkablePosition(candidate, spawnAheadDistance + 8f);
            }

            return candidate;
        }

        private void TrackEnemy(Enemy enemy)
        {
            if (enemy == null)
            {
                return;
            }

            if (!enemy.TryGetComponent<CancerCellBehaviour>(out _))
            {
                CancerCellBehaviour behaviour = enemy.gameObject.AddComponent<CancerCellBehaviour>();
                behaviour.Initialize(this);
            }

            enemy.OnDeath -= OnTrackedEnemyDeath;
            enemy.OnDeath += OnTrackedEnemyDeath;
            trackedEnemies.Add(enemy);
            RefreshHud();
        }

        private void OnTrackedEnemyDeath(Unit unit)
        {
            Enemy enemy = unit as Enemy;
            if (enemy != null)
            {
                trackedEnemies.Remove(enemy);
            }
            RefreshHud();
        }

        private void CleanupTrackedEnemies()
        {
            trackedEnemies.RemoveAll(enemy => enemy == null || enemy.IsDead);
            RefreshHud();
        }

        private void RefreshHud()
        {
            if (!objectiveAssigned || objectiveCompleted || isRefreshingHud)
            {
                return;
            }

            ObjectiveHUDController hud = FindFirstObjectByType<ObjectiveHUDController>(FindObjectsInactive.Include);
            if (hud == null)
            {
                return;
            }

            isRefreshingHud = true;
            int remaining = trackedEnemies.Count;
            int cleared = Mathf.Max(0, initialTargetCount - remaining);
            float progress = initialTargetCount <= 0 ? 0f : 1f - Mathf.Clamp01((float)remaining / initialTargetCount);
            hud.SetProgress(progress, $"{cleared} / {initialTargetCount} dibersihkan");
            isRefreshingHud = false;
        }

        private void CompleteObjective()
        {
            objectiveCompleted = true;
            ObjectiveHUDController hud = FindFirstObjectByType<ObjectiveHUDController>(FindObjectsInactive.Include);
            hud?.SetProgress(1f, "Objektif selesai");
            EventManager.TriggerEvent(GameEvents.BATTLE_ENDED, true);
        }
    }
}
