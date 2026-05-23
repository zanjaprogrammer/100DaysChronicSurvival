using System.Collections.Generic;
using UnityEngine;
using ChronicSurvival.ProceduralVisuals;

namespace ChronicSurvival.Arena
{
    public class BloodFlowSimulator : MonoBehaviour
    {
        [SerializeField] private float cellsPerArea = 0.34f;
        [SerializeField] private float spawnInterval = 0.03f;
        [SerializeField] private float minCellSize = 0.62f;
        [SerializeField] private float maxCellSize = 0.84f;
        [SerializeField] private float minFlowSpeed = 2.4f;
        [SerializeField] private float maxFlowSpeed = 4.0f;

        private readonly List<RedBloodCell> cells = new List<RedBloodCell>();
        private Bounds arenaBounds;
        private float vesselHalfWidth;
        private float spawnTimer;
        private int targetCount;
        private Camera targetCamera;

        public void Configure(Bounds bounds, float halfWidth, int rendererSortingOrder, Camera cameraRef)
        {
            arenaBounds = bounds;
            vesselHalfWidth = halfWidth;
            targetCamera = cameraRef != null ? cameraRef : (Camera.main != null ? Camera.main : FindFirstObjectByType<Camera>(FindObjectsInactive.Include));
            targetCount = Mathf.Clamp(Mathf.RoundToInt(bounds.size.x * halfWidth * 2f * cellsPerArea), 60, 220);
            spawnTimer = Random.Range(0f, spawnInterval);
            SeedInitialCells();
        }

        private void Update()
        {
            cells.RemoveAll(cell => cell == null);
            spawnTimer += Time.deltaTime;

            while (cells.Count < targetCount && spawnTimer >= spawnInterval)
            {
                spawnTimer -= spawnInterval;
                SpawnCell(false);
            }
        }

        private void SeedInitialCells()
        {
            int initialCount = Mathf.RoundToInt(targetCount * 0.7f);
            if (initialCount <= 0) return;

            for (int i = 0; i < initialCount; i++)
            {
                float t = (i + Random.Range(0.12f, 0.88f)) / initialCount;
                SpawnCell(StratifiedFlowPosition(t));
            }
        }

        private void SpawnCell(bool anywhereInFlow)
        {
            Vector2 position = anywhereInFlow ? RandomFlowPosition() : LeftSpawnPosition();
            SpawnCell(position);
        }

        private void SpawnCell(Vector2 position)
        {
            if (ArenaWalkableMask.Instance != null)
            {
                position = ArenaWalkableMask.Instance.GetNearestWalkablePosition(position, vesselHalfWidth * 0.45f);
            }

            GameObject cellObject = new GameObject("RedBloodCell", typeof(CircleCollider2D), typeof(Rigidbody2D), typeof(RedBloodCellBlob), typeof(RedBloodCell));
            cellObject.transform.SetParent(transform, false);
            cellObject.transform.position = new Vector3(position.x, position.y, 0f);

            float size = Random.Range(minCellSize, maxCellSize);
            cellObject.transform.localScale = Vector3.one;
            cellObject.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-18f, 18f));

            CircleCollider2D collider = cellObject.GetComponent<CircleCollider2D>();
            collider.radius = size * 0.52f;

            Rigidbody2D body = cellObject.GetComponent<Rigidbody2D>();
            body.gravityScale = 0f;
            body.linearDamping = 0.12f;
            body.angularDamping = 1.0f;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
            body.mass = 0.65f;
            body.freezeRotation = true;
            body.linearVelocity = new Vector2(Random.Range(minFlowSpeed, maxFlowSpeed), Random.Range(-0.25f, 0.25f));

            PhysicsMaterial2D physicsMaterial = new PhysicsMaterial2D("RedBloodCellMaterial")
            {
                friction = 0.04f,
                bounciness = 0.08f
            };
            collider.sharedMaterial = physicsMaterial;

            RedBloodCellBlob blob = cellObject.GetComponent<RedBloodCellBlob>();
            blob.Configure(size, new Color(Random.Range(0.78f, 0.96f), Random.Range(0.05f, 0.14f), Random.Range(0.05f, 0.12f), 1f));

            RedBloodCell cell = cellObject.GetComponent<RedBloodCell>();
            float laneCenter = Mathf.Clamp(position.y + Random.Range(-0.45f, 0.45f), -vesselHalfWidth * 0.8f, vesselHalfWidth * 0.8f);
            cell.Configure(Random.Range(minFlowSpeed, maxFlowSpeed), Random.Range(0.12f, 0.28f), arenaBounds.max.x + 2.5f, targetCamera, laneCenter);
            cells.Add(cell);
        }

        private Vector2 LeftSpawnPosition()
        {
            float leftX = arenaBounds.min.x + 1.5f;
            return new Vector2(leftX, Random.Range(-vesselHalfWidth * 0.88f, vesselHalfWidth * 0.88f));
        }

        private Vector2 RandomFlowPosition()
        {
            float minX = arenaBounds.min.x + 1.5f;
            float maxX = arenaBounds.max.x - 1.5f;
            return new Vector2(Random.Range(minX, maxX), Random.Range(-vesselHalfWidth * 0.9f, vesselHalfWidth * 0.9f));
        }

        private Vector2 StratifiedFlowPosition(float t)
        {
            float minX = arenaBounds.min.x + 1.5f;
            float maxX = arenaBounds.max.x - 1.5f;
            float x = Mathf.Lerp(minX, maxX, Mathf.Clamp01(t));
            float y = Random.Range(-vesselHalfWidth * 0.86f, vesselHalfWidth * 0.86f);
            return new Vector2(x, y);
        }
    }
}
