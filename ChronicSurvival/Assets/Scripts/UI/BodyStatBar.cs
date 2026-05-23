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
        [SerializeField] private Color normalColor = new Color(0.2f, 0.84f, 0.4f);       // Emerald Green (#33d666)
        [SerializeField] private Color warningColor = new Color(0.95f, 0.6f, 0.15f);     // Warm Orange (#f29926)
        [SerializeField] private Color criticalColor = new Color(0.9f, 0.2f, 0.2f);       // Coral Red (#e53333)
        [SerializeField] private Color excellentColor = new Color(0.2f, 0.84f, 0.62f);   // Teal/Mint (#33d69f)

        [Header("Animation")]
        [SerializeField] private float smoothSpeed = 5f;

        private BodyComponent component;
        private float targetFillAmount;

        public void Initialize(BodyComponent bodyComponent)
        {
            component = bodyComponent;

            ApplyPremiumStyling();

            if (component != null)
            {
                if (nameText != null)
                {
                    nameText.text = GetIndonesianName(component.componentName).ToUpperInvariant();
                }

                component.OnValueChanged += OnValueChanged;
                component.OnStateChanged += OnStateChanged;

                UpdateDisplay();
            }
        }

        private void ApplyPremiumStyling()
        {
            LayoutElement layoutElement = GetComponent<LayoutElement>();
            if (layoutElement == null)
            {
                layoutElement = gameObject.AddComponent<LayoutElement>();
            }
            layoutElement.minHeight = 48f;
            layoutElement.preferredHeight = 48f;

            if (iconImage != null)
            {
                iconImage.gameObject.SetActive(false);
            }

            if (nameText != null)
            {
                RectTransform nameRect = nameText.rectTransform;
                nameRect.anchorMin = new Vector2(0f, 0.45f);
                nameRect.anchorMax = new Vector2(0.7f, 1f);
                nameRect.pivot = new Vector2(0f, 1f);
                nameRect.anchoredPosition = Vector2.zero;
                nameRect.sizeDelta = Vector2.zero;

                nameText.alignment = TextAlignmentOptions.Left;
                nameText.fontSize = 14f;
                nameText.fontStyle = FontStyles.Bold;
                nameText.color = new Color(0.94f, 0.97f, 1f, 1f);
            }

            if (valueText != null)
            {
                RectTransform valRect = valueText.rectTransform;
                valRect.anchorMin = new Vector2(0.7f, 0.45f);
                valRect.anchorMax = new Vector2(1f, 1f);
                valRect.pivot = new Vector2(1f, 1f);
                valRect.anchoredPosition = Vector2.zero;
                valRect.sizeDelta = Vector2.zero;

                valueText.alignment = TextAlignmentOptions.Right;
                valueText.fontSize = 13f;
                valueText.fontStyle = FontStyles.Bold;
                valueText.color = new Color(0.55f, 0.62f, 0.70f, 1f);
            }

            if (fillBar != null)
            {
                Transform trackTransform = fillBar.transform.parent;
                if (trackTransform != null && trackTransform != transform)
                {
                    RectTransform trackRect = trackTransform.GetComponent<RectTransform>();
                    if (trackRect != null)
                    {
                        trackRect.anchorMin = new Vector2(0f, 0f);
                        trackRect.anchorMax = new Vector2(1f, 0f);
                        trackRect.pivot = new Vector2(0f, 0f);
                        trackRect.anchoredPosition = new Vector2(0f, 4f);
                        trackRect.sizeDelta = new Vector2(0f, 8f);

                        Image trackImage = trackTransform.GetComponent<Image>();
                        if (trackImage != null)
                        {
                            trackImage.color = new Color(0.07f, 0.085f, 0.11f, 0.95f);
                            trackImage.type = Image.Type.Simple;
                        }
                    }
                }

                RectTransform fillRect = fillBar.rectTransform;
                fillRect.anchorMin = Vector2.zero;
                fillRect.anchorMax = Vector2.one;
                fillRect.pivot = new Vector2(0f, 0.5f);
                fillRect.anchoredPosition = Vector2.zero;
                fillRect.sizeDelta = Vector2.zero;

                fillBar.type = Image.Type.Filled;
                fillBar.fillMethod = Image.FillMethod.Horizontal;
                fillBar.fillOrigin = (int)Image.OriginHorizontal.Left;
            }
        }

        private string GetIndonesianName(string englishName)
        {
            switch (englishName)
            {
                case "Energy": return "Energy";
                case "BloodSugar": return "Gula Darah";
                case "BloodPressure": return "Tekanan Darah";
                case "ImmuneStrength": return "Sistem Imun";
                case "Inflammation": return "Peradangan";
                case "SleepQuality": return "Tidur";
                case "Stress": return "Stress";
                case "Toxicity": return "Toksin";
                case "InsulinEfficiency": return "Insulin Eff.";
                default: return englishName;
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
                valueText.text = $"{component.CurrentValue:F0}%";
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
                case ComponentState.Optimal:
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
            if (valueText != null) valueText.color = targetColor;
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
