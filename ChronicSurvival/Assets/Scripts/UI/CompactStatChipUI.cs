using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ChronicSurvival.BodyComponents;

namespace ChronicSurvival.UI
{
    /// <summary>
    /// Compact stat chip for the bottom body-components bar.
    /// </summary>
    public class CompactStatChipUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI labelText;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private Image fillBar;
        [SerializeField] private float smoothSpeed = 8f;

        private BodyComponent component;
        private float targetFill;
        private string componentKey;

        public void Bind(string key, BodyComponent bodyComponent)
        {
            componentKey = key;
            component = bodyComponent;

            if (labelText != null)
            {
                labelText.text = GetShortLabel(key).ToUpperInvariant();
            }

            if (component != null)
            {
                component.OnValueChanged += OnChanged;
                UpdateDisplay();
            }
        }

        private void Update()
        {
            if (fillBar != null)
            {
                fillBar.fillAmount = Mathf.Lerp(fillBar.fillAmount, targetFill, Time.deltaTime * smoothSpeed);
            }
        }

        private void OnChanged(float _) => UpdateDisplay();

        private void UpdateDisplay()
        {
            if (component == null) return;

            targetFill = component.GetNormalizedValue();
            if (valueText != null)
            {
                valueText.text = $"{component.CurrentValue:F0}%";
            }
            Color stateColor = GetColorForState(component.CurrentState);
            if (fillBar != null)
            {
                fillBar.color = stateColor;
            }
            if (valueText != null)
            {
                valueText.color = stateColor;
            }
        }

        private void OnDestroy()
        {
            if (component != null)
            {
                component.OnValueChanged -= OnChanged;
            }
        }

        public static string GetShortLabel(string key)
        {
            switch (key)
            {
                case "Energy": return "Energy";
                case "BloodSugar": return "Gula Darah";
                case "BloodPressure": return "Tekanan Darah";
                case "ImmuneStrength": return "Sistem Imun";
                case "Inflammation": return "Peradangan";
                case "SleepQuality": return "Tidur";
                case "Stress": return "Stress";
                case "Toxicity": return "Toksin";
                case "InsulinEfficiency": return "Insulin";
                default: return key;
            }
        }

        private static Color GetColorForState(ComponentState state)
        {
            switch (state)
            {
                case ComponentState.Optimal: return new Color(0.2f, 0.9f, 0.55f);
                case ComponentState.Warning: return new Color(1f, 0.65f, 0.15f);
                case ComponentState.Critical: return new Color(0.95f, 0.2f, 0.2f);
                default: return new Color(0.2f, 0.85f, 0.4f);
            }
        }
    }
}
