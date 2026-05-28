using UnityEngine;
using System.Collections.Generic;
using ChronicSurvival.Core;
using ChronicSurvival.Cards;
using ChronicSurvival.Units;
using ChronicSurvival.ProceduralVisuals;
using ChronicSurvival.Arena;

namespace ChronicSurvival.Battle
{
    public class BattleManager : MonoBehaviour
    {
        public static BattleManager Instance { get; private set; }

        [Header("Battle Settings")]
        [SerializeField] private float battleDuration = 60f;
        [SerializeField] private bool useTimer = false;

        [Header("Spawn Areas")]
        [SerializeField] private Transform immuneSpawnArea;
        [SerializeField] private Transform enemySpawnArea;
        [SerializeField] private float spawnRadius = 3f;

        [Header("Prefabs")]
        [SerializeField] private GameObject immuneCellPrefab;
        [SerializeField] private GameObject enemyPrefab;

        [Header("Gameplay Mode")]
        [SerializeField] private bool spawnGameplayUnits = true;
        [SerializeField] private bool allowBattleGameOver = false;
        [SerializeField] private bool useObjectiveDrivenMode = true;

        [Header("Debug")]
        [SerializeField] private bool debugMode = true;

        private readonly List<ImmuneCell> immuneCells = new List<ImmuneCell>();
        private readonly List<Enemy> enemies = new List<Enemy>();
        private float battleTimer;
        private bool battleActive = false;

        public bool BattleActive => battleActive;
        public float BattleTimer => battleTimer;
        public int ImmuneCount => immuneCells.Count;
        public int EnemyCount => enemies.Count;
        public IReadOnlyList<Enemy> Enemies => enemies;
        public IReadOnlyList<ImmuneCell> ImmuneCells => immuneCells;

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

        private void OnEnable()
        {
            ConnectToGameManager();
            GameManager.OnInstanceReady += ConnectToGameManager;
        }

        private void Start()
        {
            ConnectToGameManager();
        }

        private void OnDisable()
        {
            GameManager.OnInstanceReady -= ConnectToGameManager;
            DisconnectFromGameManager();
        }

        private void OnDestroy()
        {
            DisconnectFromGameManager();
        }

        public void ConnectToGameManager()
        {
            if (GameManager.Instance == null) return;

            GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;

            if (GameManager.Instance.CurrentState == GameState.Battle && !battleActive)
            {
                StartGameplayBattle();
            }
        }

        private void DisconnectFromGameManager()
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
                var infectionNodes = FindObjectsByType<InfectionNode>(FindObjectsInactive.Include, FindObjectsSortMode.None);
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
            if (debugMode) Debug.Log("[BattleManager] Transitioned to Battle state.");

            ClearAllUnits();
            EnsureRuntimeDefaults();

            if (spawnGameplayUnits)
            {
                UnitSpawner spawner = FindFirstObjectByType<UnitSpawner>(FindObjectsInactive.Include);
                if (spawner != null)
                {
                    spawner.SpawnAllUnits();
                }
                else
                {
                    Debug.LogWarning("[BattleManager] UnitSpawner not found in scene!");
                }
            }

            if (!useObjectiveDrivenMode)
            {
                var infectionNodes = FindObjectsByType<InfectionNode>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (var node in infectionNodes)
                {
                    if (node != null)
                    {
                        node.ActivateNode();
                    }
                }
            }

            StartBattle();
        }

        private void EnsureRuntimeDefaults()
        {
            if (immuneCellPrefab == null) immuneCellPrefab = CreateRuntimeUnitPrefab("RuntimeImmuneCellPrefab", true);
            if (enemyPrefab == null) enemyPrefab = CreateRuntimeUnitPrefab("RuntimeEnemyPrefab", false);

            if (FindFirstObjectByType<UnitSpawner>(FindObjectsInactive.Include) == null)
            {
                GameObject spawnerObj = new GameObject("UnitSpawner");
                spawnerObj.transform.position = Vector3.zero;
                spawnerObj.AddComponent<UnitSpawner>();
            }

            if (FindFirstObjectByType<CancerObjectiveController>(FindObjectsInactive.Include) == null)
            {
                new GameObject("CancerObjectiveController").AddComponent<CancerObjectiveController>();
            }

            if (useObjectiveDrivenMode)
            {
                var infectionNodes = FindObjectsByType<InfectionNode>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (var node in infectionNodes)
                {
                    if (node != null)
                    {
                        node.DeactivateNode();
                    }
                }
            }
            else if (FindFirstObjectByType<InfectionNode>(FindObjectsInactive.Include) == null)
            {
                CreateRuntimeNode("DiabetesNode", DiseaseType.Diabetes, new Vector2(9f, 1.8f));
                CreateRuntimeNode("HypertensionNode", DiseaseType.Hypertension, new Vector2(-8f, 3.4f));
                CreateRuntimeNode("CancerNode", DiseaseType.Cancer, new Vector2(7f, -3.8f));
            }
        }

        private GameObject CreateRuntimeUnitPrefab(string name, bool immune)
        {
            GameObject prefab = new GameObject(name);
            prefab.SetActive(false);
            prefab.AddComponent<BlobGenerator>();
            prefab.AddComponent<SlimeBlobPhysics>();
            CircleCollider2D collider = prefab.AddComponent<CircleCollider2D>();
            collider.radius = immune ? 0.42f : 0.34f;
            Rigidbody2D body = prefab.AddComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.linearDamping = 2f;

            if (immune) prefab.AddComponent<ImmuneCell>();
            else prefab.AddComponent<Enemy>();

            DontDestroyOnLoad(prefab);
            return prefab;
        }

        private void CreateRuntimeNode(string name, DiseaseType disease, Vector2 position)
        {
            GameObject node = new GameObject(name);
            node.transform.position = position;
            InfectionNode infection = node.AddComponent<InfectionNode>();
            infection.SetDiseaseType(disease);

            SpriteRenderer renderer = node.AddComponent<SpriteRenderer>();
            renderer.sprite = CreateRuntimeCircleSprite(32, new Color(1f, 0.08f, 0.08f, 0.65f));
            renderer.sortingOrder = -20;
        }

        private Sprite CreateRuntimeCircleSprite(int size, Color color)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            Color[] pixels = new Color[size * size];
            Vector2 center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            float radius = size * 0.45f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float d = Vector2.Distance(new Vector2(x, y), center);
                    float a = 1f - Mathf.SmoothStep(radius * 0.45f, radius, d);
                    pixels[y * size + x] = new Color(color.r, color.g, color.b, color.a * a);
                }
            }

            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
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
            if (allowBattleGameOver)
            {
                CheckBattleEnd();
            }
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

            if (!useObjectiveDrivenMode)
            {
                GameManager.Instance?.EndBattle(victory);
            }
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

        public ImmuneCell SpawnSquadImmuneCell(ImmuneSquadRole role, Vector2 position)
        {
            Vector2 spawnPos = GetSafeSpawnPosition(position);
            GameObject obj = Instantiate(immuneCellPrefab, spawnPos, Quaternion.identity);
            obj.SetActive(true);
            ImmuneCell cell = obj.GetComponent<ImmuneCell>();

            if (cell != null)
            {
                cell.ConfigureSquadRole(role);
                cell.SetAutonomousCombat(false);
                CardBuffManager.Instance?.ApplyBuffsToUnit(cell);
                cell.OnDeath += OnImmuneCellDeath;
                immuneCells.Add(cell);

                EventManager.TriggerEvent(GameEvents.UNIT_SPAWNED, cell);

                if (debugMode) Debug.Log($"[BattleManager] Spawned squad {role}");
            }

            return cell;
        }

        public ImmuneCell SpawnImmuneCell(ImmuneCellType type, Vector2? position = null)
        {
            Vector2 spawnPos = GetSafeSpawnPosition(position ?? GetRandomSpawnPosition(immuneSpawnArea));

            GameObject obj = Instantiate(immuneCellPrefab, spawnPos, Quaternion.identity);
            obj.SetActive(true);
            ImmuneCell cell = obj.GetComponent<ImmuneCell>();

            if (cell != null)
            {
                cell.SetCellType(type);
                CardBuffManager.Instance?.ApplyBuffsToUnit(cell);
                cell.OnDeath += OnImmuneCellDeath;
                immuneCells.Add(cell);

                EventManager.TriggerEvent(GameEvents.UNIT_SPAWNED, cell);

                if (debugMode) Debug.Log($"[BattleManager] Spawned {type}");
            }

            return cell;
        }

        public Enemy SpawnEnemy(EnemyType type, DiseaseType disease, Vector2? position = null)
        {
            Vector2 spawnPos = GetSafeSpawnPosition(position ?? GetRandomSpawnPosition(enemySpawnArea));

            GameObject obj = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            obj.SetActive(true);
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

        private Vector2 GetSafeSpawnPosition(Vector2 position)
        {
            if (ArenaWalkableMask.Instance != null)
            {
                return ArenaWalkableMask.Instance.GetNearestWalkablePosition(position, 10f);
            }

            return position;
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
            if (debugMode) Debug.Log("[BattleManager] Immune cell died");
        }

        private void OnEnemyDeath(Unit unit)
        {
            if (debugMode) Debug.Log("[BattleManager] Enemy died");
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

            ImmuneSquadController squadController = FindFirstObjectByType<ImmuneSquadController>(FindObjectsInactive.Include);
            squadController?.ClearSquad();
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
