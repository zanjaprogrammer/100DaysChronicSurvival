using UnityEngine;
using System.Collections.Generic;
using ChronicSurvival.Core;
using ChronicSurvival.Units;
using ChronicSurvival.Arena;

namespace ChronicSurvival.Battle
{
    public class UnitSpawner : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [SerializeField] private Transform spawnArea;
        [SerializeField] private float spawnRadius = 3f;
        [SerializeField] private float spawnSpacing = 1f;

        [Header("Available Units")]
        [SerializeField] private int maxMacrophages = 3;
        [SerializeField] private int maxTCells = 2;
        [SerializeField] private int maxBCells = 2;
        [SerializeField] private int maxNKCells = 1;
        [SerializeField] private int maxNeutrophils = 4;

        [Header("Starting Units")]
        [SerializeField] private int startingMacrophages = 2;
        [SerializeField] private int startingTCells = 1;
        [SerializeField] private int startingBCells = 1;
        [SerializeField] private int startingNKCells = 0;
        [SerializeField] private int startingNeutrophils = 2;

        private Dictionary<ImmuneCellType, int> currentUnits = new Dictionary<ImmuneCellType, int>();
        private Dictionary<ImmuneCellType, int> maxUnits = new Dictionary<ImmuneCellType, int>();

        public System.Action<ImmuneCellType, int, int> OnUnitCountChanged;

        private void Awake()
        {
            InitializeUnitCounts();
        }

        private void InitializeUnitCounts()
        {
            maxUnits[ImmuneCellType.Macrophage] = maxMacrophages;
            maxUnits[ImmuneCellType.TCell] = maxTCells;
            maxUnits[ImmuneCellType.BCell] = maxBCells;
            maxUnits[ImmuneCellType.NKCell] = maxNKCells;
            maxUnits[ImmuneCellType.Neutrophil] = maxNeutrophils;

            currentUnits[ImmuneCellType.Macrophage] = startingMacrophages;
            currentUnits[ImmuneCellType.TCell] = startingTCells;
            currentUnits[ImmuneCellType.BCell] = startingBCells;
            currentUnits[ImmuneCellType.NKCell] = startingNKCells;
            currentUnits[ImmuneCellType.Neutrophil] = startingNeutrophils;
        }

        public void SpawnAllUnits()
        {
            if (BattleManager.Instance == null)
            {
                Debug.LogError("[UnitSpawner] BattleManager not found!");
                return;
            }

            List<Vector2> spawnPositions = GenerateSpawnPositions(GetTotalUnitCount());
            int posIndex = 0;

            foreach (var kvp in currentUnits)
            {
                ImmuneCellType cellType = kvp.Key;
                int count = kvp.Value;

                for (int i = 0; i < count; i++)
                {
                    if (posIndex < spawnPositions.Count)
                    {
                        BattleManager.Instance.SpawnImmuneCell(cellType, spawnPositions[posIndex]);
                        posIndex++;
                    }
                }
            }

            EventManager.TriggerEvent(GameEvents.UNITS_SPAWNED);
        }

        private List<Vector2> GenerateSpawnPositions(int count)
        {
            List<Vector2> positions = new List<Vector2>();
            Vector2 center = spawnArea != null ? (Vector2)spawnArea.position : Vector2.zero;
            if (ArenaWalkableMask.Instance != null)
            {
                center = ArenaWalkableMask.Instance.GetNearestWalkablePosition(center, 12f);
            }

            int rows = Mathf.CeilToInt(Mathf.Sqrt(count));
            int cols = Mathf.CeilToInt((float)count / rows);

            float startX = center.x - (cols - 1) * spawnSpacing * 0.5f;
            float startY = center.y - (rows - 1) * spawnSpacing * 0.5f;

            for (int i = 0; i < count; i++)
            {
                int row = i / cols;
                int col = i % cols;

                Vector2 pos = new Vector2(
                    startX + col * spawnSpacing,
                    startY + row * spawnSpacing
                );

                Vector2 randomOffset = Random.insideUnitCircle * 0.2f;
                Vector2 candidate = pos + randomOffset;
                if (ArenaWalkableMask.Instance != null)
                {
                    candidate = ArenaWalkableMask.Instance.GetNearestWalkablePosition(candidate, 8f);
                }
                positions.Add(candidate);
            }

            return positions;
        }

        public bool CanAddUnit(ImmuneCellType cellType)
        {
            return currentUnits[cellType] < maxUnits[cellType];
        }

        public bool AddUnit(ImmuneCellType cellType)
        {
            if (!CanAddUnit(cellType)) return false;

            currentUnits[cellType]++;
            OnUnitCountChanged?.Invoke(cellType, currentUnits[cellType], maxUnits[cellType]);
            return true;
        }

        public bool RemoveUnit(ImmuneCellType cellType)
        {
            if (currentUnits[cellType] <= 0) return false;

            currentUnits[cellType]--;
            OnUnitCountChanged?.Invoke(cellType, currentUnits[cellType], maxUnits[cellType]);
            return true;
        }

        public int GetUnitCount(ImmuneCellType cellType)
        {
            return currentUnits[cellType];
        }

        public int GetMaxUnitCount(ImmuneCellType cellType)
        {
            return maxUnits[cellType];
        }

        public int GetTotalUnitCount()
        {
            int total = 0;
            foreach (var count in currentUnits.Values)
            {
                total += count;
            }
            return total;
        }

        public void ResetToDefaults()
        {
            currentUnits[ImmuneCellType.Macrophage] = startingMacrophages;
            currentUnits[ImmuneCellType.TCell] = startingTCells;
            currentUnits[ImmuneCellType.BCell] = startingBCells;
            currentUnits[ImmuneCellType.NKCell] = startingNKCells;
            currentUnits[ImmuneCellType.Neutrophil] = startingNeutrophils;

            foreach (var kvp in currentUnits)
            {
                OnUnitCountChanged?.Invoke(kvp.Key, kvp.Value, maxUnits[kvp.Key]);
            }
        }

        public void UpgradeMaxUnits(ImmuneCellType cellType, int amount)
        {
            maxUnits[cellType] += amount;
            OnUnitCountChanged?.Invoke(cellType, currentUnits[cellType], maxUnits[cellType]);
        }

        private void OnDrawGizmos()
        {
            if (spawnArea != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireSphere(spawnArea.position, spawnRadius);
            }
        }
    }
}
