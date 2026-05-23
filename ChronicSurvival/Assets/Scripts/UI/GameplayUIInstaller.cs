using UnityEngine;
using UnityEngine.UI;
using ChronicSurvival.Core;

namespace ChronicSurvival.UI
{
    /// <summary>
    /// Ensures Canvas scale, UIManager, and gameplay UI panels exist when entering Play mode.
    /// UI is built in code at runtime (NeonShooter-style) — no editor menu required.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class GameplayUIInstaller : MonoBehaviour
    {
        [SerializeField] private bool autoBuildIfMissing = true;

        private void Awake()
        {
            Canvas canvas = GetComponent<Canvas>();
            if (canvas != null)
            {
                GameplayUICreator.FixCanvasLayout(canvas);
            }
            EnsureEventSystem();
            EnsureUIManagerOnCanvas();

            if (autoBuildIfMissing && NeedsUIRebuild())
            {
                GameplayUICreator.BuildAll(transform);
                Debug.Log("[GameplayUIInstaller] UI dibangun otomatis saat Play (runtime code).");
            }

            WireUIManagerReferences();
            SyncUIWithGameState();
        }

        private void EnsureEventSystem()
        {
            if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>(FindObjectsInactive.Include) == null)
            {
                GameObject es = new GameObject("EventSystem");
                es.AddComponent<UnityEngine.EventSystems.EventSystem>();
                es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
        }

        private void EnsureUIManagerOnCanvas()
        {
            if (GetComponent<UIManager>() == null)
            {
                gameObject.AddComponent<UIManager>();
            }
        }

        private bool NeedsUIRebuild()
        {
            if (transform.Find("MainMenuPanel") == null) return true;

            Transform battle = transform.Find("BattleHUDPanel");
            if (battle == null) return true;

            // Empty legacy panel from old scene setup (no v2 children)
            if (battle.Find("RoundPanel") == null) return true;
            if (battle.Find("BodyStatsBottom") == null) return true;

            return false;
        }

        private void WireUIManagerReferences()
        {
            UIManager ui = GetComponent<UIManager>();
            if (ui == null) return;

            ui.BindPanels(
                FindPanel("MainMenuPanel"),
                FindPanel("BattleHUDPanel"),
                FindPanel("CardSelectionPanel"),
                FindPanel("RandomEventPanel"),
                FindPanel("GameOverPanel"),
                FindPanel("VictoryPanel"),
                FindPanel("PausePanel"));
        }

        private GameObject FindPanel(string name)
        {
            Transform t = transform.Find(name);
            return t != null ? t.gameObject : null;
        }

        private void SyncUIWithGameState()
        {
            GetComponent<UIManager>()?.RefreshForCurrentState();
        }

        private void Start()
        {
            WireUIManagerReferences();
            GetComponent<UIManager>()?.RefreshForCurrentState();
        }
    }
}
