using UnityEngine;
using ChronicSurvival.Battle;
using ChronicSurvival.Units;
using ChronicSurvival.Core;

namespace ChronicSurvival.Test
{
    public class BattleTest : MonoBehaviour
    {
        [Header("Test Settings")]
        [SerializeField] private bool enableTestControls = true;
        [SerializeField] private KeyCode startBattleKey = KeyCode.Space;
        [SerializeField] private KeyCode endBattleKey = KeyCode.Escape;
        [SerializeField] private KeyCode spawnImmuneCellKey = KeyCode.I;
        [SerializeField] private KeyCode spawnEnemyKey = KeyCode.E;
        [SerializeField] private KeyCode clearAllKey = KeyCode.C;

        [Header("Spawn Test Settings")]
        [SerializeField] private ImmuneCellType testImmuneCellType = ImmuneCellType.Macrophage;
        [SerializeField] private EnemyType testEnemyType = EnemyType.Basic;
        [SerializeField] private DiseaseType testDiseaseType = DiseaseType.Diabetes;

        [Header("References")]
        [SerializeField] private BattleManager battleManager;
        [SerializeField] private UnitSpawner unitSpawner;
        [SerializeField] private InfectionNode[] infectionNodes;

        private void Start()
        {
            if (battleManager == null)
                battleManager = FindAnyObjectByType<BattleManager>();

            if (unitSpawner == null)
                unitSpawner = FindAnyObjectByType<UnitSpawner>();

            if (infectionNodes == null || infectionNodes.Length == 0)
                infectionNodes = FindObjectsByType<InfectionNode>(FindObjectsSortMode.None);

            Debug.Log("[BattleTest] Battle Test initialized");
            Debug.Log($"[BattleTest] Controls:");
            Debug.Log($"  {startBattleKey} - Start Battle");
            Debug.Log($"  {endBattleKey} - End Battle");
            Debug.Log($"  {spawnImmuneCellKey} - Spawn Immune Cell");
            Debug.Log($"  {spawnEnemyKey} - Spawn Enemy");
            Debug.Log($"  {clearAllKey} - Clear All Units");
            Debug.Log($"  1-5 - Change Immune Cell Type");
            Debug.Log($"  6-8 - Change Disease Type");
            Debug.Log($"  Q/W/E - Change Enemy Type");
        }

        private void Update()
        {
            if (!enableTestControls) return;

            // Battle Controls
            if (Input.GetKeyDown(startBattleKey))
            {
                StartTestBattle();
            }

            if (Input.GetKeyDown(endBattleKey))
            {
                EndTestBattle();
            }

            // Spawn Controls
            if (Input.GetKeyDown(spawnImmuneCellKey))
            {
                SpawnTestImmuneCell();
            }

            if (Input.GetKeyDown(spawnEnemyKey))
            {
                SpawnTestEnemy();
            }

            if (Input.GetKeyDown(clearAllKey))
            {
                ClearAllUnits();
            }

            // Type Selection
            HandleTypeSelection();
        }

        private void HandleTypeSelection()
        {
            // Immune Cell Type Selection
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                testImmuneCellType = ImmuneCellType.Macrophage;
                Debug.Log($"[BattleTest] Selected: {testImmuneCellType}");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                testImmuneCellType = ImmuneCellType.TCell;
                Debug.Log($"[BattleTest] Selected: {testImmuneCellType}");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                testImmuneCellType = ImmuneCellType.BCell;
                Debug.Log($"[BattleTest] Selected: {testImmuneCellType}");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                testImmuneCellType = ImmuneCellType.NKCell;
                Debug.Log($"[BattleTest] Selected: {testImmuneCellType}");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                testImmuneCellType = ImmuneCellType.Neutrophil;
                Debug.Log($"[BattleTest] Selected: {testImmuneCellType}");
            }

            // Disease Type Selection
            if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                testDiseaseType = DiseaseType.Diabetes;
                Debug.Log($"[BattleTest] Selected: {testDiseaseType}");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                testDiseaseType = DiseaseType.Hypertension;
                Debug.Log($"[BattleTest] Selected: {testDiseaseType}");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                testDiseaseType = DiseaseType.Cancer;
                Debug.Log($"[BattleTest] Selected: {testDiseaseType}");
            }

            // Enemy Type Selection
            if (Input.GetKeyDown(KeyCode.Q))
            {
                testEnemyType = EnemyType.Basic;
                Debug.Log($"[BattleTest] Selected: {testEnemyType}");
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                testEnemyType = EnemyType.Medium;
                Debug.Log($"[BattleTest] Selected: {testEnemyType}");
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                testEnemyType = EnemyType.Elite;
                Debug.Log($"[BattleTest] Selected: {testEnemyType}");
            }
        }

        private void StartTestBattle()
        {
            if (battleManager == null)
            {
                Debug.LogError("[BattleTest] BattleManager not found!");
                return;
            }

            Debug.Log("[BattleTest] Starting battle...");

            // Spawn initial units if spawner exists
            if (unitSpawner != null)
            {
                unitSpawner.SpawnAllUnits();
            }

            // Activate infection nodes
            if (infectionNodes != null && infectionNodes.Length > 0)
            {
                foreach (var node in infectionNodes)
                {
                    if (node != null)
                    {
                        node.ActivateNode();
                    }
                }
            }

            battleManager.StartBattle();
        }

        private void EndTestBattle()
        {
            if (battleManager == null) return;

            Debug.Log("[BattleTest] Ending battle...");
            battleManager.EndBattle(true);

            // Deactivate infection nodes
            if (infectionNodes != null && infectionNodes.Length > 0)
            {
                foreach (var node in infectionNodes)
                {
                    if (node != null)
                    {
                        node.DeactivateNode();
                    }
                }
            }
        }

        private void SpawnTestImmuneCell()
        {
            if (battleManager == null) return;

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            battleManager.SpawnImmuneCell(testImmuneCellType, mousePos);

            Debug.Log($"[BattleTest] Spawned {testImmuneCellType} at {mousePos}");
        }

        private void SpawnTestEnemy()
        {
            if (battleManager == null) return;

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            battleManager.SpawnEnemy(testEnemyType, testDiseaseType, mousePos);

            Debug.Log($"[BattleTest] Spawned {testDiseaseType} {testEnemyType} at {mousePos}");
        }

        private void ClearAllUnits()
        {
            if (battleManager == null) return;

            battleManager.ClearAllUnits();
            Debug.Log("[BattleTest] Cleared all units");
        }

        private void OnGUI()
        {
            if (!enableTestControls) return;

            GUILayout.BeginArea(new Rect(10, 10, 300, 400));
            GUILayout.Box("=== BATTLE TEST ===");

            if (battleManager != null)
            {
                GUILayout.Label($"Battle Active: {battleManager.BattleActive}");
                GUILayout.Label($"Timer: {battleManager.BattleTimer:F1}s");
                GUILayout.Label($"Immune Cells: {battleManager.ImmuneCount}");
                GUILayout.Label($"Enemies: {battleManager.EnemyCount}");
            }

            GUILayout.Space(10);
            GUILayout.Label($"Selected Immune: {testImmuneCellType}");
            GUILayout.Label($"Selected Disease: {testDiseaseType}");
            GUILayout.Label($"Selected Enemy: {testEnemyType}");

            GUILayout.EndArea();
        }
    }
}
