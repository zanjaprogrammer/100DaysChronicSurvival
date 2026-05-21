using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ChronicSurvival.Core;

namespace ChronicSurvival.UI
{
    /// <summary>
    /// Shows the currently active event effect at the top-right of the Battle HUD.
    /// e.g. "Gula Darah Tinggi" with "Enemy Spawn Rate +20%"
    /// </summary>
    public class ActiveEventUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private TextMeshProUGUI eventNameText;
        [SerializeField] private TextMeshProUGUI eventEffectText;

        [Header("Animation")]
        [SerializeField] private float fadeInDuration = 0.5f;

        private CanvasGroup canvasGroup;
        private string currentEventName;
        private string currentEventEffect;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        private void Start()
        {
            // Default hidden until event is set
            if (headerText != null)
                headerText.text = "EVENT AKTIF";
            
            // Initially hide if no event
            HideEvent();
        }

        public void ShowEvent(string eventName, string effectDescription)
        {
            currentEventName = eventName;
            currentEventEffect = effectDescription;

            if (eventNameText != null)
                eventNameText.text = eventName;
            if (eventEffectText != null)
                eventEffectText.text = effectDescription;

            gameObject.SetActive(true);

            if (canvasGroup != null)
            {
                StopAllCoroutines();
                StartCoroutine(FadeIn());
            }
        }

        public void HideEvent()
        {
            if (canvasGroup != null)
                canvasGroup.alpha = 0f;
        }

        private System.Collections.IEnumerator FadeIn()
        {
            float elapsed = 0f;
            canvasGroup.alpha = 0f;

            while (elapsed < fadeInDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
                yield return null;
            }

            canvasGroup.alpha = 1f;
        }
    }
}
