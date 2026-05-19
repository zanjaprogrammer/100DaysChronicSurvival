using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ChronicSurvival.Core;

namespace ChronicSurvival.UI
{
    public class DayCounterUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI dayText;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Image progressBar;

        [Header("Animation")]
        [SerializeField] private bool animateOnDayChange = true;
        [SerializeField] private float animationDuration = 0.5f;

        private int currentDay = 1;
        private const int MAX_DAYS = 100;

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnDayChanged += OnDayChanged;
                UpdateDisplay(GameManager.Instance.CurrentDay);
            }
        }

        private void OnDayChanged(int newDay)
        {
            if (animateOnDayChange)
            {
                StartCoroutine(AnimateDayChange(currentDay, newDay));
            }
            else
            {
                UpdateDisplay(newDay);
            }

            currentDay = newDay;
        }

        private void UpdateDisplay(int day)
        {
            if (dayText != null)
            {
                dayText.text = $"Day {day}/{MAX_DAYS}";
            }

            if (progressText != null)
            {
                float progress = (float)day / MAX_DAYS * 100f;
                progressText.text = $"{progress:F0}%";
            }

            if (progressBar != null)
            {
                progressBar.fillAmount = (float)day / MAX_DAYS;
            }
        }

        private System.Collections.IEnumerator AnimateDayChange(int from, int to)
        {
            float elapsed = 0f;

            while (elapsed < animationDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / animationDuration;
                int displayDay = Mathf.RoundToInt(Mathf.Lerp(from, to, t));
                UpdateDisplay(displayDay);
                yield return null;
            }

            UpdateDisplay(to);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnDayChanged -= OnDayChanged;
            }
        }
    }
}
