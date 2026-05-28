using System.Collections.Generic;
using UnityEngine;
using ChronicSurvival.Core;

namespace ChronicSurvival.Arena
{
    [DefaultExecutionOrder(-500)]
    public class ProceduralBloodVesselArena : MonoBehaviour
    {
        private enum SegmentType
        {
            Straight,
            Branch,
            Arena
        }

        private struct SegmentSocket
        {
            public Vector2 position;
            public Vector2 direction;
            public float width;
            public bool isMainRoute;
        }

        private sealed class SegmentDefinition
        {
            public SegmentType type;
            public Vector2[] localPath;
            public Vector2[] branchPath;
            public Vector2 localExitPosition;
            public Vector2 localExitDirection;
            public Vector2 localBranchPosition;
            public Vector2 localBranchDirection;
            public float width;
            public bool createsBranch;
        }

        private sealed class PlacedSegment
        {
            public float width;
            public Vector2[] mainPath;
            public Vector2[] branchPath;
            public SegmentSocket exitSocket;
            public SegmentSocket branchSocket;
            public Rect bounds;
        }

        [Header("Arena")]
        [SerializeField] private Vector2 arenaSize = new Vector2(96f, 48f);
        [SerializeField] private int textureWidth = 480;
        [SerializeField] private int textureHeight = 240;
        [SerializeField] private float defaultVesselHalfWidth = 4.6f;
        [SerializeField] private float edgeFeather = 1.15f;
        [SerializeField] private int sortingOrder = -120;

        [Header("Streaming")]
        [SerializeField] private int initialMainSegments = 5;
        [SerializeField] private int initialExtraBranches = 2;
        [SerializeField] private float generationTriggerDistance = 24f;
        [SerializeField] private float minimumMainAheadDistance = 42f;
        [SerializeField] private int maxMainRouteSegments = 72;
        [SerializeField] private int maxBranchSegments = 28;
        [SerializeField] private float segmentSpacingPadding = 4f;
        [SerializeField] private float cameraBoundsPadding = 18f;

        [Header("Branching")]
        [SerializeField] private float branchChance = 0.4f;
        [SerializeField] private float arenaChance = 0.24f;

        private readonly List<PlacedSegment> placedSegments = new List<PlacedSegment>();
        private readonly List<SegmentSocket> frontierSockets = new List<SegmentSocket>();
        private readonly List<SegmentDefinition> straightDefinitions = new List<SegmentDefinition>();
        private readonly List<SegmentDefinition> branchDefinitions = new List<SegmentDefinition>();
        private readonly List<SegmentDefinition> arenaDefinitions = new List<SegmentDefinition>();

        private SpriteRenderer spriteRenderer;
        private Material runtimeMaterial;
        private Texture2D arenaTexture;
        private Texture2D maskTexture;
        private Bounds arenaBounds;
        private Camera runtimeCamera;
        private CameraController cameraController;
        private BloodFlowSimulator flowSimulator;

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
            InitializeDefinitions();
            BuildArena();
        }

        private void Update()
        {
            if (runtimeCamera == null) return;
            GenerateNearCameraFrontiers();
        }

        private void BuildArena()
        {
            SetupCamera();
            GenerateInitialLayout();
            RebuildArenaPresentation();
            BuildBloodFlow();
            EnsurePathfinder();
            RefreshCameraBounds();
        }

        private void InitializeDefinitions()
        {
            if (straightDefinitions.Count > 0) return;

            straightDefinitions.Add(CreateStraightDefinition(
                new[]
                {
                    new Vector2(0f, 0f),
                    new Vector2(7f, -0.3f),
                    new Vector2(14f, 0.2f),
                    new Vector2(21f, 0f)
                },
                defaultVesselHalfWidth));

            straightDefinitions.Add(CreateStraightDefinition(
                new[]
                {
                    new Vector2(0f, 0f),
                    new Vector2(6f, 0.55f),
                    new Vector2(12.5f, -0.25f),
                    new Vector2(19.5f, 0.35f)
                },
                defaultVesselHalfWidth * 0.98f));

            straightDefinitions.Add(CreateStraightDefinition(
                new[]
                {
                    new Vector2(0f, 0f),
                    new Vector2(8f, -0.5f),
                    new Vector2(15f, -0.1f),
                    new Vector2(23f, 0.45f)
                },
                defaultVesselHalfWidth * 1.02f));

            branchDefinitions.Add(CreateBranchDefinition(
                new[]
                {
                    new Vector2(0f, 0f),
                    new Vector2(6f, 0.1f),
                    new Vector2(12f, 0f),
                    new Vector2(18f, 0.2f)
                },
                new[]
                {
                    new Vector2(10f, 0f),
                    new Vector2(13f, 3.2f),
                    new Vector2(17f, 5.5f)
                },
                defaultVesselHalfWidth * 0.92f));

            branchDefinitions.Add(CreateBranchDefinition(
                new[]
                {
                    new Vector2(0f, 0f),
                    new Vector2(5.5f, -0.2f),
                    new Vector2(11.5f, 0.3f),
                    new Vector2(17.5f, 0.1f)
                },
                new[]
                {
                    new Vector2(9f, 0.2f),
                    new Vector2(12.5f, -2.8f),
                    new Vector2(16.5f, -5f)
                },
                defaultVesselHalfWidth * 0.9f));

            branchDefinitions.Add(CreateBranchDefinition(
                new[]
                {
                    new Vector2(0f, 0f),
                    new Vector2(6.5f, 0.15f),
                    new Vector2(13.5f, 0.15f),
                    new Vector2(20f, -0.1f)
                },
                new[]
                {
                    new Vector2(11.5f, 0.1f),
                    new Vector2(15.2f, 2.3f),
                    new Vector2(19f, 3.6f)
                },
                defaultVesselHalfWidth * 0.95f));

            arenaDefinitions.Add(CreateStraightDefinition(
                new[]
                {
                    new Vector2(0f, 0f),
                    new Vector2(7f, -0.3f),
                    new Vector2(15f, 0.25f),
                    new Vector2(24f, 0f)
                },
                defaultVesselHalfWidth * 1.65f));

            arenaDefinitions.Add(CreateStraightDefinition(
                new[]
                {
                    new Vector2(0f, 0f),
                    new Vector2(6f, 0.45f),
                    new Vector2(14f, -0.2f),
                    new Vector2(22f, 0.15f)
                },
                defaultVesselHalfWidth * 1.8f));

            arenaDefinitions.Add(CreateStraightDefinition(
                new[]
                {
                    new Vector2(0f, 0f),
                    new Vector2(8f, -0.45f),
                    new Vector2(16f, 0.35f),
                    new Vector2(25f, -0.05f)
                },
                defaultVesselHalfWidth * 1.7f));
        }

        private SegmentDefinition CreateStraightDefinition(Vector2[] path, float width)
        {
            return new SegmentDefinition
            {
                type = SegmentType.Straight,
                localPath = path,
                width = width,
                localExitPosition = path[path.Length - 1],
                localExitDirection = (path[path.Length - 1] - path[path.Length - 2]).normalized
            };
        }

        private SegmentDefinition CreateBranchDefinition(Vector2[] path, Vector2[] branchPath, float width)
        {
            return new SegmentDefinition
            {
                type = SegmentType.Branch,
                localPath = path,
                branchPath = branchPath,
                width = width,
                createsBranch = true,
                localExitPosition = path[path.Length - 1],
                localExitDirection = (path[path.Length - 1] - path[path.Length - 2]).normalized,
                localBranchPosition = branchPath[0],
                localBranchDirection = (branchPath[branchPath.Length - 1] - branchPath[branchPath.Length - 2]).normalized
            };
        }

        private void GenerateInitialLayout()
        {
            placedSegments.Clear();
            frontierSockets.Clear();

            SegmentSocket startSocket = new SegmentSocket
            {
                position = new Vector2(-18f, 0f),
                direction = Vector2.right,
                width = defaultVesselHalfWidth,
                isMainRoute = true
            };

            SegmentDefinition startingDefinition = arenaDefinitions[Random.Range(0, arenaDefinitions.Count)];
            PlacedSegment startingSegment = TryPlaceSegment(startSocket, startingDefinition);
            if (startingSegment == null) return;

            AddPlacedSegment(startingSegment);

            for (int i = 0; i < initialMainSegments; i++)
            {
                if (!TryExpandMainRoute()) break;
            }

            for (int i = 0; i < initialExtraBranches; i++)
            {
                TryExpandBranchRoute();
            }
        }

        private void GenerateNearCameraFrontiers()
        {
            if (frontierSockets.Count == 0 || ReachedSegmentCapacity()) return;

            Vector2 cameraPos = runtimeCamera.transform.position;
            bool generated = false;

            for (int i = frontierSockets.Count - 1; i >= 0; i--)
            {
                SegmentSocket socket = frontierSockets[i];
                if (Vector2.Distance(cameraPos, socket.position) > generationTriggerDistance) continue;
                generated |= socket.isMainRoute ? TryExpandSpecificSocket(i, true) : TryExpandSpecificSocket(i, false);
            }

            if (!generated)
            {
                EnsureMainRouteAhead(cameraPos);
                return;
            }

            RebuildArenaPresentation();
            RefreshFlowAndPathing();
            RefreshCameraBounds();
            EnsureMainRouteAhead(cameraPos);
        }

        private void EnsureMainRouteAhead(Vector2 cameraPos)
        {
            float farthestAhead = float.MinValue;
            for (int i = 0; i < frontierSockets.Count; i++)
            {
                if (!frontierSockets[i].isMainRoute) continue;
                farthestAhead = Mathf.Max(farthestAhead, frontierSockets[i].position.x - cameraPos.x);
            }

            bool changed = false;
            while (farthestAhead < minimumMainAheadDistance && !ReachedMainRouteCapacity())
            {
                if (!TryExpandMainRoute()) break;
                changed = true;
                farthestAhead = float.MinValue;
                for (int i = 0; i < frontierSockets.Count; i++)
                {
                    if (!frontierSockets[i].isMainRoute) continue;
                    farthestAhead = Mathf.Max(farthestAhead, frontierSockets[i].position.x - cameraPos.x);
                }
            }

            if (!changed) return;

            RebuildArenaPresentation();
            RefreshFlowAndPathing();
            RefreshCameraBounds();
        }

        private bool ReachedSegmentCapacity()
        {
            return ReachedMainRouteCapacity() && ReachedBranchCapacity();
        }

        private bool ReachedMainRouteCapacity()
        {
            return CountPlacedSegments(true) >= maxMainRouteSegments;
        }

        private bool ReachedBranchCapacity()
        {
            return CountPlacedSegments(false) >= maxBranchSegments;
        }

        private int CountPlacedSegments(bool mainRoute)
        {
            int count = 0;
            for (int i = 0; i < placedSegments.Count; i++)
            {
                if (placedSegments[i].exitSocket.isMainRoute == mainRoute)
                {
                    count++;
                }
            }
            return count;
        }

        private bool TryExpandMainRoute()
        {
            if (ReachedMainRouteCapacity()) return false;

            int socketIndex = frontierSockets.FindIndex(socket => socket.isMainRoute);
            if (socketIndex < 0) return false;
            return TryExpandSpecificSocket(socketIndex, true);
        }

        private bool TryExpandBranchRoute()
        {
            if (ReachedBranchCapacity()) return false;

            int socketIndex = frontierSockets.FindIndex(socket => !socket.isMainRoute);
            if (socketIndex < 0) return false;
            return TryExpandSpecificSocket(socketIndex, false);
        }

        private bool TryExpandSpecificSocket(int socketIndex, bool isMainRoute)
        {
            if (socketIndex < 0 || socketIndex >= frontierSockets.Count) return false;

            SegmentSocket socket = frontierSockets[socketIndex];
            List<SegmentDefinition> candidates = BuildCandidateList(isMainRoute);
            for (int attempt = 0; attempt < candidates.Count; attempt++)
            {
                PlacedSegment placed = TryPlaceSegment(socket, candidates[attempt]);
                if (placed == null) continue;

                frontierSockets.RemoveAt(socketIndex);
                AddPlacedSegment(placed);
                return true;
            }

            frontierSockets.RemoveAt(socketIndex);
            return false;
        }

        private List<SegmentDefinition> BuildCandidateList(bool isMainRoute)
        {
            List<SegmentDefinition> candidates = new List<SegmentDefinition>();
            if (isMainRoute && Random.value < arenaChance)
            {
                AddShuffled(candidates, arenaDefinitions);
            }

            AddShuffled(candidates, straightDefinitions);

            if (Random.value < branchChance)
            {
                AddShuffled(candidates, branchDefinitions);
            }

            if (!isMainRoute)
            {
                AddShuffled(candidates, straightDefinitions);
            }

            return candidates;
        }

        private void AddShuffled(List<SegmentDefinition> target, List<SegmentDefinition> source)
        {
            List<SegmentDefinition> temp = new List<SegmentDefinition>(source);
            for (int i = 0; i < temp.Count; i++)
            {
                int swap = Random.Range(i, temp.Count);
                SegmentDefinition value = temp[i];
                temp[i] = temp[swap];
                temp[swap] = value;
            }
            target.AddRange(temp);
        }

        private PlacedSegment TryPlaceSegment(SegmentSocket socket, SegmentDefinition definition)
        {
            Quaternion rotation = Quaternion.FromToRotation(Vector3.right, socket.direction);
            Vector2[] mainPath = TransformPath(definition.localPath, socket.position, rotation, definition.localPath[0]);
            Vector2[] branchPath = definition.branchPath != null ? TransformPath(definition.branchPath, socket.position, rotation, definition.localPath[0]) : null;

            Rect bounds = CalculateBounds(mainPath, branchPath, definition.width + segmentSpacingPadding);
            if (OverlapsExisting(bounds, socket.position, definition.width * 1.15f))
            {
                return null;
            }

            SegmentSocket exitSocket = new SegmentSocket
            {
                position = TransformPoint(definition.localExitPosition, socket.position, rotation, definition.localPath[0]),
                direction = (rotation * definition.localExitDirection).normalized,
                width = definition.width,
                isMainRoute = socket.isMainRoute
            };

            SegmentSocket branchSocket = default;
            if (definition.createsBranch)
            {
                branchSocket = new SegmentSocket
                {
                    position = TransformPoint(definition.localBranchPosition, socket.position, rotation, definition.localPath[0]),
                    direction = (rotation * definition.localBranchDirection).normalized,
                    width = definition.width * 0.88f,
                    isMainRoute = false
                };
            }

            return new PlacedSegment
            {
                width = definition.width,
                mainPath = mainPath,
                branchPath = branchPath,
                exitSocket = exitSocket,
                branchSocket = branchSocket,
                bounds = bounds
            };
        }

        private Vector2[] TransformPath(Vector2[] localPath, Vector2 anchorPosition, Quaternion rotation, Vector2 localOrigin)
        {
            Vector2[] result = new Vector2[localPath.Length];
            for (int i = 0; i < localPath.Length; i++)
            {
                result[i] = TransformPoint(localPath[i], anchorPosition, rotation, localOrigin);
            }
            return result;
        }

        private Vector2 TransformPoint(Vector2 point, Vector2 anchorPosition, Quaternion rotation, Vector2 localOrigin)
        {
            return anchorPosition + (Vector2)(rotation * (point - localOrigin));
        }

        private Rect CalculateBounds(Vector2[] pathA, Vector2[] pathB, float padding)
        {
            Vector2 min = pathA[0];
            Vector2 max = pathA[0];
            ExpandBounds(pathA, ref min, ref max);
            if (pathB != null)
            {
                ExpandBounds(pathB, ref min, ref max);
            }

            min -= Vector2.one * padding;
            max += Vector2.one * padding;
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

        private void ExpandBounds(Vector2[] path, ref Vector2 min, ref Vector2 max)
        {
            for (int i = 0; i < path.Length; i++)
            {
                min = Vector2.Min(min, path[i]);
                max = Vector2.Max(max, path[i]);
            }
        }

        private bool OverlapsExisting(Rect candidate, Vector2 connectionPoint, float allowedConnectionRadius)
        {
            Rect allowedRect = Rect.MinMaxRect(
                connectionPoint.x - allowedConnectionRadius,
                connectionPoint.y - allowedConnectionRadius,
                connectionPoint.x + allowedConnectionRadius,
                connectionPoint.y + allowedConnectionRadius
            );

            for (int i = 0; i < placedSegments.Count; i++)
            {
                if (!candidate.Overlaps(placedSegments[i].bounds)) continue;

                Rect overlap = GetOverlapRect(candidate, placedSegments[i].bounds);
                if (overlap.width <= 0f || overlap.height <= 0f) continue;
                if (!allowedRect.Overlaps(overlap)) return true;
            }

            return false;
        }

        private Rect GetOverlapRect(Rect a, Rect b)
        {
            return Rect.MinMaxRect(
                Mathf.Max(a.xMin, b.xMin),
                Mathf.Max(a.yMin, b.yMin),
                Mathf.Min(a.xMax, b.xMax),
                Mathf.Min(a.yMax, b.yMax)
            );
        }

        private void AddPlacedSegment(PlacedSegment placed)
        {
            placedSegments.Add(placed);
            frontierSockets.Add(placed.exitSocket);
            if (placed.branchPath != null)
            {
                frontierSockets.Add(placed.branchSocket);
            }
        }

        private void RebuildArenaPresentation()
        {
            RecalculateArenaBounds();
            CreateTextures();
            SetupRenderer();
            BuildVesselLines();
            SetupMask();
        }

        private void RefreshFlowAndPathing()
        {
            if (flowSimulator != null)
            {
                flowSimulator.Reconfigure(arenaBounds, GetAverageHalfWidth(), runtimeCamera, GetPrimaryRouteBounds());
            }

            if (BloodstreamPathfinder.Instance != null)
            {
                BloodstreamPathfinder.Instance.InitializeGrid();
            }
        }

        private void RefreshCameraBounds()
        {
            if (cameraController == null) return;

            Bounds paddedBounds = arenaBounds;
            paddedBounds.Expand(new Vector3(cameraBoundsPadding * 2f, cameraBoundsPadding * 2f, 0f));
            cameraController.SetBounds(paddedBounds, 0.5f);
        }

        private void RecalculateArenaBounds()
        {
            if (placedSegments.Count == 0)
            {
                arenaBounds = new Bounds(Vector3.zero, new Vector3(arenaSize.x, arenaSize.y, 0f));
                return;
            }

            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);
            for (int i = 0; i < placedSegments.Count; i++)
            {
                Rect rect = placedSegments[i].bounds;
                min = Vector2.Min(min, rect.min);
                max = Vector2.Max(max, rect.max);
            }

            Vector2 size = max - min;
            size.x = Mathf.Max(size.x, arenaSize.x);
            size.y = Mathf.Max(size.y, arenaSize.y);
            arenaBounds = new Bounds((min + max) * 0.5f, new Vector3(size.x, size.y, 0f));
        }

        private void CreateTextures()
        {
            arenaTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGBA32, false);
            arenaTexture.wrapMode = TextureWrapMode.Clamp;
            arenaTexture.filterMode = FilterMode.Point;

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
                    float dist = DistanceToNetwork(world, out float widthAtPoint);
                    float vesselHalfWidth = Mathf.Max(defaultVesselHalfWidth * 0.7f, widthAtPoint);
                    float vessel = 1f - Mathf.SmoothStep(vesselHalfWidth - edgeFeather, vesselHalfWidth, dist);
                    float centerGlow = 1f - Mathf.SmoothStep(0f, vesselHalfWidth * 0.68f, dist);
                    float noise = Mathf.PerlinNoise(world.x * 0.27f + 19.3f, world.y * 0.38f - 4.7f);
                    float ripple = Mathf.Sin(world.x * 1.05f + world.y * 0.52f) * 0.5f + 0.5f;

                    Color tissue = Color.Lerp(new Color(0.07f, 0.012f, 0.025f, 1f), new Color(0.19f, 0.03f, 0.055f, 1f), Quantize01(noise, 4));
                    Color blood = Color.Lerp(new Color(0.34f, 0.01f, 0.025f, 1f), new Color(0.62f, 0.035f, 0.045f, 1f), Quantize01(centerGlow * 0.72f + ripple * 0.14f, 4));
                    Color edge = new Color(0.18f, 0.004f, 0.018f, 1f);
                    float vesselBlend = Quantize01(Mathf.Clamp01(vessel * 1.2f), 3);
                    Color final = Color.Lerp(tissue, Color.Lerp(edge, blood, vesselBlend), Quantize01(vessel, 3));
                    final += new Color(0.08f, 0.006f, 0.006f, 0f) * Quantize01(centerGlow, 3);
                    final = QuantizeColor(final, 5);

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

            Sprite sprite = Sprite.Create(arenaTexture, new Rect(0, 0, textureWidth, textureHeight), new Vector2(0.5f, 0.5f), textureWidth / arenaBounds.size.x);
            spriteRenderer.sprite = sprite;
            spriteRenderer.sortingOrder = sortingOrder;

            Shader shader = Shader.Find("Sprites/Default");
            runtimeMaterial = shader != null ? new Material(shader) : null;
            if (runtimeMaterial != null)
            {
                spriteRenderer.material = runtimeMaterial;
                runtimeMaterial.color = Color.white;
            }

            transform.position = arenaBounds.center;
            transform.localScale = Vector3.one;
        }

        private void BuildVesselLines()
        {
            Transform old = transform.Find("VesselLines");
            if (old != null) Destroy(old.gameObject);

            GameObject root = new GameObject("VesselLines");
            root.transform.SetParent(transform, false);

            for (int i = 0; i < placedSegments.Count; i++)
            {
                CreateVesselLine(root.transform, $"Segment_{i}", placedSegments[i].mainPath, placedSegments[i].width * 2f, new Color(0.48f, 0.015f, 0.025f, 0.78f));
                if (placedSegments[i].branchPath != null)
                {
                    CreateVesselLine(root.transform, $"Segment_{i}_Branch", placedSegments[i].branchPath, placedSegments[i].width * 1.7f, new Color(0.45f, 0.015f, 0.03f, 0.72f));
                }
            }
        }

        private void CreateVesselLine(Transform parent, string name, Vector2[] points, float width, Color color)
        {
            GameObject lineObject = new GameObject(name);
            lineObject.transform.SetParent(parent, false);
            LineRenderer line = lineObject.AddComponent<LineRenderer>();
            line.positionCount = points.Length;
            line.useWorldSpace = true;
            line.widthMultiplier = width;
            line.numCapVertices = 2;
            line.numCornerVertices = 2;
            line.sortingOrder = sortingOrder + 1;
            Shader shader = Shader.Find("Sprites/Default");
            if (shader != null)
            {
                line.material = new Material(shader);
            }
            line.startColor = QuantizeColor(color, 4);
            line.endColor = QuantizeColor(color, 4);

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

            mask.SetRuntimeMask(maskTexture, arenaBounds.min, arenaBounds.max, 0.3f);
        }

        private void BuildBloodFlow()
        {
            Transform old = transform.Find("BloodFlowSimulator");
            if (old != null) Destroy(old.gameObject);

            GameObject flowObject = new GameObject("BloodFlowSimulator");
            flowObject.transform.SetParent(transform, false);
            flowSimulator = flowObject.AddComponent<BloodFlowSimulator>();
            flowSimulator.Configure(arenaBounds, GetAverageHalfWidth(), sortingOrder + 40, runtimeCamera, GetPrimaryRouteBounds());
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

            cameraController = cam.GetComponent<CameraController>();
            if (cameraController == null) cameraController = cam.gameObject.AddComponent<CameraController>();
            cameraController.SetBounds(new Bounds(Vector3.zero, new Vector3(arenaSize.x * 4f, arenaSize.y * 4f, 0f)), 0.5f);
            cameraController.SetZoomLimits(3.2f, arenaSize.y * 0.55f);
            cameraController.SetPosition(new Vector3(0f, 0f, -10f));
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

        private float DistanceToNetwork(Vector2 point, out float widthAtPoint)
        {
            float minDistance = float.MaxValue;
            widthAtPoint = defaultVesselHalfWidth;

            for (int i = 0; i < placedSegments.Count; i++)
            {
                float candidate = DistanceToPolyline(point, placedSegments[i].mainPath);
                if (candidate < minDistance)
                {
                    minDistance = candidate;
                    widthAtPoint = placedSegments[i].width;
                }

                if (placedSegments[i].branchPath == null) continue;

                float branchDistance = DistanceToPolyline(point, placedSegments[i].branchPath);
                if (branchDistance < minDistance)
                {
                    minDistance = branchDistance;
                    widthAtPoint = placedSegments[i].width * 0.88f;
                }
            }

            return minDistance;
        }

        private Vector2 PixelToWorld(int x, int y)
        {
            float u = (x + 0.5f) / textureWidth;
            float v = (y + 0.5f) / textureHeight;
            return new Vector2(
                Mathf.Lerp(arenaBounds.min.x, arenaBounds.max.x, u),
                Mathf.Lerp(arenaBounds.min.y, arenaBounds.max.y, v)
            );
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
            float denominator = ab.sqrMagnitude;
            if (denominator <= 0.0001f) return Vector2.Distance(p, a);
            float t = Mathf.Clamp01(Vector2.Dot(p - a, ab) / denominator);
            return Vector2.Distance(p, a + ab * t);
        }

        private float GetAverageHalfWidth()
        {
            if (placedSegments.Count == 0) return defaultVesselHalfWidth;
            float total = 0f;
            for (int i = 0; i < placedSegments.Count; i++)
            {
                total += placedSegments[i].width;
            }
            return total / placedSegments.Count;
        }

        private Bounds GetPrimaryRouteBounds()
        {
            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);
            bool found = false;

            for (int i = 0; i < placedSegments.Count; i++)
            {
                ExpandBounds(placedSegments[i].mainPath, ref min, ref max);
                found = true;
            }

            if (!found)
            {
                return arenaBounds;
            }

            Vector2 padding = new Vector2(4f, defaultVesselHalfWidth * 2.4f);
            Vector2 size = (max - min) + padding * 2f;
            return new Bounds((min + max) * 0.5f, new Vector3(Mathf.Max(size.x, 20f), Mathf.Max(size.y, 12f), 0f));
        }

        private static float Quantize01(float value, int steps)
        {
            value = Mathf.Clamp01(value);
            if (steps <= 1) return value;
            return Mathf.Round(value * (steps - 1)) / (steps - 1);
        }

        private static Color QuantizeColor(Color color, int steps)
        {
            return new Color(
                Quantize01(color.r, steps),
                Quantize01(color.g, steps),
                Quantize01(color.b, steps),
                color.a
            );
        }
    }
}
