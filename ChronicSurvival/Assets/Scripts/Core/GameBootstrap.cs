using UnityEngine;

namespace ChronicSurvival.Core
{
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Manager Prefabs")]
        [SerializeField] private GameObject gameManagerPrefab;
        [SerializeField] private GameObject eventManagerPrefab;
        [SerializeField] private GameObject audioManagerPrefab;
        [SerializeField] private GameObject poolManagerPrefab;
        [SerializeField] private GameObject sceneLoaderPrefab;

        [Header("Settings")]
        [SerializeField] private bool autoInitialize = true;

        private void Awake()
        {
            if (autoInitialize)
            {
                InitializeManagers();
            }
        }

        [ContextMenu("Initialize Managers")]
        public void InitializeManagers()
        {
            Debug.Log("[GameBootstrap] Initializing managers...");

            if (GameManager.Instance == null && gameManagerPrefab != null)
                Instantiate(gameManagerPrefab);

            if (EventManager.Instance == null && eventManagerPrefab != null)
                Instantiate(eventManagerPrefab);

            if (AudioManager.Instance == null && audioManagerPrefab != null)
                Instantiate(audioManagerPrefab);

            if (PoolManager.Instance == null && poolManagerPrefab != null)
                Instantiate(poolManagerPrefab);

            if (SceneLoader.Instance == null && sceneLoaderPrefab != null)
                Instantiate(sceneLoaderPrefab);

            Debug.Log("[GameBootstrap] Managers initialized!");
        }
    }
}
