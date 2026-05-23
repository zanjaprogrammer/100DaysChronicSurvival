using UnityEngine;
using System.Collections.Generic;
using ChronicSurvival.Core;

namespace ChronicSurvival.BodyComponents
{
    public class BodyComponentManager : MonoBehaviour
    {
        private static BodyComponentManager _instance;
        public static BodyComponentManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<BodyComponentManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("_BodyComponentManager");
                        _instance = go.AddComponent<BodyComponentManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

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
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeComponents();
        }

        private BodyComponent EnsureComponent(
            string name, 
            BodyComponent existing, 
            string description, 
            float startVal, 
            float minVal, 
            float maxVal, 
            float optMin, 
            float optMax, 
            float critLow, 
            float critHigh, 
            float decay, 
            Color normalCol)
        {
            if (existing != null)
            {
                if (string.IsNullOrEmpty(existing.componentName))
                    existing.componentName = name;
                return existing;
            }

            BodyComponent comp = ScriptableObject.CreateInstance<BodyComponent>();
            comp.componentName = name;
            comp.description = description;
            comp.startingValue = startVal;
            comp.minValue = minVal;
            comp.maxValue = maxVal;
            comp.optimalMin = optMin;
            comp.optimalMax = optMax;
            comp.criticalLow = critLow;
            comp.criticalHigh = critHigh;
            comp.passiveDecay = decay;
            comp.decayInterval = 1f;
            comp.normalColor = normalCol;
            comp.warningColor = new Color(0.95f, 0.6f, 0.15f); // Orange
            comp.dangerColor = new Color(0.85f, 0.15f, 0.15f); // Red
            return comp;
        }

        private void InitializeComponents()
        {
            components = new Dictionary<string, BodyComponent>();

            // Ensure all components exist (fallback to runtime instantiation with defaults if missing)
            energy = EnsureComponent("Energy", energy, "Stamina tubuh dan energi aktivitas.", 70f, 0f, 100f, 60f, 100f, 20f, 100f, -0.2f, new Color(0.95f, 0.76f, 0.2f));
            bloodSugar = EnsureComponent("Gula Darah", bloodSugar, "Kadar glukosa dalam aliran darah.", 85f, 0f, 200f, 70f, 110f, 50f, 150f, -0.1f, new Color(0.9f, 0.3f, 0.3f));
            bloodPressure = EnsureComponent("Tekanan Darah", bloodPressure, "Tekanan darah sistolik.", 120f, 0f, 200f, 90f, 130f, 70f, 160f, -0.05f, new Color(0.85f, 0.2f, 0.4f));
            immuneStrength = EnsureComponent("Sistem Imun", immuneStrength, "Kekuatan pertahanan tubuh melawan patogen.", 68f, 0f, 100f, 60f, 100f, 20f, 100f, -0.1f, new Color(0.3f, 0.65f, 0.85f));
            stress = EnsureComponent("Stress", stress, "Tingkat ketegangan saraf dan pikiran.", 60f, 0f, 100f, 0f, 40f, 0f, 85f, 0.1f, new Color(0.9f, 0.6f, 0.75f));
            sleepQuality = EnsureComponent("Tidur", sleepQuality, "Kualitas tidur dan pemulihan tubuh.", 55f, 0f, 100f, 60f, 100f, 30f, 100f, -0.3f, new Color(0.5f, 0.4f, 0.8f));
            inflammation = EnsureComponent("Peradangan", inflammation, "Respon inflamasi sistemik dalam tubuh.", 45f, 0f, 100f, 0f, 35f, 0f, 70f, 0.05f, new Color(0.95f, 0.45f, 0.1f));
            insulinEfficiency = EnsureComponent("Insulin Eff.", insulinEfficiency, "Efektivitas insulin dalam mengontrol gula darah.", 40f, 0f, 100f, 60f, 100f, 30f, 100f, -0.05f, new Color(0.2f, 0.65f, 0.6f));
            toxicity = EnsureComponent("Toksin", toxicity, "Akumulasi zat beracun dalam tubuh.", 35f, 0f, 100f, 0f, 30f, 0f, 65f, 0.05f, new Color(0.45f, 0.75f, 0.45f));
            metabolism = EnsureComponent("Metabolism", metabolism, "Laju metabolisme dasar tubuh.", 70f, 0f, 100f, 50f, 90f, 30f, 100f, 0f, Color.grey);
            hydration = EnsureComponent("Hydration", hydration, "Kecukupan cairan dalam tubuh.", 80f, 0f, 100f, 60f, 100f, 35f, 100f, -0.4f, Color.grey);
            oxygenLevel = EnsureComponent("Oxygen Level", oxygenLevel, "Tingkat kejenuhan oksigen dalam darah.", 98f, 0f, 100f, 95f, 100f, 90f, 100f, -0.05f, Color.grey);
            heartStability = EnsureComponent("Heart Stability", heartStability, "Kestabilan detak jantung.", 90f, 0f, 100f, 70f, 100f, 50f, 100f, 0f, Color.grey);
            hormoneBalance = EnsureComponent("Hormone Balance", hormoneBalance, "Keseimbangan hormon regulasi tubuh.", 80f, 0f, 100f, 60f, 100f, 40f, 100f, 0f, Color.grey);

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

        public new BodyComponent GetComponent(string name)
        {
            if (components.TryGetValue(name, out BodyComponent component))
            {
                return component;
            }
            return null;
        }

        public float GetComponentValue(string name)
        {
            BodyComponent component = GetComponent(name);
            return component != null ? component.CurrentValue : 50f; // Default to 50 if not found
        }

        public void ModifyComponent(string name, float amount)
        {
            BodyComponent component = GetComponent(name);
            if (component != null)
            {
                component.ModifyValue(amount);
            }
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

        public void ResetComponents()
        {
            foreach (var component in components.Values)
            {
                if (component != null)
                {
                    component.Initialize();
                }
            }
            if (debugMode) Debug.Log("[BodyComponentManager] Reset all body components");
        }
    }
}
