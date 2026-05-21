using UnityEngine;

namespace ChronicSurvival.Arena
{
    /// <summary>
    /// Generates a walkable mask texture from the arena background image.
    /// This is an Editor utility — run it once to create the mask,
    /// then assign the output to ArenaWalkableMask.
    /// 
    /// How it works:
    /// The arena background has bright red blood vessels and dark tissue.
    /// Blood vessels (bright areas) = walkable.
    /// Dark corners/tissue = non-walkable.
    /// </summary>
    public class WalkableMaskGenerator : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private Texture2D sourceArenaImage;

        [Header("Generation Settings")]
        [Tooltip("Pixels with brightness above this become walkable (white)")]
        [SerializeField] private float brightnessThreshold = 0.25f;
        
        [Tooltip("How much to weigh the red channel (blood vessels are red)")]
        [SerializeField] private float redWeight = 0.6f;
        
        [Tooltip("Output resolution (lower = faster but less precise)")]
        [SerializeField] private int outputWidth = 512;
        [SerializeField] private int outputHeight = 288;

        [Header("Post Processing")]
        [Tooltip("Number of times to dilate walkable areas (expands the walkable zone)")]
        [SerializeField] private int dilateIterations = 3;

        [Header("Output")]
        [SerializeField] private Texture2D generatedMask;

        /// <summary>
        /// Call this from a custom editor button or context menu to generate the mask.
        /// </summary>
        [ContextMenu("Generate Walkable Mask")]
        public void GenerateMask()
        {
            if (sourceArenaImage == null)
            {
                Debug.LogError("[WalkableMaskGenerator] No source image assigned!");
                return;
            }

            // Create output texture
            Texture2D mask = new Texture2D(outputWidth, outputHeight, TextureFormat.RGBA32, false);
            mask.filterMode = FilterMode.Bilinear;

            Color[] sourcePixels;
            try
            {
                sourcePixels = sourceArenaImage.GetPixels();
            }
            catch (UnityException)
            {
                Debug.LogError("[WalkableMaskGenerator] Source texture is not readable! " +
                    "Enable Read/Write in import settings.");
                return;
            }

            int srcWidth = sourceArenaImage.width;
            int srcHeight = sourceArenaImage.height;

            // First pass: determine walkable based on brightness/color
            bool[,] walkable = new bool[outputWidth, outputHeight];

            for (int y = 0; y < outputHeight; y++)
            {
                for (int x = 0; x < outputWidth; x++)
                {
                    // Sample the source image
                    float u = (float)x / outputWidth;
                    float v = (float)y / outputHeight;

                    int srcX = Mathf.Clamp(Mathf.FloorToInt(u * srcWidth), 0, srcWidth - 1);
                    int srcY = Mathf.Clamp(Mathf.FloorToInt(v * srcHeight), 0, srcHeight - 1);

                    Color pixel = sourcePixels[srcY * srcWidth + srcX];

                    // Weight red channel more heavily (blood vessels are red)
                    float score = pixel.r * redWeight + pixel.grayscale * (1f - redWeight);

                    walkable[x, y] = score >= brightnessThreshold;
                }
            }

            // Dilate: expand walkable areas slightly (to give cells room to move)
            for (int iter = 0; iter < dilateIterations; iter++)
            {
                bool[,] dilated = new bool[outputWidth, outputHeight];
                System.Array.Copy(walkable, dilated, walkable.Length);

                for (int y = 1; y < outputHeight - 1; y++)
                {
                    for (int x = 1; x < outputWidth - 1; x++)
                    {
                        if (walkable[x, y]) continue;

                        // If any neighbor is walkable, this pixel becomes walkable too
                        if (walkable[x - 1, y] || walkable[x + 1, y] ||
                            walkable[x, y - 1] || walkable[x, y + 1])
                        {
                            dilated[x, y] = true;
                        }
                    }
                }

                walkable = dilated;
            }

            // Write to texture
            Color[] outputPixels = new Color[outputWidth * outputHeight];
            for (int y = 0; y < outputHeight; y++)
            {
                for (int x = 0; x < outputWidth; x++)
                {
                    outputPixels[y * outputWidth + x] = walkable[x, y] ? Color.white : Color.black;
                }
            }

            mask.SetPixels(outputPixels);
            mask.Apply();

            generatedMask = mask;

            // Save as PNG
            byte[] pngData = mask.EncodeToPNG();
            string outputPath = "Assets/Art/Background/ArenaBranched_WalkableMask.png";
            System.IO.File.WriteAllBytes(
                System.IO.Path.Combine(Application.dataPath, "../", outputPath), pngData);

            Debug.Log($"[WalkableMaskGenerator] Mask saved to {outputPath}");
            Debug.Log($"[WalkableMaskGenerator] Walkable pixels: " +
                $"{CountWalkable(walkable)} / {outputWidth * outputHeight}");

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        private int CountWalkable(bool[,] walkable)
        {
            int count = 0;
            foreach (bool w in walkable)
            {
                if (w) count++;
            }
            return count;
        }
    }
}
