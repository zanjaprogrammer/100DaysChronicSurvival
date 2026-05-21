using UnityEditor;
using UnityEngine;
using System.IO;

namespace ChronicSurvival.Arena
{
    public static class WalkableMaskBatchGenerator
    {
        [MenuItem("Tools/Generate Walkable Mask Batch")]
        public static void GenerateMask()
        {
            string srcPath = "Assets/Art/Background/ArenaBranched.png";
            Texture2D sourceArenaImage = AssetDatabase.LoadAssetAtPath<Texture2D>(srcPath);
            if (sourceArenaImage == null)
            {
                Debug.LogError("[WalkableMaskBatchGenerator] Source image not found at: " + srcPath);
                return;
            }

            int outputWidth = 512;
            int outputHeight = 288;
            float brightnessThreshold = 0.25f;
            float redWeight = 0.6f;
            int dilateIterations = 3;

            // Make sure texture is readable
            string assetPath = AssetDatabase.GetAssetPath(sourceArenaImage);
            TextureImporter textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (textureImporter != null && !textureImporter.isReadable)
            {
                textureImporter.isReadable = true;
                textureImporter.SaveAndReimport();
            }

            Color[] sourcePixels = sourceArenaImage.GetPixels();
            int srcWidth = sourceArenaImage.width;
            int srcHeight = sourceArenaImage.height;

            bool[,] walkable = new bool[outputWidth, outputHeight];

            for (int y = 0; y < outputHeight; y++)
            {
                for (int x = 0; x < outputWidth; x++)
                {
                    float u = (float)x / outputWidth;
                    float v = (float)y / outputHeight;

                    int srcX = Mathf.Clamp(Mathf.FloorToInt(u * srcWidth), 0, srcWidth - 1);
                    int srcY = Mathf.Clamp(Mathf.FloorToInt(v * srcHeight), 0, srcHeight - 1);

                    Color pixel = sourcePixels[srcY * srcWidth + srcX];
                    float score = pixel.r * redWeight + pixel.grayscale * (1f - redWeight);
                    walkable[x, y] = score >= brightnessThreshold;
                }
            }

            for (int iter = 0; iter < dilateIterations; iter++)
            {
                bool[,] dilated = new bool[outputWidth, outputHeight];
                System.Array.Copy(walkable, dilated, walkable.Length);

                for (int y = 1; y < outputHeight - 1; y++)
                {
                    for (int x = 1; x < outputWidth - 1; x++)
                    {
                        if (walkable[x, y]) continue;

                        if (walkable[x - 1, y] || walkable[x + 1, y] ||
                            walkable[x, y - 1] || walkable[x, y + 1])
                        {
                            dilated[x, y] = true;
                        }
                    }
                }
                walkable = dilated;
            }

            Texture2D mask = new Texture2D(outputWidth, outputHeight, TextureFormat.RGBA32, false);
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

            byte[] pngData = mask.EncodeToPNG();
            string outputPath = "Assets/Art/Background/ArenaBranched_WalkableMask.png";
            string absoluteOutputPath = Path.Combine(Application.dataPath, "../", outputPath);
            File.WriteAllBytes(absoluteOutputPath, pngData);

            Debug.Log($"[WalkableMaskBatchGenerator] Saved mask to {outputPath}");
            AssetDatabase.Refresh();

            // Set the import settings for the mask to ensure it is readable and Point-filtered
            AssetDatabase.ImportAsset(outputPath);
            TextureImporter maskImporter = AssetImporter.GetAtPath(outputPath) as TextureImporter;
            if (maskImporter != null)
            {
                maskImporter.isReadable = true;
                maskImporter.textureCompression = TextureImporterCompression.Uncompressed;
                maskImporter.filterMode = FilterMode.Point;
                maskImporter.SaveAndReimport();
            }
        }
    }
}
