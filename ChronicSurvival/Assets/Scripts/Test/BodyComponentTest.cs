using UnityEngine;
using ChronicSurvival.BodyComponents;

namespace ChronicSurvival.Test
{
    public class BodyComponentTest : MonoBehaviour
    {
        [Header("Test Actions")]
        [SerializeField] private bool testStressIncrease = false;
        [SerializeField] private bool testSleepDecrease = false;
        [SerializeField] private bool testSugarIncrease = false;
        [SerializeField] private bool testExercise = false;

        [Header("Test Values")]
        [SerializeField] private float testAmount = 10f;

        private BodyComponentManager manager;

        private void Start()
        {
            manager = BodyComponentManager.Instance;

            if (manager == null)
            {
                Debug.LogError("[BodyComponentTest] BodyComponentManager not found!");
            }
        }

        private void Update()
        {
            if (manager == null) return;

            // Keyboard shortcuts for testing
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TestStressIncrease();
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                TestSleepDecrease();
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                TestSugarIncrease();
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                TestExercise();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetAllComponents();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                LogAllComponents();
            }
        }

        [ContextMenu("Test Stress Increase")]
        public void TestStressIncrease()
        {
            if (manager?.Stress != null)
            {
                manager.Stress.ModifyValue(testAmount);
                Debug.Log($"[Test] Stress increased by {testAmount}");
            }
        }

        [ContextMenu("Test Sleep Decrease")]
        public void TestSleepDecrease()
        {
            if (manager?.SleepQuality != null)
            {
                manager.SleepQuality.ModifyValue(-testAmount);
                Debug.Log($"[Test] Sleep decreased by {testAmount}");
            }
        }

        [ContextMenu("Test Sugar Increase")]
        public void TestSugarIncrease()
        {
            if (manager?.BloodSugar != null)
            {
                manager.BloodSugar.ModifyValue(testAmount);
                Debug.Log($"[Test] Blood sugar increased by {testAmount}");
            }
        }

        [ContextMenu("Test Exercise")]
        public void TestExercise()
        {
            if (manager != null)
            {
                manager.Energy?.ModifyValue(10f);
                manager.Stress?.ModifyValue(-15f);
                manager.BloodSugar?.ModifyValue(-10f);
                manager.BloodPressure?.ModifyValue(-10f);
                manager.ImmuneStrength?.ModifyValue(5f);
                Debug.Log("[Test] Exercise effects applied");
            }
        }

        [ContextMenu("Reset All Components")]
        public void ResetAllComponents()
        {
            if (manager != null)
            {
                var components = manager.GetAllComponents();
                foreach (var component in components.Values)
                {
                    if (component != null)
                    {
                        component.Initialize();
                    }
                }
                Debug.Log("[Test] All components reset");
            }
        }

        [ContextMenu("Log All Components")]
        public void LogAllComponents()
        {
            if (manager != null)
            {
                Debug.Log("=== BODY COMPONENTS STATUS ===");
                var components = manager.GetAllComponents();
                foreach (var kvp in components)
                {
                    var component = kvp.Value;
                    if (component != null)
                    {
                        Debug.Log($"{component.componentName}: {component.CurrentValue:F1} ({component.CurrentState})");
                    }
                }
                Debug.Log($"Overall Health: {manager.GetOverallHealth() * 100:F1}%");
                Debug.Log($"Critical Components: {manager.GetCriticalCount()}");
            }
        }
    }
}
