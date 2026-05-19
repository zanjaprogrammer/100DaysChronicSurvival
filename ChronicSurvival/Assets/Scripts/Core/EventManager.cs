using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChronicSurvival.Core
{
    /// <summary>
    /// Event system for decoupled communication between systems
    /// Singleton pattern
    /// </summary>
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance { get; private set; }

        // Event dictionaries
        private Dictionary<string, Action> eventDictionary = new Dictionary<string, Action>();
        private Dictionary<string, Action<object>> eventDictionaryWithParam = new Dictionary<string, Action<object>>();

        [Header("Debug")]
        [SerializeField] private bool debugMode = true;
        [SerializeField] private bool logAllEvents = false;

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
        }

        public void Initialize()
        {
            if (debugMode) Debug.Log("[EventManager] Initialized");
        }

        #region Event Registration (No Parameters)

        public static void StartListening(string eventName, Action listener)
        {
            if (Instance == null) return;

            Action thisEvent;
            if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent += listener;
                Instance.eventDictionary[eventName] = thisEvent;
            }
            else
            {
                thisEvent += listener;
                Instance.eventDictionary.Add(eventName, thisEvent);
            }

            if (Instance.debugMode && Instance.logAllEvents)
                Debug.Log($"[EventManager] Listener added: {eventName}");
        }

        public static void StopListening(string eventName, Action listener)
        {
            if (Instance == null) return;

            Action thisEvent;
            if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent -= listener;
                Instance.eventDictionary[eventName] = thisEvent;

                if (Instance.debugMode && Instance.logAllEvents)
                    Debug.Log($"[EventManager] Listener removed: {eventName}");
            }
        }

        public static void TriggerEvent(string eventName)
        {
            if (Instance == null) return;

            Action thisEvent;
            if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent?.Invoke();

                if (Instance.debugMode && Instance.logAllEvents)
                    Debug.Log($"[EventManager] Event triggered: {eventName}");
            }
        }

        #endregion

        #region Event Registration (With Parameters)

        public static void StartListening(string eventName, Action<object> listener)
        {
            if (Instance == null) return;

            Action<object> thisEvent;
            if (Instance.eventDictionaryWithParam.TryGetValue(eventName, out thisEvent))
            {
                thisEvent += listener;
                Instance.eventDictionaryWithParam[eventName] = thisEvent;
            }
            else
            {
                thisEvent += listener;
                Instance.eventDictionaryWithParam.Add(eventName, thisEvent);
            }

            if (Instance.debugMode && Instance.logAllEvents)
                Debug.Log($"[EventManager] Listener added (with param): {eventName}");
        }

        public static void StopListening(string eventName, Action<object> listener)
        {
            if (Instance == null) return;

            Action<object> thisEvent;
            if (Instance.eventDictionaryWithParam.TryGetValue(eventName, out thisEvent))
            {
                thisEvent -= listener;
                Instance.eventDictionaryWithParam[eventName] = thisEvent;

                if (Instance.debugMode && Instance.logAllEvents)
                    Debug.Log($"[EventManager] Listener removed (with param): {eventName}");
            }
        }

        public static void TriggerEvent(string eventName, object param)
        {
            if (Instance == null) return;

            Action<object> thisEvent;
            if (Instance.eventDictionaryWithParam.TryGetValue(eventName, out thisEvent))
            {
                thisEvent?.Invoke(param);

                if (Instance.debugMode && Instance.logAllEvents)
                    Debug.Log($"[EventManager] Event triggered (with param): {eventName} | Param: {param}");
            }
        }

        #endregion

        #region Utility

        public static void ClearAllListeners()
        {
            if (Instance == null) return;

            Instance.eventDictionary.Clear();
            Instance.eventDictionaryWithParam.Clear();

            if (Instance.debugMode)
                Debug.Log("[EventManager] All listeners cleared");
        }

        public static void ClearListeners(string eventName)
        {
            if (Instance == null) return;

            if (Instance.eventDictionary.ContainsKey(eventName))
                Instance.eventDictionary.Remove(eventName);

            if (Instance.eventDictionaryWithParam.ContainsKey(eventName))
                Instance.eventDictionaryWithParam.Remove(eventName);

            if (Instance.debugMode)
                Debug.Log($"[EventManager] Listeners cleared for: {eventName}");
        }

        public static int GetListenerCount(string eventName)
        {
            if (Instance == null) return 0;

            int count = 0;

            if (Instance.eventDictionary.ContainsKey(eventName))
            {
                var del = Instance.eventDictionary[eventName];
                count += del?.GetInvocationList().Length ?? 0;
            }

            if (Instance.eventDictionaryWithParam.ContainsKey(eventName))
            {
                var del = Instance.eventDictionaryWithParam[eventName];
                count += del?.GetInvocationList().Length ?? 0;
            }

            return count;
        }

        #endregion

        private void OnDestroy()
        {
            ClearAllListeners();
        }
    }

    #region Common Event Names (Constants)

    public static class GameEvents
    {
        // Game State
        public const string GAME_STARTED = "OnGameStarted";
        public const string GAME_PAUSED = "OnGamePaused";
        public const string GAME_RESUMED = "OnGameResumed";
        public const string GAME_OVER = "OnGameOver";
        public const string VICTORY = "OnVictory";

        // Round/Day
        public const string DAY_STARTED = "OnDayStarted";
        public const string DAY_ENDED = "OnDayEnded";
        public const string BATTLE_STARTED = "OnBattleStarted";
        public const string BATTLE_ENDED = "OnBattleEnded";

        // Card Selection
        public const string CARD_SELECTED = "OnCardSelected";
        public const string CARD_APPLIED = "OnCardApplied";

        // Random Event
        public const string RANDOM_EVENT_TRIGGERED = "OnRandomEventTriggered";
        public const string RANDOM_EVENT_COMPLETED = "OnRandomEventCompleted";

        // Body Components
        public const string COMPONENT_CHANGED = "OnComponentChanged";
        public const string COMPONENT_CRITICAL = "OnComponentCritical";

        // Disease
        public const string DISEASE_PROGRESSED = "OnDiseaseProgressed";
        public const string DISEASE_STAGE_CHANGED = "OnDiseaseStageChanged";
        public const string DISEASE_CRITICAL = "OnDiseaseCritical";

        // Combat
        public const string UNIT_SPAWNED = "OnUnitSpawned";
        public const string UNIT_DIED = "OnUnitDied";
        public const string ENEMY_SPAWNED = "OnEnemySpawned";
        public const string ENEMY_DIED = "OnEnemyDied";
        public const string NODE_DESTROYED = "OnNodeDestroyed";
    }

    #endregion
}
