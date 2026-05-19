using UnityEngine;
using System.Collections.Generic;
using ChronicSurvival.Core;

namespace ChronicSurvival.BodyComponents
{
    public class BodyComponentManager : MonoBehaviour
    {
        public static BodyComponentManager Instance { get; private set; }

        [Header("Components")]
        [SerializeField] private BodyComponent energy;
        [SerializeField] private BodyComponent bloodSugar;
        [SerializeField] private BodyComponent bloodPressure;
        [SerializeField] private BodyComponent immuneStrength;
        [SerializeField] private BodyComponent stress;
        [SerializeField] private BodyComponent sleepQuality;
        [SerializeField] private BodyComponent inflammation;
        [SerializeField] private BodyComponent insulinEfficiency;
        [SerializeField] private BodyComponent toxicity;
        [SerializeField] private BodyComponent metabolism;
        [SerializeField] private BodyComponent hydration;
        [SerializeField] private BodyComponent oxygenLevel;
        [SerializeField] private BodyComponent heartStability;
        [SerializeField] private BodyComponent hormoneBalance;

        [Header("Update Settings")]
        [SerializeField] private float updateInterval = 1f;
        [SerializeField] private bool enableInterconnections = true;

        [Header("Debug")]
        [SerializeField] private bool debugMode = true;

        private Dictionary<string, BodyComponent> components;
        private float updateTimer;

        public BodyComponent Energy => energy;
        public BodyComponent BloodSugar => bloodSugar;
        public BodyComponent BloodPressure => bloodPressure;
        public BodyComponent ImmuneStrength => immuneStrength;
        public BodyComponent Stress => stress;
        public BodyComponent SleepQuality => sleepQuality;
        public BodyComponent Inflammation => inflammation;
        public BodyComponent InsulinEfficiency => insulinEfficiency;
        public BodyComponent Toxicity => toxicity;
        public BodyComponent Metabolism => metabolism;
        public BodyComponent Hydration => hydration;
        public BodyComponent OxygenLevel => oxygenLevel;
        public BodyComponent HeartStability => heartStability;
        public BodyComponent HormoneBalance => hormoneBalance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeComponents();
        }

        private void InitializeComponents()
        {
            components = new Dictionary<string, BodyComponent>();

            AddComponent("Energy", energy);
            AddComponent("BloodSugar", bloodSugar);
            AddComponent("BloodPressure", bloodPressure);
            AddComponent("ImmuneStrength", immuneStrength);
            AddComponent("Stress", stress);
            AddComponent("SleepQuality", sleepQuality);
            AddComponent("Inflammation", inflammation);
            AddComponent("InsulinEfficiency", insulinEfficiency);
            AddComponent("Toxicity", toxicity);
            AddComponent("Metabolism", metabolism);
            AddComponent("Hydration", hydration);
            AddComponent("OxygenLevel", oxygenLevel);
            AddComponent("HeartStability", heartStability);
            AddComponent("HormoneBalance", hormoneBalance);

            foreach (var component in components.Values)
            {
                if (component != null)
                {
                    component.Initialize();
                    component.OnValueChanged += (value) => OnComponentChanged(component);
                    component.OnCritical += () => OnComponentCritical(component);
                }
            }

            if (debugMode) Debug.Log($"[BodyComponentManager] Initialized {components.Count} components");
        }

        private void AddComponent(string key, BodyComponent component)
        {
            if (component != null)
            {
                components[key] = component;
            }
        }

        private void Update()
        {
            updateTimer += Time.deltaTime;

            if (updateTimer >= updateInterval)
            {
                UpdateComponents(updateTimer);
                updateTimer = 0f;
            }
        }

        private void UpdateComponents(float deltaTime)
        {
            foreach (var component in components.Values)
            {
                if (component != null)
                {
                    component.ApplyPassiveDecay(deltaTime);
                }
            }

            if (enableInterconnections)
            {
                ApplyInterconnections();
            }
        }

        private void ApplyInterconnections()
        {
            // Stress affects multiple systems
            if (stress != null && stress.CurrentValue > 60)
            {
                float stressImpact = (stress.CurrentValue - 60) * 0.01f;
                bloodPressure?.ModifyValue(stressImpact * 0.5f);
                sleepQuality?.ModifyValue(-stressImpact * 0.3f);
                immuneStrength?.ModifyValue(-stressImpact * 0.2f);
            }

            // Sleep quality affects everything
            if (sleepQuality != null && sleepQuality.CurrentValue < 50)
            {
                float sleepDeficit = (50 - sleepQuality.CurrentValue) * 0.01f;
                energy?.ModifyValue(-sleepDeficit * 0.5f);
                immuneStrength?.ModifyValue(-sleepDeficit * 0.3f);
                stress?.ModifyValue(sleepDeficit * 0.4f);
                inflammation?.ModifyValue(sleepDeficit * 0.2f);
            }

            // High blood sugar affects insulin
            if (bloodSugar != null && bloodSugar.CurrentValue > 140)
            {
                float sugarExcess = (bloodSugar.CurrentValue - 140) * 0.01f;
                insulinEfficiency?.ModifyValue(-sugarExcess * 0.1f);
                energy?.ModifyValue(-sugarExcess * 0.2f);
                inflammation?.ModifyValue(sugarExcess * 0.1f);
            }

            // Inflammation affects immune system
            if (inflammation != null && inflammation.CurrentValue > 50)
            {
                float inflammationLevel = (inflammation.CurrentValue - 50) * 0.01f;
                immuneStrength?.ModifyValue(-inflammationLevel * 0.3f);
            }

            // Toxicity affects immune and inflammation
            if (toxicity != null && toxicity.CurrentValue > 50)
            {
                float toxicLevel = (toxicity.CurrentValue - 50) * 0.01f;
                immuneStrength?.ModifyValue(-toxicLevel * 0.2f);
                inflammation?.ModifyValue(toxicLevel * 0.3f);
            }

            // Low hydration affects everything
            if (hydration != null && hydration.CurrentValue < 50)
            {
                float dehydration = (50 - hydration.CurrentValue) * 0.01f;
                energy?.ModifyValue(-dehydration * 0.2f);
                bloodPressure?.ModifyValue(dehydration * 0.3f);
                toxicity?.ModifyValue(dehydration * 0.1f);
            }

            // Low oxygen affects energy
            if (oxygenLevel != null && oxygenLevel.CurrentValue < 80)
            {
                float oxygenDeficit = (80 - oxygenLevel.CurrentValue) * 0.01f;
                energy?.ModifyValue(-oxygenDeficit * 0.4f);
            }

            // High blood pressure affects heart
            if (bloodPressure != null && bloodPressure.CurrentValue > 140)
            {
                float pressureExcess = (bloodPressure.CurrentValue - 140) * 0.01f;
                heartStability?.ModifyValue(-pressureExcess * 0.3f);
            }
        }

        private void OnComponentChanged(BodyComponent component)
        {
            EventManager.TriggerEvent(GameEvents.COMPONENT_CHANGED, component);

            if (debugMode)
            {
                Debug.Log($"[BodyComponent] {component.componentName}: {component.CurrentValue:F1} ({component.CurrentState})");
            }
        }

        private void OnComponentCritical(BodyComponent component)
        {
            EventManager.TriggerEvent(GameEvents.COMPONENT_CRITICAL, component);

            if (debugMode)
            {
                Debug.LogWarning($"[BodyComponent] CRITICAL: {component.componentName} = {component.CurrentValue:F1}");
            }
        }

        public BodyComponent GetComponent(string name)
        {
            if (components.TryGetValue(name, out BodyComponent component))
            {
                return component;
            }
            return null;
        }

        public Dictionary<string, BodyComponent> GetAllComponents()
        {
            return new Dictionary<string, BodyComponent>(components);
        }

        public int GetCriticalCount()
        {
            int count = 0;
            foreach (var component in components.Values)
            {
                if (component != null && component.IsCritical())
                {
                    count++;
                }
            }
            return count;
        }

        public float GetOverallHealth()
        {
            float total = 0f;
            int count = 0;

            foreach (var component in components.Values)
            {
                if (component != null)
                {
                    total += component.GetNormalizedValue();
                    count++;
                }
            }

            return count > 0 ? total / count : 0f;
        }
    }
}
