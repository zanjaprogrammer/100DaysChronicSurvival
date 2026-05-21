using UnityEngine;

namespace ChronicSurvival.Arena
{
    /// <summary>
    /// Sets up the arena background in the scene.
    /// Handles the SpriteRenderer for the background image and
    /// connects to the ArenaWalkableMask for boundary enforcement.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class ArenaSetup : MonoBehaviour
    {
        [Header("Background")]
        [SerializeField] private Sprite arenaSprite;
        [SerializeField] private int sortingOrder = -100;
        [SerializeField] private string sortingLayerName = "Default";

        [Header("Scale")]
        [Tooltip("If true, auto-scale the sprite to match the camera's orthographic view")]
        [SerializeField] private bool autoScaleToCamera = true;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            SetupBackground();
        }

        private void SetupBackground()
        {
            if (arenaSprite != null)
            {
                spriteRenderer.sprite = arenaSprite;
            }

            spriteRenderer.sortingOrder = sortingOrder;
            spriteRenderer.sortingLayerName = sortingLayerName;

            if (autoScaleToCamera)
            {
                ScaleToCamera();
            }
        }

        private void ScaleToCamera()
        {
            Camera cam = Camera.main;
            if (cam == null || spriteRenderer.sprite == null) return;

            float cameraHeight = cam.orthographicSize * 2f;
            float cameraWidth = cameraHeight * cam.aspect;

            float spriteWidth = spriteRenderer.sprite.bounds.size.x;
            float spriteHeight = spriteRenderer.sprite.bounds.size.y;

            float scaleX = cameraWidth / spriteWidth;
            float scaleY = cameraHeight / spriteHeight;

            // Use the larger scale to ensure the arena covers the screen
            float scale = Mathf.Max(scaleX, scaleY);
            transform.localScale = new Vector3(scale, scale, 1f);
        }

        /// <summary>
        /// Returns the world-space bounds of the arena background.
        /// </summary>
        public Bounds GetWorldBounds()
        {
            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {
                return spriteRenderer.bounds;
            }
            return new Bounds(Vector3.zero, new Vector3(19.2f, 10.8f, 0f));
        }
    }
}
