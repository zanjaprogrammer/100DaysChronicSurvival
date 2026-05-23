using UnityEngine;
using ChronicSurvival.Core;

namespace ChronicSurvival.Arena
{
    [DefaultExecutionOrder(-500)]
    public class ProceduralBloodVesselArena : MonoBehaviour
    {
        [Header("Arena")]
        [SerializeField] private Vector2 arenaSize = new Vector2(58f, 22f);
        [SerializeField] private int textureWidth = 1400;
        [SerializeField] private int textureHeight = 540;
        [SerializeField] private float vesselHalfWidth = 4.6f;
        [SerializeField] private float edgeFeather = 1.15f;
        [SerializeField] private int sortingOrder = -120;

        private static readonly Vector2[] pathPoints =
        {
            new Vector2(-30f, -1.1f),
            new Vector2(-21f, -0.2f),
            new Vector2(-12f, 0.55f),
            new Vector2(-3f, -0.25f),
            new Vector2(7f, 0.4f),
            new Vector2(18f, -0.1f),
            new Vector2(30f, 0.75f)
        };

        private SpriteRenderer spriteRenderer;
        private Material runtimeMaterial;
        private Texture2D arenaTexture;
        private Texture2D maskTexture;
        private Bounds arenaBounds;
        private Camera runtimeCamera;

        public Bounds ArenaBounds => arenaBounds;
        public Camera RuntimeCamera => runtimeCamera;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureArena()
        {
            if (FindFirstObjectByType<ProceduralBloodVesselArena>(FindObjectsInactive.Include) != null) return;

            GameObject go = new GameObject("ProceduralBloodVesselArena");
            go.AddComponent<ProceduralBloodVesselArena>();
        }

        private void Awake()
        {
            BuildArena();
        }

        private void BuildArena()
        {
            arenaBounds = new Bounds(Vector3.zero, new Vector3(arenaSize.x, arenaSize.y, 0f));
            CreateTextures();
            SetupRenderer();
            BuildVesselLines();
            SetupMask();
            SetupCamera();
            BuildBloodFlow();
            EnsurePathfinder();
        }

        private void CreateTextures()
        {
            arenaTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
            arenaTexture.wrapMode = TextureWrapMode.Clamp;
            arenaTexture.filterMode = FilterMode.Bilinear;

            maskTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
            maskTexture.wrapMode = TextureWrapMode.Clamp;
            maskTexture.filterMode = FilterMode.Point;

            Color[] colors = new Color[textureWidth * textureHeight];
            Color[] mask = new Color[textureWidth * textureHeight];

            for (int y = 0; y < textureHeight; y++)
            {
                for (int x = 0; x < textureWidth; x++)
                {
                    Vector2 world = PixelToWorld(x, y);
                    float dist = DistanceToNetwork(world);
                    float vessel = 1f - Mathf.SmoothStep(vesselHalfWidth - edgeFeather, vesselHalfWidth, dist);
                    float centerGlow = 1f - Mathf.SmoothStep(0f, vesselHalfWidth * 0.65f, dist);
                    float noise = Mathf.PerlinNoise(world.x * 0.33f + 19.3f, world.y * 0.42f - 4.7f);
                    float ripple = Mathf.Sin(world.x * 1.35f + world.y * 0.55f) * 0.5f + 0.5f;

                    Color tissue = Color.Lerp(new Color(0.07f, 0.012f, 0.025f, 1f), new Color(0.19f, 0.03f, 0.055f, 1f), noise * 0.75f);
                    Color blood = Color.Lerp(new Color(0.34f, 0.01f, 0.025f, 1f), new Color(0.62f, 0.035f, 0.045f, 1f), centerGlow * 0.7f + ripple * 0.15f);
                    Color edge = new Color(0.18f, 0.004f, 0.018f, 1f);
                    Color final = Color.Lerp(tissue, Color.Lerp(edge, blood, Mathf.Clamp01(vessel * 1.25f)), vessel);
                    final += new Color(0.08f, 0.006f, 0.006f, 0f) * centerGlow;
                    final.r = Mathf.Clamp01(final.r);
                    final.g = Mathf.Clamp01(final.g);
                    final.b = Mathf.Clamp01(final.b);

                    int idx = y * textureWidth + x;
                    colors[idx] = final;
                    mask[idx] = vessel > 0.38f ? Color.white : Color.black;
                }
            }

            arenaTexture.SetPixels(colors);
            arenaTexture.Apply();
            maskTexture.SetPixels(mask);
            maskTexture.Apply();
        }

        private void SetupRenderer()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

            Sprite sprite = Sprite.Create(arenaTexture, new Rect(0, 0, textureWidth, textureHeight), new Vector2(0.5f, 0.5f), textureWidth / arenaSize.x);
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = sortingOrder;

            Shader shader = Shader.Find("Sprites/Default");
            runtimeMaterial = shader != null ? new Material(shader) : null;
            if (runtimeMaterial != null)
            {
                spriteRenderer.material = runtimeMaterial;
                runtimeMaterial.color = Color.white;
            }

            transform.position = Vector3.zero;
            transform.localScale = Vector3.one;
        }

        private void BuildVesselLines()
        {
            Transform old = transform.Find("VesselLines");
            if (old != null) Destroy(old.gameObject);

            GameObject root = new GameObject("VesselLines");
            root.transform.SetParent(transform, false);

            CreateVesselLine(root.transform, "MainVessel", pathPoints, vesselHalfWidth * 2f, new Color(0.48f, 0.015f, 0.025f, 0.78f));
        }

        private void CreateVesselLine(Transform parent, string name, Vector2[] points, float width, Color color)
        {
            GameObject lineObject = new GameObject(name);
            lineObject.transform.SetParent(parent, false);
            LineRenderer line = lineObject.AddComponent<LineRenderer>();
            line.positionCount = points.Length;
            line.useWorldSpace = false;
            line.widthMultiplier = width;
            line.numCapVertices = 24;
            line.numCornerVertices = 24;
            line.sortingOrder = sortingOrder + 1;
            Shader shader = Shader.Find("Sprites/Default");
            if (shader != null)
            {
                line.material = new Material(shader);
            }
            line.startColor = color;
            line.endColor = color;

            for (int i = 0; i < points.Length; i++)
            {
                line.SetPosition(i, new Vector3(points[i].x, points[i].y, -0.01f));
            }
        }

        private void SetupMask()
        {
            ArenaWalkableMask mask = FindFirstObjectByType<ArenaWalkableMask>(FindObjectsInactive.Include);
            if (mask == null)
            {
                GameObject go = new GameObject("ArenaWalkableMask");
                mask = go.AddComponent<ArenaWalkableMask>();
            }

            Vector2 min = new Vector2(-arenaSize.x * 0.5f, -arenaSize.y * 0.5f);
            Vector2 max = new Vector2(arenaSize.x * 0.5f, arenaSize.y * 0.5f);
            mask.SetRuntimeMask(maskTexture, min, max, 0.3f);
        }

        private void BuildBloodFlow()
        {
            Transform old = transform.Find("BloodFlowSimulator");
            if (old != null) Destroy(old.gameObject);

            GameObject flowObject = new GameObject("BloodFlowSimulator");
            flowObject.transform.SetParent(transform, false);
            BloodFlowSimulator flow = flowObject.AddComponent<BloodFlowSimulator>();
            flow.Configure(arenaBounds, vesselHalfWidth, sortingOrder + 40, runtimeCamera);
        }

        private void SetupCamera()
        {
            Camera cam = Camera.main;
            if (cam == null) cam = FindFirstObjectByType<Camera>(FindObjectsInactive.Include);
            if (cam == null) return;

            runtimeCamera = cam;
            cam.orthographic = true;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.025f, 0.04f, 0.065f, 1f);
            cam.orthographicSize = Mathf.Min(7.2f, arenaSize.y * 0.42f);

            CameraController controller = cam.GetComponent<CameraController>();
            if (controller == null) controller = cam.gameObject.AddComponent<CameraController>();
            controller.SetBounds(arenaBounds, 0.5f);
            controller.SetZoomLimits(3.2f, arenaSize.y * 0.55f);
            controller.SetPosition(new Vector3(0f, 0f, -10f));
        }

        private void EnsurePathfinder()
        {
            BloodstreamPathfinder pathfinder = FindFirstObjectByType<BloodstreamPathfinder>(FindObjectsInactive.Include);
            if (pathfinder == null)
            {
                GameObject go = new GameObject("BloodstreamPathfinder");
                pathfinder = go.AddComponent<BloodstreamPathfinder>();
            }
            pathfinder.InitializeGrid();
        }

        private Vector2 PixelToWorld(int x, int y)
        {
            float u = (x + 0.5f) / textureWidth;
            float v = (y + 0.5f) / textureHeight;
            return new Vector2((u - 0.5f) * arenaSize.x, (v - 0.5f) * arenaSize.y);
        }

        private float DistanceToNetwork(Vector2 p)
        {
            return DistanceToPolyline(p, pathPoints);
        }

        private float DistanceToPolyline(Vector2 p, Vector2[] points)
        {
            float min = float.MaxValue;
            for (int i = 0; i < points.Length - 1; i++)
            {
                min = Mathf.Min(min, DistanceToSegment(p, points[i], points[i + 1]));
            }
            return min;
        }

        private float DistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
        {
            Vector2 ab = b - a;
            float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / ab.sqrMagnitude);
            return Vector2.Distance(p, a + ab * t);
        }
    }
}

