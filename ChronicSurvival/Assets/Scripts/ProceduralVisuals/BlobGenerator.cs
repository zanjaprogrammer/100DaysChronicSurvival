using UnityEngine;

namespace ChronicSurvival.ProceduralVisuals
{
    /// <summary>
    /// Generates procedural blob shapes for cells with neon glow effects
    /// Inspired by slime/blob dynamics for organic feel
    /// </summary>
    public class BlobGenerator : MonoBehaviour
    {
        [Header("Blob Shape Settings")]
        [SerializeField] private int segments = 32; // Number of vertices in blob
        [SerializeField] private float baseRadius = 0.5f;
        [SerializeField] private float radiusVariation = 0.15f; // How much the radius varies
        [SerializeField] private float noiseScale = 2f;
        [SerializeField] private float noiseSpeed = 1f;

        [Header("Neon Glow Settings")]
        [SerializeField] private Color coreColor = new Color(0.2f, 0.5f, 1f, 1f); // Blue
        [SerializeField] private Color glowColor = new Color(0.4f, 0.7f, 1f, 0.5f); // Light blue glow
        [SerializeField] private float glowIntensity = 2f;
        [SerializeField] private float glowSize = 1.3f;

        [Header("Animation Settings")]
        [SerializeField] private bool animate = true;
        [SerializeField] private float pulseSpeed = 1f;
        [SerializeField] private float pulseAmount = 0.1f;
        [SerializeField] private float wobbleSpeed = 0.5f;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private Material coreMaterial;
        private Material glowMaterial;
        private float timeOffset;
        private Vector3[] baseVertices;

        private void Awake()
        {
            SetupComponents();
            GenerateBlob();
            SetupMaterials();
        }

        private void SetupComponents()
        {
            // Setup mesh filter
            meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter == null)
                meshFilter = gameObject.AddComponent<MeshFilter>();

            // Setup mesh renderer
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
                meshRenderer = gameObject.AddComponent<MeshRenderer>();

            // Random time offset for variation
            timeOffset = Random.Range(0f, 100f);
        }

        private void GenerateBlob()
        {
            Mesh mesh = new Mesh();
            mesh.name = "ProceduralBlob";

            // Generate vertices in a circle with noise
            Vector3[] vertices = new Vector3[segments + 1]; // +1 for center
            Vector2[] uv = new Vector2[segments + 1];
            int[] triangles = new int[segments * 3];

            // Center vertex
            vertices[0] = Vector3.zero;
            uv[0] = new Vector2(0.5f, 0.5f);

            // Outer vertices with perlin noise variation
            for (int i = 0; i < segments; i++)
            {
                float angle = (float)i / segments * Mathf.PI * 2f;
                
                // Add perlin noise for organic shape
                float noiseValue = Mathf.PerlinNoise(
                    Mathf.Cos(angle) * noiseScale + timeOffset,
                    Mathf.Sin(angle) * noiseScale + timeOffset
                );
                
                float radius = baseRadius + (noiseValue - 0.5f) * radiusVariation;
                
                vertices[i + 1] = new Vector3(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius,
                    0f
                );

                uv[i + 1] = new Vector2(
                    Mathf.Cos(angle) * 0.5f + 0.5f,
                    Mathf.Sin(angle) * 0.5f + 0.5f
                );
            }

            // Generate triangles
            for (int i = 0; i < segments; i++)
            {
                triangles[i * 3] = 0; // Center
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = (i + 1) % segments + 1;
            }

            mesh.vertices = vertices;
            mesh.uv = uv;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            meshFilter.mesh = mesh;
            baseVertices = (Vector3[])vertices.Clone();
        }

        private void SetupMaterials()
        {
            // Create core material (solid blob)
            coreMaterial = new Material(Shader.Find("Sprites/Default"));
            coreMaterial.color = coreColor;

            // Create glow material (outer glow)
            glowMaterial = new Material(Shader.Find("Sprites/Default"));
            glowMaterial.color = glowColor;

            // Apply materials
            meshRenderer.materials = new Material[] { coreMaterial };

            // TODO: Add glow as separate GameObject with larger scale
            CreateGlowEffect();
        }

        private void CreateGlowEffect()
        {
            // Create child object for glow
            GameObject glowObj = new GameObject("Glow");
            glowObj.transform.SetParent(transform);
            glowObj.transform.localPosition = Vector3.zero;
            glowObj.transform.localScale = Vector3.one * glowSize;
            glowObj.transform.localRotation = Quaternion.identity;

            // Copy mesh
            MeshFilter glowMeshFilter = glowObj.AddComponent<MeshFilter>();
            glowMeshFilter.mesh = meshFilter.mesh;

            // Setup glow renderer
            MeshRenderer glowRenderer = glowObj.AddComponent<MeshRenderer>();
            glowRenderer.material = glowMaterial;
            glowRenderer.sortingOrder = -1; // Behind core

            // Make glow slightly transparent
            Color glowCol = glowColor;
            glowCol.a = 0.3f;
            glowMaterial.color = glowCol;
        }

        private void Update()
        {
            if (animate)
            {
                AnimateBlob();
            }
        }

        private void AnimateBlob()
        {
            Mesh mesh = meshFilter.mesh;
            Vector3[] vertices = mesh.vertices;

            float time = Time.time * noiseSpeed + timeOffset;

            // Pulse effect (breathing)
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;

            // Update vertices with animated noise
            for (int i = 1; i < vertices.Length; i++)
            {
                float angle = Mathf.Atan2(baseVertices[i].y, baseVertices[i].x);
                
                // Animated perlin noise
                float noiseValue = Mathf.PerlinNoise(
                    Mathf.Cos(angle) * noiseScale + time,
                    Mathf.Sin(angle) * noiseScale + time
                );

                // Wobble effect
                float wobble = Mathf.Sin(time * wobbleSpeed + angle * 3f) * 0.05f;
                
                float radius = (baseRadius + (noiseValue - 0.5f) * radiusVariation + wobble) * pulse;
                
                vertices[i] = new Vector3(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius,
                    0f
                );
            }

            mesh.vertices = vertices;
            mesh.RecalculateBounds();

            // Animate glow intensity
            float glowPulse = 1f + Mathf.Sin(Time.time * pulseSpeed * 2f) * 0.3f;
            Color currentGlow = glowColor;
            currentGlow.a = glowColor.a * glowPulse;
            glowMaterial.color = currentGlow;
        }

        // Public methods to customize blob
        public void SetColors(Color core, Color glow)
        {
            coreColor = core;
            glowColor = glow;
            if (coreMaterial != null) coreMaterial.color = core;
            if (glowMaterial != null) glowMaterial.color = glow;
        }

        public void SetSize(float size)
        {
            baseRadius = size;
            GenerateBlob();
        }

        public void SetAnimationSpeed(float speed)
        {
            noiseSpeed = speed;
            pulseSpeed = speed;
            wobbleSpeed = speed * 0.5f;
        }

        // Preset configurations for different cell types
        public void ApplyPreset(BlobPreset preset)
        {
            switch (preset)
            {
                case BlobPreset.Macrophage:
                    SetColors(
                        new Color(0.18f, 0.37f, 0.67f, 1f), // Deep blue
                        new Color(0.29f, 0.56f, 0.89f, 0.5f)  // Light blue glow
                    );
                    SetSize(0.6f);
                    SetAnimationSpeed(0.5f);
                    break;

                case BlobPreset.TCell:
                    SetColors(
                        new Color(0.29f, 0.56f, 0.89f, 1f), // Bright blue
                        new Color(0.48f, 0.73f, 0.95f, 0.5f)  // Lighter glow
                    );
                    SetSize(0.45f);
                    SetAnimationSpeed(1.2f);
                    break;

                case BlobPreset.BCell:
                    SetColors(
                        new Color(0.48f, 0.73f, 0.91f, 1f), // Light blue
                        new Color(0.67f, 0.85f, 0.98f, 0.5f)  // Very light glow
                    );
                    SetSize(0.4f);
                    SetAnimationSpeed(0.8f);
                    break;

                case BlobPreset.NKCell:
                    SetColors(
                        new Color(0.42f, 0.35f, 0.58f, 1f), // Blue-purple
                        new Color(0.62f, 0.55f, 0.78f, 0.5f)  // Purple glow
                    );
                    SetSize(0.5f);
                    SetAnimationSpeed(1.5f);
                    break;

                case BlobPreset.Neutrophil:
                    SetColors(
                        new Color(0f, 0.81f, 0.82f, 1f), // Cyan
                        new Color(0.4f, 0.91f, 0.92f, 0.5f)  // Light cyan glow
                    );
                    SetSize(0.35f);
                    SetAnimationSpeed(2f);
                    break;

                case BlobPreset.SugarBlob:
                    SetColors(
                        new Color(1f, 0.7f, 0.28f, 0.8f), // Golden amber
                        new Color(1f, 0.84f, 0.47f, 0.4f)  // Light gold glow
                    );
                    SetSize(0.3f);
                    SetAnimationSpeed(0.6f);
                    break;

                case BlobPreset.PressurePulse:
                    SetColors(
                        new Color(1f, 0f, 0f, 1f), // Bright red
                        new Color(1f, 0.41f, 0.41f, 0.6f)  // Pink glow
                    );
                    SetSize(0.35f);
                    SetAnimationSpeed(2.5f);
                    break;

                case BlobPreset.DamagedCell:
                    SetColors(
                        new Color(0.58f, 0.44f, 0.86f, 0.9f), // Light purple
                        new Color(0.78f, 0.64f, 0.96f, 0.4f)  // Purple glow
                    );
                    SetSize(0.35f);
                    SetAnimationSpeed(1.8f);
                    break;
            }
        }
    }

    public enum BlobPreset
    {
        Macrophage,
        TCell,
        BCell,
        NKCell,
        Neutrophil,
        SugarBlob,
        PressurePulse,
        DamagedCell
    }
}
