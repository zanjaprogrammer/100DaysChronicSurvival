using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChronicSurvival.Core
{
    /// <summary>
    /// Main game manager - Singleton pattern
    /// Manages game state, initialization, and core game loop
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.MainMenu;
        [SerializeField] private int currentDay = 1;
        [SerializeField] private const int MAX_DAYS = 100;

        [Header("Debug")]
        [SerializeField] private bool debugMode = true;

        // Events
        public System.Action<GameState> OnGameStateChanged;
        public System.Action<int> OnDayChanged;
        public System.Action OnGameOver;
        public System.Action OnVictory;

        // Properties
        public GameState CurrentState => currentState;
        public int CurrentDay => currentDay;
        public bool IsGameActive => currentState == GameState.Battle || currentState == GameState.CardSelection;

        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Initialize();
        }

        private void Initialize()
        {
            if (debugMode) Debug.Log("[GameManager] Initializing...");

            // Initialize other managers
            EventManager.Instance?.Initialize();

            // Set initial state
            ChangeState(GameState.MainMenu);
        }

        private void Start()
        {
            if (debugMode) Debug.Log("[GameManager] Game started");
        }

        private void Update()
        {
            // Debug shortcuts
            if (debugMode)
            {
                if (Input.GetKeyDown(KeyCode.F1)) StartNewGame();
                if (Input.GetKeyDown(KeyCode.F2)) PauseGame();
                if (Input.GetKeyDown(KeyCode.F3)) ResumeGame();
                if (Input.GetKeyDown(KeyCode.Escape)) PauseGame();
            }
        }

        #region Game State Management

        public void ChangeState(GameState newState)
        {
            if (currentState == newState) return;

            GameState previousState = currentState;
            currentState = newState;

            if (debugMode) Debug.Log($"[GameManager] State changed: {previousState} -> {newState}");

            OnGameStateChanged?.Invoke(newState);
            EventManager.TriggerEvent("OnGameStateChanged", newState);

            HandleStateChange(newState);
        }

        private void HandleStateChange(GameState state)
        {
            switch (state)
            {
                case GameState.MainMenu:
                    Time.timeScale = 1f;
                    break;

                case GameState.Initializing:
                    InitializeNewGame();
                    break;

                case GameState.Battle:
                    Time.timeScale = 1f;
                    break;

                case GameState.CardSelection:
                    Time.timeScale = 1f;
                    break;

                case GameState.RandomEvent:
                    Time.timeScale = 1f;
                    break;

                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;

                case GameState.GameOver:
                    Time.timeScale = 0f;
                    HandleGameOver();
                    break;

                case GameState.Victory:
                    Time.timeScale = 0f;
                    HandleVictory();
                    break;
            }
        }

        #endregion

        #region Game Flow

        public void StartNewGame()
        {
            if (debugMode) Debug.Log("[GameManager] Starting new game...");

            currentDay = 1;
            ChangeState(GameState.Initializing);
        }

        private void InitializeNewGame()
        {
            // Reset all systems
            currentDay = 1;

            // Load game scene if not already loaded
            if (SceneManager.GetActiveScene().name != "GameScene")
            {
                SceneManager.LoadScene("GameScene");
            }

            // Start first battle
            ChangeState(GameState.Battle);
        }

        public void StartNextDay()
        {
            currentDay++;
            OnDayChanged?.Invoke(currentDay);
            EventManager.TriggerEvent("OnDayChanged", currentDay);

            if (debugMode) Debug.Log($"[GameManager] Day {currentDay} started");

            // Check victory condition
            if (currentDay > MAX_DAYS)
            {
                ChangeState(GameState.Victory);
                return;
            }

            // Start battle phase
            ChangeState(GameState.Battle);
        }

        public void EndBattle(bool victory)
        {
            if (!victory)
            {
                ChangeState(GameState.GameOver);
                return;
            }

            // Move to card selection
            ChangeState(GameState.CardSelection);
        }

        public void EndCardSelection()
        {
            // Move to random event
            ChangeState(GameState.RandomEvent);
        }

        public void EndRandomEvent()
        {
            // Check if any disease reached critical level
            // This will be implemented when we have disease system
            bool diseasesCritical = false; // TODO: Check disease meters

            if (diseasesCritical)
            {
                ChangeState(GameState.GameOver);
                return;
            }

            // Start next day
            StartNextDay();
        }

        #endregion

        #region Pause/Resume

        public void PauseGame()
        {
            if (currentState == GameState.Paused) return;
            if (currentState == GameState.MainMenu) return;
            if (currentState == GameState.GameOver) return;
            if (currentState == GameState.Victory) return;

            ChangeState(GameState.Paused);
        }

        public void ResumeGame()
        {
            if (currentState != GameState.Paused) return;

            // Return to previous state (default to battle)
            ChangeState(GameState.Battle);
        }

        #endregion

        #region Game Over / Victory

        private void HandleGameOver()
        {
            if (debugMode) Debug.Log("[GameManager] Game Over!");

            OnGameOver?.Invoke();
            EventManager.TriggerEvent("OnGameOver");

            // Show game over UI
            // TODO: Implement UI system
        }

        private void HandleVictory()
        {
            if (debugMode) Debug.Log("[GameManager] Victory! Survived 100 days!");

            OnVictory?.Invoke();
            EventManager.TriggerEvent("OnVictory");

            // Show victory UI
            // TODO: Implement UI system
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            StartNewGame();
        }

        public void QuitToMainMenu()
        {
            Time.timeScale = 1f;
            ChangeState(GameState.MainMenu);
            SceneManager.LoadScene("MainMenu");
        }

        public void QuitGame()
        {
            if (debugMode) Debug.Log("[GameManager] Quitting game...");

            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        #endregion

        #region Utility

        public float GetGameProgress()
        {
            return (float)currentDay / MAX_DAYS;
        }

        public int GetRemainingDays()
        {
            return MAX_DAYS - currentDay;
        }

        #endregion
    }

    public enum GameState
    {
        MainMenu,
        Initializing,
        Battle,
        CardSelection,
        RandomEvent,
        Paused,
        GameOver,
        Victory
    }
}
