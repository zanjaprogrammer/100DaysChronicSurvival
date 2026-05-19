using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace ChronicSurvival.Core
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        [Header("Loading")]
        [SerializeField] private bool useLoadingScreen = true;
        [SerializeField] private float minimumLoadTime = 1f;

        [Header("Debug")]
        [SerializeField] private bool debugMode = true;

        public System.Action<float> OnLoadProgress;
        public System.Action OnLoadComplete;

        private bool isLoading = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void LoadScene(string sceneName)
        {
            if (isLoading) return;

            if (debugMode) Debug.Log($"[SceneLoader] Loading scene: {sceneName}");

            StartCoroutine(LoadSceneAsync(sceneName));
        }

        public void LoadScene(int sceneIndex)
        {
            if (isLoading) return;

            if (debugMode) Debug.Log($"[SceneLoader] Loading scene index: {sceneIndex}");

            StartCoroutine(LoadSceneAsync(sceneIndex));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            isLoading = true;
            float startTime = Time.realtimeSinceStartup;

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                OnLoadProgress?.Invoke(progress);

                if (operation.progress >= 0.9f)
                {
                    float elapsedTime = Time.realtimeSinceStartup - startTime;
                    if (elapsedTime >= minimumLoadTime)
                    {
                        operation.allowSceneActivation = true;
                    }
                }

                yield return null;
            }

            OnLoadComplete?.Invoke();
            isLoading = false;

            if (debugMode) Debug.Log($"[SceneLoader] Scene loaded: {sceneName}");
        }

        private IEnumerator LoadSceneAsync(int sceneIndex)
        {
            isLoading = true;
            float startTime = Time.realtimeSinceStartup;

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
            operation.allowSceneActivation = false;

            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                OnLoadProgress?.Invoke(progress);

                if (operation.progress >= 0.9f)
                {
                    float elapsedTime = Time.realtimeSinceStartup - startTime;
                    if (elapsedTime >= minimumLoadTime)
                    {
                        operation.allowSceneActivation = true;
                    }
                }

                yield return null;
            }

            OnLoadComplete?.Invoke();
            isLoading = false;

            if (debugMode) Debug.Log($"[SceneLoader] Scene loaded: {sceneIndex}");
        }

        public void ReloadCurrentScene()
        {
            LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
