using UnityEngine;
using ChronicSurvival.Core;

namespace ChronicSurvival.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI Panels")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject battleHUDPanel;
        [SerializeField] private GameObject cardSelectionPanel;
        [SerializeField] private GameObject randomEventPanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private GameObject pausePanel;

        [Header("HUD Components")]
        [SerializeField] private DayCounterUI dayCounter;
        [SerializeField] private BodyStatsPanel bodyStatsPanel;
        [SerializeField] private DiseaseProgressPanel diseaseProgressPanel;

        private GameObject currentPanel;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeUI();
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
            }
        }

        private void InitializeUI()
        {
            HideAllPanels();
            ShowPanel(mainMenuPanel);
        }

        private void OnGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.MainMenu:
                    ShowPanel(mainMenuPanel);
                    break;

                case GameState.Battle:
                    ShowPanel(battleHUDPanel);
                    break;

                case GameState.CardSelection:
                    ShowPanel(cardSelectionPanel);
                    break;

                case GameState.RandomEvent:
                    ShowPanel(randomEventPanel);
                    break;

                case GameState.Paused:
                    ShowPanel(pausePanel);
                    break;

                case GameState.GameOver:
                    ShowPanel(gameOverPanel);
                    break;

                case GameState.Victory:
                    ShowPanel(victoryPanel);
                    break;
            }
        }

        private void HideAllPanels()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (battleHUDPanel != null) battleHUDPanel.SetActive(false);
            if (cardSelectionPanel != null) cardSelectionPanel.SetActive(false);
            if (randomEventPanel != null) randomEventPanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (victoryPanel != null) victoryPanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false);
        }

        private void ShowPanel(GameObject panel)
        {
            HideAllPanels();

            if (panel != null)
            {
                panel.SetActive(true);
                currentPanel = panel;
            }
        }

        public void ShowBattleHUD()
        {
            ShowPanel(battleHUDPanel);
        }

        public void ShowCardSelection()
        {
            ShowPanel(cardSelectionPanel);
        }

        public void ShowRandomEvent()
        {
            ShowPanel(randomEventPanel);
        }

        public void ShowPause()
        {
            ShowPanel(pausePanel);
        }

        public void HidePause()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResumeGame();
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
            }
        }
    }
}
