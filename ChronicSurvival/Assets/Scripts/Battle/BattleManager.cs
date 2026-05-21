using UnityEngine;
using System.Collections.Generic;
using ChronicSurvival.Core;
using ChronicSurvival.Units;

namespace ChronicSurvival.Battle
{
    public class BattleManager : MonoBehaviour
    {
        public static BattleManager Instance { get; private set; }

        [Header("Battle Settings")]
        [SerializeField] private float battleDuration = 60f;
        [SerializeField] private bool useTimer = true;

        [Header("Spawn Areas")]
        [SerializeField] private Transform immuneSpawnArea;
        [SerializeField] private Transform enemySpawnArea;
        [SerializeField] private float spawnRadius = 3f;

        [Header("Prefabs")]
        [SerializeField] private GameObject immuneCellPrefab;
        [SerializeField] private GameObject enemyPrefab;

        [Header("Debug")]
        [SerializeField] private bool debugMode = true;

        private List<ImmuneCell> immuneCells = new List<ImmuneCell>();
        private List<Enemy> enemies = new List<Enemy>();
        private float battleTimer;
        private bool battleActive = false;

        public bool BattleActive => battleActive;
        public float BattleTimer => battleTimer;
        public int ImmuneCount => immuneCells.Count;
        public int EnemyCount => enemies.Count;

        public System.Action OnBattleStart;
        public System.Action<bool> OnBattleEnd;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
            }
        }

        private void OnGameStateChanged(GameState newState)
        {
            if (newState == GameState.Battle)
            {
                StartGameplayBattle();
            }
            else if (newState != GameState.Paused && newState != GameState.Initializing)
            {
                // Stop spawning and clear units if state moves away from Battle/Pause
                var infectionNodes = FindObjectsByType<InfectionNode>(FindObjectsSortMode.None);
                foreach (var node in infectionNodes)
                {
                    if (node != null)
                    {
                        node.DeactivateNode();
                    }
                }
            }
        }

        private void StartGameplayBattle()
        {
            if (debugMode) Debug.Log("[BattleManager] Transitioned to Battle state. Auto-spawning cells and nodes...");

            // Clear any lingering units
            ClearAllUnits();

            // Spawn immune units
            UnitSpawner spawner = FindAnyObjectByType<UnitSpawner>();
            if (spawner != null)
            {
                spawner.SpawnAllUnits();
            }
            else
            {
                Debug.LogWarning("[BattleManager] UnitSpawner not found in scene!");
            }

            // Activate infection nodes
            var infectionNodes = FindObjectsByType<InfectionNode>(FindObjectsSortMode.None);
            foreach (var node in infectionNodes)
            {
                if (node != null)
                {
                    node.ActivateNode();
                }
            }

            // Start the actual battle sequence
            StartBattle();
        }

        private void Update()
        {
            if (!battleActive) return;

            if (useTimer)
            {
                battleTimer -= Time.deltaTime;
                if (battleTimer <= 0)
                {
                    EndBattle(true);
                    return;
                }
            }

            CleanupDeadUnits();
            CheckBattleEnd();
        }

        public void StartBattle()
        {
            if (battleActive) return;

            battleActive = true;
            battleTimer = battleDuration;

            OnBattleStart?.Invoke();
            EventManager.TriggerEvent(GameEvents.BATTLE_STARTED);

            if (debugMode) Debug.Log("[BattleManager] Battle started!");
        }

        public void EndBattle(bool victory)
        {
            if (!battleActive) return;

            battleActive = false;

            OnBattleEnd?.Invoke(victory);
            EventManager.TriggerEvent(GameEvents.BATTLE_ENDED, victory);

            if (debugMode) Debug.Log($"[BattleManager] Battle ended! Victory: {victory}");

            GameManager.Instance?.EndBattle(victory);
        }

        private void CleanupDeadUnits()
        {
            immuneCells.RemoveAll(cell => cell == null || cell.IsDead);
            enemies.RemoveAll(enemy => enemy == null || enemy.IsDead);
        }

        private void CheckBattleEnd()
        {
            if (immuneCells.Count == 0)
            {
                EndBattle(false);
            }
            else if (enemies.Count == 0 && !useTimer)
            {
                EndBattle(true);
            }
        }

        public ImmuneCell SpawnImmuneCell(ImmuneCellType type, Vector2? position = null)
        {
            Vector2 spawnPos = position ?? GetRandomSpawnPosition(immuneSpawnArea);
            
            GameObject obj = Instantiate(immuneCellPrefab, spawnPos, Quaternion.identity);
            ImmuneCell cell = obj.GetComponent<ImmuneCell>();
            
            if (cell != null)
            {
                cell.SetCellType(type);
                cell.OnDeath += OnImmuneCellDeath;
                immuneCells.Add(cell);

                EventManager.TriggerEvent(GameEvents.UNIT_SPAWNED, cell);
                
                if (debugMode) Debug.Log($"[BattleManager] Spawned {type}");
            }

            return cell;
        }

        public Enemy SpawnEnemy(EnemyType type, DiseaseType disease, Vector2? position = null)
        {
            Vector2 spawnPos = position ?? GetRandomSpawnPosition(enemySpawnArea);
            
            GameObject obj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            Enemy enemy = obj.GetComponent<Enemy>();
            
            if (enemy != null)
            {
                enemy.SetEnemyType(type, disease);
                enemy.OnDeath += OnEnemyDeath;
                enemies.Add(enemy);

                EventManager.TriggerEvent(GameEvents.ENEMY_SPAWNED, enemy);
                
                if (debugMode) Debug.Log($"[BattleManager] Spawned {disease} {type}");
            }

            return enemy;
        }

        private Vector2 GetRandomSpawnPosition(Transform spawnArea)
        {
            if (spawnArea == null)
                return Random.insideUnitCircle * spawnRadius;

            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            return (Vector2)spawnArea.position + randomOffset;
        }

        private void OnImmuneCellDeath(Unit unit)
        {
            if (debugMode) Debug.Log($"[BattleManager] Immune cell died");
        }

        private void OnEnemyDeath(Unit unit)
        {
            if (debugMode) Debug.Log($"[BattleManager] Enemy died");
        }

        public void ClearAllUnits()
        {
            foreach (var cell in immuneCells)
            {
                if (cell != null) Destroy(cell.gameObject);
            }

            foreach (var enemy in enemies)
            {
                if (enemy != null) Destroy(enemy.gameObject);
            }

            immuneCells.Clear();
            enemies.Clear();
        }

        private void OnDrawGizmos()
        {
            if (immuneSpawnArea != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(immuneSpawnArea.position, spawnRadius);
            }

            if (enemySpawnArea != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(enemySpawnArea.position, spawnRadius);
            }
        }
    }
}
