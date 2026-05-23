using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ChronicSurvival.BodyComponents;

namespace ChronicSurvival.UI
{
    public class BodyStatsPanel : MonoBehaviour
    {
        [Header("Prefab")]
        [SerializeField] private GameObject statBarPrefab;

        [Header("Layout")]
        [SerializeField] private Transform statsContainer;
        [SerializeField] private bool showOnlyImportant = true;

        [Header("Important Stats")]
        [SerializeField] private List<string> importantStats = new List<string>
        {
            "Energy",
            "BloodSugar",
            "BloodPressure",
            "ImmuneStrength",
            "Inflammation",
            "SleepQuality",
            "Stress",
            "Toxicity",
            "InsulinEfficiency"
        };

        private Dictionary<string, BodyStatBar> statBars = new Dictionary<string, BodyStatBar>();

        private void Start()
        {
            // Apply premium layout group spacing and padding at runtime for breathability
            if (statsContainer != null)
            {
                VerticalLayoutGroup layoutGroup = statsContainer.GetComponent<VerticalLayoutGroup>();
                if (layoutGroup != null)
                {
                    layoutGroup.spacing = 14f;
                    layoutGroup.padding = new RectOffset(6, 6, 6, 6);
                }
            }
            InitializeStatBars();
        }

        private void InitializeStatBars()
        {
            if (BodyComponentManager.Instance == null)
            {
                Debug.LogWarning("[BodyStatsPanel] BodyComponentManager not found!");
                return;
            }

            if (showOnlyImportant)
            {
                foreach (string componentName in importantStats)
                {
                    BodyComponent component = BodyComponentManager.Instance.GetComponent(componentName);
                    if (component != null)
                    {
                        CreateStatBar(componentName, component);
                    }
                }
            }
            else
            {
                var components = BodyComponentManager.Instance.GetAllComponents();
                foreach (var kvp in components)
                {
                    string componentName = kvp.Key;
                    BodyComponent component = kvp.Value;

                    if (component == null) continue;

                    CreateStatBar(componentName, component);
                }
            }
        }

        private void CreateStatBar(string name, BodyComponent component)
        {
            if (statBarPrefab == null || statsContainer == null)
            {
                Debug.LogWarning("[BodyStatsPanel] Missing prefab or container!");
                return;
            }

            GameObject barObj = Instantiate(statBarPrefab, statsContainer);
            BodyStatBar statBar = barObj.GetComponent<BodyStatBar>();

            if (statBar != null)
            {
                statBar.Initialize(component);
                statBars[name] = statBar;
            }
        }

        public void ToggleImportantOnly()
        {
            showOnlyImportant = !showOnlyImportant;
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            // Clear existing
            foreach (var bar in statBars.Values)
            {
                if (bar != null)
                    Destroy(bar.gameObject);
            }
            statBars.Clear();

            // Recreate
            InitializeStatBars();
        }
    }
}
