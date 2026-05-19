using UnityEngine;
using ChronicSurvival.ProceduralVisuals;

namespace ChronicSurvival.Test
{
    public class BlobTest : MonoBehaviour
    {
        [Header("Test Settings")]
        [SerializeField] private bool spawnOnStart = true;
        [SerializeField] private int numberOfBlobs = 5;
        [SerializeField] private float spawnRadius = 3f;

        [Header("Blob Presets to Test")]
        [SerializeField] private bool testMacrophage = true;
        [SerializeField] private bool testTCell = true;
        [SerializeField] private bool testBCell = true;
        [SerializeField] private bool testNKCell = true;
        [SerializeField] private bool testNeutrophil = true;
        [SerializeField] private bool testSugarBlob = true;
        [SerializeField] private bool testPressurePulse = true;
        [SerializeField] private bool testDamagedCell = true;

        [Header("Movement Test")]
        [SerializeField] private bool enableMovement = true;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float changeDirectionInterval = 2f;

        private void Start()
        {
            if (spawnOnStart)
            {
                SpawnTestBlobs();
            }
        }

        private void Update()
        {
            // Keyboard shortcuts
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnTestBlobs();
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                ClearAllBlobs();
            }
        }

        [ContextMenu("Spawn Test Blobs")]
        public void SpawnTestBlobs()
        {
            ClearAllBlobs();

            int index = 0;

            if (testMacrophage) SpawnBlob(BlobPreset.Macrophage, index++);
            if (testTCell) SpawnBlob(BlobPreset.TCell, index++);
            if (testBCell) SpawnBlob(BlobPreset.BCell, index++);
            if (testNKCell) SpawnBlob(BlobPreset.NKCell, index++);
            if (testNeutrophil) SpawnBlob(BlobPreset.Neutrophil, index++);
            if (testSugarBlob) SpawnBlob(BlobPreset.SugarBlob, index++);
            if (testPressurePulse) SpawnBlob(BlobPreset.PressurePulse, index++);
            if (testDamagedCell) SpawnBlob(BlobPreset.DamagedCell, index++);

            Debug.Log($"[BlobTest] Spawned {index} test blobs");
        }

        private void SpawnBlob(BlobPreset preset, int index)
        {
            float angle = (index / 8f) * Mathf.PI * 2f;
            Vector3 position = new Vector3(
                Mathf.Cos(angle) * spawnRadius,
                Mathf.Sin(angle) * spawnRadius,
                0f
            );

            GameObject blobObj = new GameObject($"TestBlob_{preset}");
            blobObj.transform.position = position;
            blobObj.transform.SetParent(transform);

            BlobGenerator blob = blobObj.AddComponent<BlobGenerator>();
            blob.ApplyPreset(preset);

            SlimeBlobPhysics physics = blobObj.AddComponent<SlimeBlobPhysics>();
            BlobTrailEffect trail = blobObj.AddComponent<BlobTrailEffect>();

            if (enableMovement)
            {
                BlobMover mover = blobObj.AddComponent<BlobMover>();
                mover.moveSpeed = moveSpeed;
                mover.changeDirectionInterval = changeDirectionInterval;
            }
        }

        [ContextMenu("Clear All Blobs")]
        public void ClearAllBlobs()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public class BlobMover : MonoBehaviour
    {
        public float moveSpeed = 2f;
        public float changeDirectionInterval = 2f;

        private Vector2 moveDirection;
        private float timer;

        private void Start()
        {
            ChangeDirection();
        }

        private void Update()
        {
            transform.position += (Vector3)moveDirection * moveSpeed * Time.deltaTime;

            timer += Time.deltaTime;
            if (timer >= changeDirectionInterval)
            {
                ChangeDirection();
                timer = 0f;
            }

            Vector3 pos = transform.position;
            if (Mathf.Abs(pos.x) > 10f || Mathf.Abs(pos.y) > 10f)
            {
                moveDirection = -moveDirection;
            }
        }

        private void ChangeDirection()
        {
            moveDirection = Random.insideUnitCircle.normalized;
        }
    }
}
