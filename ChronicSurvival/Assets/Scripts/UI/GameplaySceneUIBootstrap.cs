using UnityEngine;
using ChronicSurvival.Core;

namespace ChronicSurvival.UI
{
    /// <summary>
    /// Scene-placed fallback: guarantees UI is built after all Awakes (fixes empty Canvas / zero-size rect).
    /// Add to any active scene object (e.g. EventSystem) if UI does not appear on Play.
    /// </summary>
    [DefaultExecutionOrder(-250)]
    public class GameplaySceneUIBootstrap : MonoBehaviour
    {
        [SerializeField] private bool logSteps = true;

        private void Awake()
        {
            EnsureUI();
        }

        private void Start()
        {
            // Second pass — GameManager may appear on frame 1 via GameSystemsBootstrap
            EnsureUI();
            UIManager.Instance?.ConnectToGameManager();
            UIManager.Instance?.RefreshForCurrentState();
        }

        private void EnsureUI()
        {
            Canvas canvas = GameplayUICreator.EnsureCanvas();
            if (canvas == null) return;

            Transform root = canvas.transform;
            Transform battle = root.Find("BattleHUDPanel");
            bool needsBuild = root.Find("MainMenuPanel") == null
                || battle == null
                || battle.Find("RoundPanel") == null;

            if (needsBuild)
            {
                GameplayUICreator.BuildAll(root);
                if (logSteps)
                {
                    Debug.Log("[GameplaySceneUIBootstrap] UI v2 dibangun dari scene bootstrap.");
                }
            }

            if (canvas.GetComponent<GameplayUIInstaller>() == null)
            {
                canvas.gameObject.AddComponent<GameplayUIInstaller>();
            }

            if (canvas.GetComponent<UIManager>() == null)
            {
                canvas.gameObject.AddComponent<UIManager>();
            }

            var ui = canvas.GetComponent<UIManager>();
            ui?.ConnectToGameManager();
            ui?.RefreshForCurrentState();
        }
    }
}
