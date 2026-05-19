using UnityEngine;
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
        [SerializeField] private bool showOnlyImportant = false;

        [Header("Important Stats")]
        [SerializeField] private List<string> importantStats = new List<string>
        {
            "Energy",
            "BloodSugar",
            "BloodPressure",
            "ImmuneStrength",
            "Stress"
        };

        private Dictionary<string, BodyStatBar> statBars = new Dictionary<string, BodyStatBar>();

        private void Start()
        {
            InitializeStatBars();
        }

        private void InitializeStatBars()
        {
            if (BodyComponentManager.Instance == null)
            {
                Debug.LogWarning("[BodyStatsPanel] BodyComponentManager not found!");
                return;
            }

            var components = BodyComponentManager.Instance.GetAllComponents();

            foreach (var kvp in components)
            {
                string componentName = kvp.Key;
                BodyComponent component = kvp.Value;

                if (component == null) continue;

                // Skip if showing only important and this isn't important
                if (showOnlyImportant && !importantStats.Contains(componentName))
                    continue;

                CreateStatBar(componentName, component);
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
