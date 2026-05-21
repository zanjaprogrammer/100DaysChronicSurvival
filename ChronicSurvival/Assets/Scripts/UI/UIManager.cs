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
        [SerializeField] private TopStatusBarUI topStatusBar;
        [SerializeField] private ActiveEventUI activeEventUI;

        private GameObject currentPanel;
        private GameState previousState = GameState.MainMenu;

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
                    HideAllPanels();
                    ShowPanel(mainMenuPanel);
                    break;

                case GameState.Battle:
                    HideAllPanels();
                    ShowPanel(battleHUDPanel);
                    break;

                case GameState.CardSelection:
                    // Card selection is shown as OVERLAY on top of battle HUD
                    // Don't hide battleHUD - show card selection on top
                    if (battleHUDPanel != null) battleHUDPanel.SetActive(true);
                    if (cardSelectionPanel != null) cardSelectionPanel.SetActive(true);
                    currentPanel = cardSelectionPanel;
                    break;

                case GameState.RandomEvent:
                    HideAllPanels();
                    ShowPanel(randomEventPanel);
                    break;

                case GameState.Paused:
                    // Pause is shown as overlay on top of whatever was showing
                    if (pausePanel != null) pausePanel.SetActive(true);
                    currentPanel = pausePanel;
                    break;

                case GameState.GameOver:
                    HideAllPanels();
                    ShowPanel(gameOverPanel);
                    break;

                case GameState.Victory:
                    HideAllPanels();
                    ShowPanel(victoryPanel);
                    break;
            }

            previousState = newState;
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
            if (panel != null)
            {
                panel.SetActive(true);
                currentPanel = panel;
            }
        }

        public void ShowBattleHUD()
        {
            HideAllPanels();
            ShowPanel(battleHUDPanel);
        }

        public void ShowCardSelection()
        {
            // Card selection overlays the battle HUD
            if (battleHUDPanel != null) battleHUDPanel.SetActive(true);
            if (cardSelectionPanel != null) cardSelectionPanel.SetActive(true);
            currentPanel = cardSelectionPanel;
        }

        public void ShowRandomEvent()
        {
            HideAllPanels();
            ShowPanel(randomEventPanel);
        }

        public void ShowPause()
        {
            if (pausePanel != null) pausePanel.SetActive(true);
            currentPanel = pausePanel;
        }

        public void HidePause()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResumeGame();
            }
        }

        /// <summary>
        /// Show an active event indicator on the battle HUD
        /// </summary>
        public void ShowActiveEvent(string eventName, string effectDescription)
        {
            if (activeEventUI != null)
            {
                activeEventUI.ShowEvent(eventName, effectDescription);
            }
        }

        /// <summary>
        /// Hide the active event indicator
        /// </summary>
        public void HideActiveEvent()
        {
            if (activeEventUI != null)
            {
                activeEventUI.HideEvent();
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
