using UnityEngine;

namespace ChronicSurvival.ProceduralVisuals
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class RedBloodCellBlob : MonoBehaviour
    {
        [SerializeField] private int segments = 24;
        [SerializeField] private float radius = 0.48f;
        [SerializeField] private float rimIndent = 0.12f;
        [SerializeField] private float wobbleAmount = 0.035f;
        [SerializeField] private float wobbleSpeed = 1.6f;
        [SerializeField] private Color fillColor = new Color(0.86f, 0.08f, 0.08f, 1f);
        [SerializeField] private Color shadeColor = new Color(0.58f, 0.02f, 0.02f, 1f);

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private Mesh mesh;
        private Vector3[] baseVertices;
        private Color[] colors;
        private float timeOffset;

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
            timeOffset = Random.Range(0f, 100f);
            BuildMesh();
            SetupMaterial();
        }

        public void Configure(float cellRadius, Color color)
        {
            radius = cellRadius;
            fillColor = color;
            shadeColor = Color.Lerp(color, new Color(0.25f, 0f, 0f, 1f), 0.38f);
            BuildMesh();
            SetupMaterial();
        }

        private void SetupMaterial()
        {
            if (meshRenderer.sharedMaterial == null)
            {
                Shader shader = Shader.Find("Sprites/Default");
                if (shader != null)
                {
                    meshRenderer.sharedMaterial = new Material(shader);
                }
            }
        }

        private void BuildMesh()
        {
            if (mesh == null)
            {
                mesh = new Mesh { name = "RedBloodCellBlob" };
                meshFilter.mesh = mesh;
            }
            else
            {
                mesh.Clear();
            }

            Vector3[] vertices = new Vector3[segments + 1];
            Vector2[] uv = new Vector2[segments + 1];
            colors = new Color[segments + 1];
            int[] triangles = new int[segments * 3];

            vertices[0] = Vector3.zero;
            uv[0] = new Vector2(0.5f, 0.5f);
            colors[0] = Color.Lerp(fillColor, Color.white, 0.12f);

            for (int i = 0; i < segments; i++)
            {
                float t = (float)i / segments;
                float angle = t * Mathf.PI * 2f;
                float horizontalFlatten = 1.18f;
                float verticalFlatten = 0.82f;
                float dimple = 1f - Mathf.Pow(Mathf.Sin(angle), 2f) * rimIndent;
                float edgeNoise = Mathf.PerlinNoise(Mathf.Cos(angle) * 1.7f + timeOffset, Mathf.Sin(angle) * 1.7f + timeOffset);
                float edgeRadius = radius * dimple * (0.94f + (edgeNoise - 0.5f) * 0.08f);

                vertices[i + 1] = new Vector3(
                    Mathf.Cos(angle) * edgeRadius * horizontalFlatten,
                    Mathf.Sin(angle) * edgeRadius * verticalFlatten,
                    0f
                );
                uv[i + 1] = new Vector2(Mathf.Cos(angle) * 0.5f + 0.5f, Mathf.Sin(angle) * 0.5f + 0.5f);

                float highlight = Mathf.Clamp01(Vector2.Dot(new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)), new Vector2(-0.65f, 0.75f)) * 0.5f + 0.5f);
                colors[i + 1] = Color.Lerp(shadeColor, fillColor, 0.5f + highlight * 0.5f);
            }

            for (int i = 0; i < segments; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = (i + 1) % segments + 1;
            }

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.colors = colors;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            baseVertices = (Vector3[])vertices.Clone();
        }

        private void Update()
        {
            if (mesh == null || baseVertices == null) return;

            Vector3[] vertices = mesh.vertices;
            float time = Time.time * wobbleSpeed + timeOffset;
            for (int i = 1; i < vertices.Length; i++)
            {
                float angle = Mathf.Atan2(baseVertices[i].y, baseVertices[i].x);
                float wobble = Mathf.Sin(time + angle * 2.5f) * wobbleAmount;
                vertices[i] = baseVertices[i] * (1f + wobble);
            }

            mesh.vertices = vertices;
            mesh.RecalculateBounds();
        }
    }
}
