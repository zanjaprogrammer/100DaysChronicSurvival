using UnityEngine;
using ChronicSurvival.Cards;
using ChronicSurvival.Battle;
using ChronicSurvival.Disease;
using ChronicSurvival.BodyComponents;
using ChronicSurvival.UI;

namespace ChronicSurvival.Core
{
    /// <summary>
    /// Ensures core managers exist when entering play mode.
    /// Reuses scene-placed managers (with prefabs wired) instead of spawning empty duplicates.
    /// </summary>
    public static class GameSystemsBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureManagers()
        {
            GameObject root = GameObject.Find("_Managers");
            if (root == null)
            {
                root = new GameObject("_Managers");
                Object.DontDestroyOnLoad(root);
            }

            // Always on DDOL root
            EnsureOnRoot<GameManager>(root);
            EnsureOnRoot<EventManager>(root);

            // Prefer existing scene instances (BattleManager has prefab refs on SampleScene)
            EnsureInScene<BodyComponentManager>(root);
            EnsureInScene<CardManager>(root);
            EnsureInScene<CardBuffManager>(root);
            EnsureInScene<DiseaseManager>(root);
            EnsureInScene<BattleManager>(root);

            Canvas canvas = GameplayUICreator.EnsureCanvas();
            if (canvas.transform.Find("MainMenuPanel") == null || canvas.transform.Find("BattleHUDPanel") == null)
            {
                GameplayUICreator.BuildAll(canvas.transform);
            }

            GameManager.NotifyInstanceReady();

            BattleManager bm = Object.FindFirstObjectByType<BattleManager>(FindObjectsInactive.Include);
            if (bm != null) bm.ConnectToGameManager();

            UIManager ui = Object.FindFirstObjectByType<UIManager>(FindObjectsInactive.Include);
            if (ui != null)
            {
                ui.ConnectToGameManager();
                ui.RefreshForCurrentState();
            }
        }

        /// <summary>Only for managers that should live on _Managers.</summary>
        private static T EnsureOnRoot<T>(GameObject root) where T : Component
        {
            T onRoot = root.GetComponent<T>();
            if (onRoot != null) return onRoot;

            T inScene = Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
            if (inScene != null)
            {
                return inScene;
            }

            return root.AddComponent<T>();
        }

        /// <summary>Use scene object if present; otherwise add to _Managers.</summary>
        private static T EnsureInScene<T>(GameObject root) where T : Component
        {
            T inScene = Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
            if (inScene != null)
            {
                return inScene;
            }

            T onRoot = root.GetComponent<T>();
            if (onRoot != null) return onRoot;

            return root.AddComponent<T>();
        }
    }
}
