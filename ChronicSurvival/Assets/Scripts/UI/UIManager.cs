using UnityEngine;
using UnityEngine.UI;
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
        [SerializeField] private BodyStatsDrawerUI bodyStatsDrawer;
        [SerializeField] private DiseaseProgressPanel diseaseProgressPanel;
        [SerializeField] private TopStatusBarUI topStatusBar;
        [SerializeField] private ActiveEventUI activeEventUI;
        [SerializeField] private GameplayHUDController gameplayHUDController;

        private GameState previousState = GameState.MainMenu;
        private bool handlingStateChange = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            InitializeUI();

        }

        private void OnEnable()
        {
            ConnectToGameManager();
            GameManager.OnInstanceReady += ConnectToGameManager;
        }

        private void Start()
        {
            ConnectToGameManager();
            RefreshForCurrentState();
        }

        public void ConnectToGameManager()
        {
            if (GameManager.Instance == null) return;

            GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
            GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
            OnGameStateChanged(GameManager.Instance.CurrentState);
        }

        private void OnDisable()
        {
            GameManager.OnInstanceReady -= ConnectToGameManager;
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
            }
        }

        private void InitializeUI()
        {
            HideAllPanels();
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        }

        /// <summary>
        /// Called by GameplayUIInstaller after runtime/editor UI build.
        /// </summary>
        public void BindPanels(
            GameObject mainMenu,
            GameObject battleHud,
            GameObject cardSelection,
            GameObject randomEvent,
            GameObject gameOver,
            GameObject victory,
            GameObject pause)
        {
            mainMenuPanel = mainMenu;
            battleHUDPanel = battleHud;
            cardSelectionPanel = cardSelection;
            randomEventPanel = randomEvent;
            gameOverPanel = gameOver;
            victoryPanel = victory;
            pausePanel = pause;

            if (battleHud != null)
            {
                dayCounter = battleHud.GetComponentInChildren<DayCounterUI>(true);
                bodyStatsDrawer = battleHud.GetComponentInChildren<BodyStatsDrawerUI>(true);
                diseaseProgressPanel = battleHud.GetComponentInChildren<DiseaseProgressPanel>(true);
                topStatusBar = battleHud.GetComponentInChildren<TopStatusBarUI>(true);
                activeEventUI = battleHud.GetComponentInChildren<ActiveEventUI>(true);
                gameplayHUDController = battleHud.GetComponent<GameplayHUDController>();
            }
        }

        public void RefreshForCurrentState()
        {
            if (GameManager.Instance != null)
            {
                OnGameStateChanged(GameManager.Instance.CurrentState);
            }
            else
            {
                InitializeUI();
            }
        }

        private void OnGameStateChanged(GameState newState)
        {
            if (handlingStateChange) return;
            handlingStateChange = true;

            try
            {
                EnsurePanelsBound();

            switch (newState)
            {
                case GameState.Initializing:
                    HideAllPanels();
                    break;

                case GameState.MainMenu:
                    SetGameplayDimmed(false);
                    HideAllPanels();
                    if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
                    break;

                case GameState.Battle:
                    SetGameplayDimmed(false);
                    HideAllPanels();
                    if (battleHUDPanel != null) battleHUDPanel.SetActive(true);
                    if (gameplayHUDController != null) gameplayHUDController.SetInteractable(true);
                    break;

                case GameState.CardSelection:
                    if (battleHUDPanel != null) battleHUDPanel.SetActive(true);
                    if (cardSelectionPanel != null) cardSelectionPanel.SetActive(true);
                    SetGameplayDimmed(true);
                    if (gameplayHUDController != null) gameplayHUDController.SetInteractable(false);
                    break;

                case GameState.RandomEvent:
                    SetGameplayDimmed(false);
                    HideAllPanels();
                    if (randomEventPanel != null) randomEventPanel.SetActive(true);
                    break;

                case GameState.Paused:
                    if (battleHUDPanel != null) battleHUDPanel.SetActive(true);
                    if (pausePanel != null) pausePanel.SetActive(true);
                    break;

                case GameState.GameOver:
                    SetGameplayDimmed(false);
                    HideAllPanels();
                    if (gameOverPanel != null) gameOverPanel.SetActive(true);
                    break;

                case GameState.Victory:
                    SetGameplayDimmed(false);
                    HideAllPanels();
                    if (victoryPanel != null) victoryPanel.SetActive(true);
                    break;
            }

            previousState = newState;
            }
            finally
            {
                handlingStateChange = false;
            }
        }

        private void SetGameplayDimmed(bool dimmed)
        {
            if (gameplayHUDController != null)
            {
                gameplayHUDController.SetDimmed(dimmed);
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

        public void ShowActiveEvent(string eventName, string effectDescription)
        {
            activeEventUI?.ShowEvent(eventName, effectDescription);
        }

        public void HideActiveEvent()
        {
            activeEventUI?.HideEvent();
        }

        public void HidePause()
        {
            if (pausePanel != null) pausePanel.SetActive(false);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResumeGame();
            }
        }

        private void EnsurePanelsBound()
        {
            Transform root = GetUIRoot();
            Transform battleHud = root.Find("BattleHUDPanel");

            bool rebuilt = false;
            if (root.Find("MainMenuPanel") == null || battleHud == null || battleHud.Find("RoundPanel") == null)
            {
                GameplayUICreator.BuildAll(root);
                battleHud = root.Find("BattleHUDPanel");
                rebuilt = true;
            }

            if (!rebuilt && mainMenuPanel != null && battleHUDPanel != null) return;

            BindPanels(
                FindPanel(root, "MainMenuPanel"),
                FindPanel(root, "BattleHUDPanel"),
                FindPanel(root, "CardSelectionPanel"),
                FindPanel(root, "RandomEventPanel"),
                FindPanel(root, "GameOverPanel"),
                FindPanel(root, "VictoryPanel"),
                FindPanel(root, "PausePanel"));
        }

        private Transform GetUIRoot()
        {
            Canvas ownCanvas = GetComponent<Canvas>();
            if (ownCanvas != null) return ownCanvas.transform;

            Canvas parentCanvas = GetComponentInParent<Canvas>(true);
            if (parentCanvas != null) return parentCanvas.transform;

            return GameplayUICreator.EnsureCanvas().transform;
        }

        private static GameObject FindPanel(Transform root, string name)
        {
            Transform t = root.Find(name);
            return t != null ? t.gameObject : null;
        }
    }
}
