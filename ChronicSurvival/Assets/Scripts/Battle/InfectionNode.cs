using UnityEngine;
using System.Collections;
using ChronicSurvival.Core;
using ChronicSurvival.Units;

namespace ChronicSurvival.Battle
{
    public class InfectionNode : MonoBehaviour
    {
        [Header("Node Settings")]
        [SerializeField] private DiseaseType diseaseType;
        [SerializeField] private float spawnInterval = 3f;
        [SerializeField] private int maxSpawns = 10;
        [SerializeField] private float spawnRadius = 2f;

        [Header("Spawn Weights")]
        [SerializeField] private float basicWeight = 0.7f;
        [SerializeField] private float mediumWeight = 0.25f;
        [SerializeField] private float eliteWeight = 0.05f;

        [Header("Progression")]
        [SerializeField] private bool scaleWithRound = true;
        [SerializeField] private float difficultyMultiplier = 1.1f;

        [Header("Visual")]
        [SerializeField] private SpriteRenderer nodeVisual;
        [SerializeField] private Color nodeColor = Color.red;

        private int spawnCount = 0;
        private bool isActive = false;
        private Coroutine spawnCoroutine;

        public DiseaseType DiseaseType => diseaseType;
        public bool IsActive => isActive;
        public int SpawnCount => spawnCount;

        private void Start()
        {
            if (nodeVisual != null)
            {
                nodeVisual.color = nodeColor;
            }
        }

        public void ActivateNode()
        {
            if (isActive) return;

            isActive = true;
            spawnCount = 0;

            if (spawnCoroutine != null)
                StopCoroutine(spawnCoroutine);

            spawnCoroutine = StartCoroutine(SpawnRoutine());

            EventManager.TriggerEvent(GameEvents.INFECTION_NODE_ACTIVATED, this);
        }

        public void DeactivateNode()
        {
            isActive = false;

            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
        }

        private IEnumerator SpawnRoutine()
        {
            while (isActive && spawnCount < maxSpawns)
            {
                SpawnEnemy();
                spawnCount++;

                yield return new WaitForSeconds(spawnInterval);
            }

            if (spawnCount >= maxSpawns)
            {
                DeactivateNode();
            }
        }

        private void SpawnEnemy()
        {
            if (BattleManager.Instance == null) return;

            EnemyType enemyType = GetRandomEnemyType();
            Vector2 spawnPos = GetRandomSpawnPosition();

            BattleManager.Instance.SpawnEnemy(enemyType, diseaseType, spawnPos);
        }

        private EnemyType GetRandomEnemyType()
        {
            float totalWeight = basicWeight + mediumWeight + eliteWeight;
            float random = Random.Range(0f, totalWeight);

            if (random < basicWeight)
                return EnemyType.Basic;
            else if (random < basicWeight + mediumWeight)
                return EnemyType.Medium;
            else
                return EnemyType.Elite;
        }

        private Vector2 GetRandomSpawnPosition()
        {
            Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
            return (Vector2)transform.position + randomOffset;
        }

        public void SetDiseaseType(DiseaseType type)
        {
            diseaseType = type;
        }

        public void SetSpawnRate(float interval)
        {
            spawnInterval = interval;
        }

        public void SetMaxSpawns(int max)
        {
            maxSpawns = max;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = isActive ? Color.red : new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawWireSphere(transform.position, spawnRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }
    }
}
