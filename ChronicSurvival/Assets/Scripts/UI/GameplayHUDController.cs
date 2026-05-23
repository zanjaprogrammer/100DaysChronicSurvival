using System.Collections;
using UnityEngine;

namespace ChronicSurvival.UI
{
    /// <summary>
    /// Dims gameplay HUD during card selection; restores when dismissed.
    /// </summary>
    public class GameplayHUDController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup hudCanvasGroup;
        [SerializeField] private float dimmedAlpha = 0.22f;
        [SerializeField] private float normalAlpha = 1f;
        [SerializeField] private float fadeDuration = 0.3f;

        private Coroutine fadeRoutine;
        private bool isDimmed;

        public bool IsDimmed => isDimmed;

        public void SetDimmed(bool dimmed)
        {
            if (isDimmed == dimmed) return;
            isDimmed = dimmed;

            if (fadeRoutine != null)
            {
                StopCoroutine(fadeRoutine);
            }

            float target = dimmed ? dimmedAlpha : normalAlpha;
            fadeRoutine = StartCoroutine(UIAnim.FadeCanvasGroup(hudCanvasGroup, target, fadeDuration, true));
        }

        public void SetInteractable(bool interactable)
        {
            if (hudCanvasGroup != null)
            {
                hudCanvasGroup.interactable = interactable;
                hudCanvasGroup.blocksRaycasts = interactable;
            }
        }
    }
}
