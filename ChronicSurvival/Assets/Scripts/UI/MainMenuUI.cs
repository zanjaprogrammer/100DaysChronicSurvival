using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using ChronicSurvival.Core;

namespace ChronicSurvival.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button quitButton;

        [Header("Animation")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private float fadeInDuration = 0.8f;
        [SerializeField] private float titlePulseSpeed = 1.2f;

        private void Awake()
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        private void Start()
        {
            if (startButton != null) startButton.onClick.AddListener(OnStartButtonClicked);
            if (quitButton != null) quitButton.onClick.AddListener(OnQuitButtonClicked);

            StartCoroutine(FadeIn());
        }

        private void Update()
        {
            if (titleText != null)
            {
                float pulse = 0.85f + Mathf.Sin(Time.unscaledTime * titlePulseSpeed) * 0.15f;
                titleText.alpha = pulse;
            }
        }

        private IEnumerator FadeIn()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            yield return UIAnim.FadeCanvasGroup(canvasGroup, 1f, fadeInDuration, true);
        }

        private void OnStartButtonClicked()
        {
            if (canvasGroup != null)
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            StartCoroutine(StartGameTransition());
        }

        private IEnumerator StartGameTransition()
        {
            yield return UIAnim.FadeCanvasGroup(canvasGroup, 0f, 0.35f, true);
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartNewGame();
            }

            UIManager.Instance?.ConnectToGameManager();
            UIManager.Instance?.RefreshForCurrentState();
            gameObject.SetActive(false);
        }

        private void OnQuitButtonClicked()
        {
            GameManager.Instance?.QuitGame();
        }

        private void OnEnable()
        {
            if (canvasGroup != null && gameObject.activeInHierarchy)
            {
                StartCoroutine(FadeIn());
            }
        }
    }
}
