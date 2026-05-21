using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ChronicSurvival.Core;

namespace ChronicSurvival.UI
{
    /// <summary>
    /// Displays the round number and timer at the top-left of the Battle HUD.
    /// Shows "RONDE X" with a timer below it.
    /// Also manages the pause button.
    /// </summary>
    public class DayCounterUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI dayText;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Image progressBar;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private Button pauseButton;

        [Header("Animation")]
        [SerializeField] private bool animateOnDayChange = true;
        [SerializeField] private float animationDuration = 0.5f;

        private int currentDay = 1;
        private const int MAX_DAYS = 100;
        private float battleTimer = 0f;
        private bool isTimerRunning = false;

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnDayChanged += OnDayChanged;
                GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
                UpdateDisplay(GameManager.Instance.CurrentDay);
            }

            if (pauseButton != null)
            {
                pauseButton.onClick.AddListener(OnPauseClicked);
            }
        }

        private void Update()
        {
            if (isTimerRunning)
            {
                battleTimer += Time.deltaTime;
                UpdateTimerDisplay();
            }
        }

        private void OnGameStateChanged(GameState newState)
        {
            if (newState == GameState.Battle)
            {
                battleTimer = 0f;
                isTimerRunning = true;
            }
            else
            {
                isTimerRunning = false;
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
                dayText.text = $"RONDE {day}";
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

        private void UpdateTimerDisplay()
        {
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(battleTimer / 60f);
                int seconds = Mathf.FloorToInt(battleTimer % 60f);
                timerText.text = $"{minutes:D2}:{seconds:D2}";
            }
        }

        private void OnPauseClicked()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PauseGame();
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
                GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
            }
        }
    }
}
