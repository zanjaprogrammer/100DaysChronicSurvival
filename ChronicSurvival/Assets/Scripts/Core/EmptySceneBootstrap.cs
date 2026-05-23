using UnityEngine;
using ChronicSurvival.Arena;
using ChronicSurvival.UI;
using ChronicSurvival.Battle;
using ChronicSurvival.BodyComponents;
using ChronicSurvival.Disease;
using ChronicSurvival.Cards;

namespace ChronicSurvival.Core
{
    [DefaultExecutionOrder(-1000)]
    public class EmptySceneBootstrap : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EnsureBootstrapObject()
        {
            if (FindFirstObjectByType<EmptySceneBootstrap>(FindObjectsInactive.Include) != null) return;

            GameObject go = new GameObject("EmptySceneBootstrap");
            DontDestroyOnLoad(go);
            go.AddComponent<EmptySceneBootstrap>();
        }

        private void Awake()
        {
            EnsureCamera();
            EnsureCoreManagers();
            EnsureArena();
            EnsureUI();
        }

        private void Start()
        {
            EnsureUI();
            UIManager.Instance?.ConnectToGameManager();
            UIManager.Instance?.RefreshForCurrentState();
            GameManager.NotifyInstanceReady();
        }

        private void EnsureCamera()
        {
            Camera cam = Camera.main;
            if (cam == null) cam = FindFirstObjectByType<Camera>(FindObjectsInactive.Include);

            if (cam == null)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                cam = cameraObject.AddComponent<Camera>();
                cameraObject.tag = "MainCamera";
            }

            cam.orthographic = true;
            cam.transform.position = new Vector3(0f, 0f, -10f);
            if (cam.GetComponent<CameraController>() == null)
            {
                cam.gameObject.AddComponent<CameraController>();
            }
        }

        private void EnsureCoreManagers()
        {
            GameObject root = GameObject.Find("_Managers");
            if (root == null)
            {
                root = new GameObject("_Managers");
                DontDestroyOnLoad(root);
            }

            if (GameManager.Instance == null && root.GetComponent<GameManager>() == null) root.AddComponent<GameManager>();
            if (EventManager.Instance == null && root.GetComponent<EventManager>() == null) root.AddComponent<EventManager>();
            if (FindFirstObjectByType<BattleManager>(FindObjectsInactive.Include) == null) root.AddComponent<BattleManager>();
            if (FindFirstObjectByType<BodyComponentManager>(FindObjectsInactive.Include) == null) root.AddComponent<BodyComponentManager>();
            if (FindFirstObjectByType<DiseaseManager>(FindObjectsInactive.Include) == null) root.AddComponent<DiseaseManager>();
            if (FindFirstObjectByType<CardManager>(FindObjectsInactive.Include) == null) root.AddComponent<CardManager>();
            if (FindFirstObjectByType<CardBuffManager>(FindObjectsInactive.Include) == null) root.AddComponent<CardBuffManager>();
        }

        private void EnsureArena()
        {
            if (FindFirstObjectByType<ProceduralBloodVesselArena>(FindObjectsInactive.Include) == null)
            {
                GameObject arena = new GameObject("ProceduralBloodVesselArena");
                arena.AddComponent<ProceduralBloodVesselArena>();
            }
        }

        private void EnsureUI()
        {
            Canvas canvas = GameplayUICreator.EnsureCanvas();
            canvas.gameObject.SetActive(true);

            if (canvas.transform.Find("MainMenuPanel") == null || canvas.transform.Find("BattleHUDPanel") == null)
            {
                GameplayUICreator.BuildAll(canvas.transform);
            }

            UIManager ui = canvas.GetComponent<UIManager>();
            if (ui == null) ui = canvas.gameObject.AddComponent<UIManager>();
            ui.ConnectToGameManager();
            ui.RefreshForCurrentState();
        }
    }
}
