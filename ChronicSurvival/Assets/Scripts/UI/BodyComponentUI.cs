using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ChronicSurvival.BodyComponents;

namespace ChronicSurvival.UI
{
    public class BodyComponentUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BodyComponent component;
        [SerializeField] private Image fillBar;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI valueText;
        [SerializeField] private GameObject warningIcon;

        [Header("Settings")]
        [SerializeField] private bool showValue = true;
        [SerializeField] private bool animateChanges = true;
        [SerializeField] private float animationSpeed = 5f;

        private float targetFill;
        private Color targetColor;

        private void Start()
        {
            if (component != null)
            {
                Initialize();
                component.OnValueChanged += OnValueChanged;
                component.OnStateChanged += OnStateChanged;
            }
        }

        private void Initialize()
        {
            if (nameText != null)
                nameText.text = component.componentName;

            if (iconImage != null && component.icon != null)
                iconImage.sprite = component.icon;

            UpdateUI();
        }

        private void Update()
        {
            if (animateChanges && fillBar != null)
            {
                fillBar.fillAmount = Mathf.Lerp(fillBar.fillAmount, targetFill, animationSpeed * Time.deltaTime);
                fillBar.color = Color.Lerp(fillBar.color, targetColor, animationSpeed * Time.deltaTime);
            }
        }

        private void OnValueChanged(float value)
        {
            UpdateUI();
        }

        private void OnStateChanged(ComponentState state)
        {
            UpdateWarning();
        }

        private void UpdateUI()
        {
            if (component == null) return;

            targetFill = component.GetNormalizedValue();
            targetColor = component.GetCurrentColor();

            if (!animateChanges && fillBar != null)
            {
                fillBar.fillAmount = targetFill;
                fillBar.color = targetColor;
            }

            if (showValue && valueText != null)
            {
                valueText.text = $"{component.CurrentValue:F0}";
            }

            UpdateWarning();
        }

        private void UpdateWarning()
        {
            if (warningIcon != null)
            {
                warningIcon.SetActive(component.IsCritical());
            }
        }

        public void SetComponent(BodyComponent newComponent)
        {
            if (component != null)
            {
                component.OnValueChanged -= OnValueChanged;
                component.OnStateChanged -= OnStateChanged;
            }

            component = newComponent;

            if (component != null)
            {
                Initialize();
                component.OnValueChanged += OnValueChanged;
                component.OnStateChanged += OnStateChanged;
            }
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
