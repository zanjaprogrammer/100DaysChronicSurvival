using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ChronicSurvival.BodyComponents;

namespace ChronicSurvival.UI
{
    public class BodyStatBar : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private Image fillBar;
        [SerializeField] private Image iconImage;

        [Header("Colors")]
        [SerializeField] private Color normalColor = Color.green;
        [SerializeField] private Color warningColor = Color.yellow;
        [SerializeField] private Color criticalColor = Color.red;
        [SerializeField] private Color excellentColor = Color.cyan;

        [Header("Animation")]
        [SerializeField] private float smoothSpeed = 5f;

        private BodyComponent component;
        private float targetFillAmount;

        public void Initialize(BodyComponent bodyComponent)
        {
            component = bodyComponent;

            if (component != null)
            {
                if (nameText != null)
                    nameText.text = component.componentName;

                component.OnValueChanged += OnValueChanged;
                component.OnStateChanged += OnStateChanged;

                UpdateDisplay();
            }
        }

        private void Update()
        {
            if (fillBar != null)
            {
                fillBar.fillAmount = Mathf.Lerp(fillBar.fillAmount, targetFillAmount, Time.deltaTime * smoothSpeed);
            }
        }

        private void OnValueChanged(float newValue)
        {
            UpdateDisplay();
        }

        private void OnStateChanged(ComponentState newState)
        {
            UpdateColor(newState);
        }

        private void UpdateDisplay()
        {
            if (component == null) return;

            if (valueText != null)
            {
                valueText.text = $"{component.CurrentValue:F0}";
            }

            targetFillAmount = component.GetNormalizedValue();

            UpdateColor(component.CurrentState);
        }

        private void UpdateColor(ComponentState state)
        {
            if (fillBar == null) return;

            Color targetColor = normalColor;

            switch (state)
            {
                case ComponentState.Excellent:
                    targetColor = excellentColor;
                    break;
                case ComponentState.Normal:
                    targetColor = normalColor;
                    break;
                case ComponentState.Warning:
                    targetColor = warningColor;
                    break;
                case ComponentState.Critical:
                    targetColor = criticalColor;
                    break;
            }

            fillBar.color = targetColor;
        }

        private void OnDestroy()
        {
            if (component != null)
            {
                component.OnValueChanged -= OnValueChanged;
                component.OnStateChanged -= OnStateChanged;
            }
        }
    }
}
