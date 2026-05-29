using UnityEngine;

namespace ChronicSurvival.Arena
{
    /// <summary>
    /// Reads a walkable mask texture to determine which areas of the arena
    /// are blood vessels (walkable) vs tissue walls (non-walkable).
    /// 
    /// The mask should be a grayscale texture where:
    ///   - White/bright pixels = walkable (blood vessel interior)
    ///   - Black/dark pixels = non-walkable (tissue/walls)
    /// 
    /// This texture is mapped to world-space coordinates via the arena bounds.
    /// </summary>
    public class ArenaWalkableMask : MonoBehaviour
    {
        public static ArenaWalkableMask Instance { get; private set; }
        public bool IsInitialized => isInitialized;

        [Header("Walkable Mask")]
        [SerializeField] private Texture2D walkableMask;

        [Header("Arena Bounds (World Space)")]
        [Tooltip("Bottom-left corner of the arena in world space")]
        [SerializeField] private Vector2 arenaBoundsMin = new Vector2(-9.6f, -5.4f);
        [Tooltip("Top-right corner of the arena in world space")]
        [SerializeField] private Vector2 arenaBoundsMax = new Vector2(9.6f, 5.4f);

        [Header("Settings")]
        [Tooltip("Brightness threshold (0-1). Pixels brighter than this are walkable.")]
        [SerializeField] private float walkableThreshold = 0.3f;

        [Header("Debug")]
        [SerializeField] private bool debugMode = false;
        [SerializeField] private bool drawGizmos = true;

        private Color[] pixelData;
        private int maskWidth;
        private int maskHeight;
        private bool isInitialized = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            Initialize();
        }

        private void Initialize()
        {
            if (walkableMask == null)
            {
                isInitialized = false;
                return;
            }

            // Ensure texture is readable
            try
            {
                pixelData = walkableMask.GetPixels();
                maskWidth = walkableMask.width;
                maskHeight = walkableMask.height;
                isInitialized = true;

                if (debugMode)
                    Debug.Log($"[ArenaWalkableMask] Initialized mask: {maskWidth}x{maskHeight}");
            }
            catch (UnityException)
            {
                Debug.LogError("[ArenaWalkableMask] Texture is not readable! " +
                    "Go to the texture import settings and enable 'Read/Write'.");
            }
        }

        public void SetRuntimeMask(Texture2D mask, Vector2 boundsMin, Vector2 boundsMax, float threshold = 0.3f)
        {
            walkableMask = mask;
            arenaBoundsMin = boundsMin;
            arenaBoundsMax = boundsMax;
            walkableThreshold = threshold;
            Initialize();
        }

        public bool IsWalkable(Vector2 worldPosition)
        {
            if (!isInitialized) return false; // Fallback: block movement if mask is unavailable

            // Convert world position to UV (0-1)
            float u = Mathf.InverseLerp(arenaBoundsMin.x, arenaBoundsMax.x, worldPosition.x);
            float v = Mathf.InverseLerp(arenaBoundsMin.y, arenaBoundsMax.y, worldPosition.y);

            // Out of bounds check
            if (u < 0f || u > 1f || v < 0f || v > 1f)
                return false;

            // Convert UV to pixel coordinates
            int pixelX = Mathf.Clamp(Mathf.FloorToInt(u * maskWidth), 0, maskWidth - 1);
            int pixelY = Mathf.Clamp(Mathf.FloorToInt(v * maskHeight), 0, maskHeight - 1);

            // Read pixel brightness
            Color pixel = pixelData[pixelY * maskWidth + pixelX];
            float brightness = pixel.grayscale;

            return brightness >= walkableThreshold;
        }

        public bool IsWalkableWithRadius(Vector2 worldPosition, float radius, int radialSamples = 8)
        {
            if (radius <= 0.001f)
            {
                return IsWalkable(worldPosition);
            }

            if (!IsWalkable(worldPosition))
            {
                return false;
            }

            radialSamples = Mathf.Max(4, radialSamples);
            for (int i = 0; i < radialSamples; i++)
            {
                float angle = (i / (float)radialSamples) * Mathf.PI * 2f;
                Vector2 edge = worldPosition + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                if (!IsWalkable(edge))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Get the nearest walkable position to a given world position.
        /// Searches in expanding circles until a walkable position is found.
        /// </summary>
        public Vector2 GetNearestWalkablePosition(Vector2 worldPosition, float maxSearchRadius = 3f)
        {
            if (!isInitialized)
                return worldPosition;

            if (IsWalkable(worldPosition))
                return worldPosition;

            float searchStep = 0.2f;

            for (float radius = searchStep; radius <= maxSearchRadius; radius += searchStep)
            {
                // Check 16 directions around the point
                for (int i = 0; i < 16; i++)
                {
                    float angle = (i / 16f) * Mathf.PI * 2f;
                    Vector2 testPos = worldPosition + new Vector2(
                        Mathf.Cos(angle) * radius,
                        Mathf.Sin(angle) * radius
                    );

                    if (IsWalkable(testPos))
                        return testPos;
                }
            }

            // Fallback: return original position
            if (debugMode)
                Debug.LogWarning($"[ArenaWalkableMask] Could not find walkable pos near {worldPosition}");

            return FindNearestWalkableByGridSearch(worldPosition);
        }

        public Vector2 GetNearestWalkablePosition(Vector2 worldPosition, float maxSearchRadius, float radius, int radialSamples = 8)
        {
            if (!isInitialized)
                return worldPosition;

            if (IsWalkableWithRadius(worldPosition, radius, radialSamples))
                return worldPosition;

            float searchStep = 0.2f;
            for (float searchRadius = searchStep; searchRadius <= maxSearchRadius; searchRadius += searchStep)
            {
                for (int i = 0; i < 24; i++)
                {
                    float angle = (i / 24f) * Mathf.PI * 2f;
                    Vector2 testPos = worldPosition + new Vector2(
                        Mathf.Cos(angle) * searchRadius,
                        Mathf.Sin(angle) * searchRadius
                    );

                    if (IsWalkableWithRadius(testPos, radius, radialSamples))
                        return testPos;
                }
            }

            Vector2 fallback = FindNearestWalkableByGridSearch(worldPosition);
            if (IsWalkableWithRadius(fallback, radius, radialSamples))
                return fallback;

            // Relax radius as a final fallback so units never remain inside black zones.
            for (float relax = radius * 0.85f; relax >= 0.02f; relax *= 0.75f)
            {
                if (IsWalkableWithRadius(fallback, relax, radialSamples))
                    return fallback;
            }

            return fallback;
        }

        /// <summary>
        /// Get a random walkable position within the arena.
        /// </summary>
        public Vector2 GetRandomWalkablePosition(int maxAttempts = 50)
        {
            for (int i = 0; i < maxAttempts; i++)
            {
                Vector2 randomPos = new Vector2(
                    Random.Range(arenaBoundsMin.x, arenaBoundsMax.x),
                    Random.Range(arenaBoundsMin.y, arenaBoundsMax.y)
                );

                if (IsWalkable(randomPos))
                    return randomPos;
            }

            // Fallback: return center
            if (debugMode)
                Debug.LogWarning("[ArenaWalkableMask] Could not find random walkable position");

            return (arenaBoundsMin + arenaBoundsMax) * 0.5f;
        }

        /// <summary>
        /// Constrain movement: given a current position and a desired position,
        /// returns the farthest walkable point along that movement vector.
        /// </summary>
        public Vector2 ConstrainMovement(Vector2 currentPos, Vector2 desiredPos)
        {
            if (IsWalkable(desiredPos))
                return desiredPos;

            // Binary search along the movement vector to find the boundary
            Vector2 validPos = currentPos;
            Vector2 testPos = desiredPos;

            for (int i = 0; i < 8; i++)
            {
                Vector2 midPoint = (validPos + testPos) * 0.5f;

                if (IsWalkable(midPoint))
                    validPos = midPoint;
                else
                    testPos = midPoint;
            }

            return validPos;
        }

        public Vector2 ConstrainMovement(Vector2 currentPos, Vector2 desiredPos, float radius, int radialSamples = 8)
        {
            if (IsWalkableWithRadius(desiredPos, radius, radialSamples))
                return desiredPos;

            Vector2 validPos = IsWalkableWithRadius(currentPos, radius, radialSamples)
                ? currentPos
                : GetNearestWalkablePosition(currentPos, radius + 2.5f, radius, radialSamples);
            Vector2 testPos = desiredPos;

            for (int i = 0; i < 10; i++)
            {
                Vector2 midPoint = (validPos + testPos) * 0.5f;
                if (IsWalkableWithRadius(midPoint, radius, radialSamples))
                    validPos = midPoint;
                else
                    testPos = midPoint;
            }

            return validPos;
        }

        private Vector2 FindNearestWalkableByGridSearch(Vector2 worldPosition)
        {
            if (!isInitialized)
            {
                return (arenaBoundsMin + arenaBoundsMax) * 0.5f;
            }

            int samplesX = Mathf.Max(16, Mathf.CeilToInt((arenaBoundsMax.x - arenaBoundsMin.x) / 1.5f));
            int samplesY = Mathf.Max(10, Mathf.CeilToInt((arenaBoundsMax.y - arenaBoundsMin.y) / 1.5f));
            float bestDist = float.MaxValue;
            Vector2 bestPos = worldPosition;

            for (int ix = 0; ix <= samplesX; ix++)
            {
                float u = ix / (float)samplesX;
                float x = Mathf.Lerp(arenaBoundsMin.x, arenaBoundsMax.x, u);
                for (int iy = 0; iy <= samplesY; iy++)
                {
                    float v = iy / (float)samplesY;
                    float y = Mathf.Lerp(arenaBoundsMin.y, arenaBoundsMax.y, v);
                    Vector2 p = new Vector2(x, y);
                    if (!IsWalkable(p)) continue;
                    float d = (p - worldPosition).sqrMagnitude;
                    if (d < bestDist)
                    {
                        bestDist = d;
                        bestPos = p;
                    }
                }
            }

            return bestDist < float.MaxValue ? bestPos : worldPosition;
        }

        /// <summary>
        /// Get arena bounds.
        /// </summary>
        public Bounds GetArenaBounds()
        {
            Vector3 center = (Vector3)(arenaBoundsMin + arenaBoundsMax) * 0.5f;
            Vector3 size = (Vector3)(arenaBoundsMax - arenaBoundsMin);
            return new Bounds(center, size);
        }

        private void OnDrawGizmos()
        {
            if (!drawGizmos) return;

            // Draw arena bounds
            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            Vector3 center = (Vector3)(arenaBoundsMin + arenaBoundsMax) * 0.5f;
            Vector3 size = (Vector3)(arenaBoundsMax - arenaBoundsMin);
            Gizmos.DrawWireCube(center, size);

            // Draw walkable sample points in editor
            if (!Application.isPlaying || !isInitialized) return;

            float step = 0.5f;
            for (float x = arenaBoundsMin.x; x <= arenaBoundsMax.x; x += step)
            {
                for (float y = arenaBoundsMin.y; y <= arenaBoundsMax.y; y += step)
                {
                    Vector2 pos = new Vector2(x, y);
                    Gizmos.color = IsWalkable(pos)
                        ? new Color(0f, 1f, 0f, 0.1f)
                        : new Color(1f, 0f, 0f, 0.1f);
                    Gizmos.DrawCube(pos, Vector3.one * step * 0.8f);
                }
            }
        }
    }
}
