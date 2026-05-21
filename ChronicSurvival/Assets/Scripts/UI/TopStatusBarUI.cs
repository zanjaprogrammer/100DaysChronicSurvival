using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ChronicSurvival.BodyComponents;
using ChronicSurvival.Core;

namespace ChronicSurvival.UI
{
    /// <summary>
    /// Displays 3 key summary stats at the top center of the Battle HUD:
    /// Immune System, Overall Health, and Inflammation.
    /// Matches the reference UI layout.
    /// </summary>
    public class TopStatusBarUI : MonoBehaviour
    {
        [Header("Immune System")]
        [SerializeField] private TextMeshProUGUI immuneLabel;
        [SerializeField] private TextMeshProUGUI immuneValueText;
        [SerializeField] private Image immuneFillBar;

        [Header("Overall Health")]
        [SerializeField] private TextMeshProUGUI healthLabel;
        [SerializeField] private TextMeshProUGUI healthValueText;
        [SerializeField] private Image healthFillBar;

        [Header("Inflammation")]
        [SerializeField] private TextMeshProUGUI inflammationLabel;
        [SerializeField] private TextMeshProUGUI inflammationValueText;
        [SerializeField] private Image inflammationFillBar;

        [Header("Animation")]
        [SerializeField] private float smoothSpeed = 5f;

        private float targetImmune, targetHealth, targetInflammation;
        private float currentImmune, currentHealth, currentInflammation;

        private void Update()
        {
            if (BodyComponentManager.Instance == null) return;

            // Get values
            var immune = BodyComponentManager.Instance.ImmuneStrength;
            var inflammation = BodyComponentManager.Instance.Inflammation;
            float overallHealth = BodyComponentManager.Instance.GetOverallHealth();

            // Immune
            if (immune != null)
            {
                targetImmune = immune.GetNormalizedValue();
                currentImmune = Mathf.Lerp(currentImmune, targetImmune, Time.deltaTime * smoothSpeed);
                
                if (immuneFillBar != null)
                    immuneFillBar.fillAmount = currentImmune;
                if (immuneValueText != null)
                    immuneValueText.text = $"{Mathf.RoundToInt(immune.CurrentValue)}%";

                // Color based on value
                if (immuneFillBar != null)
                    immuneFillBar.color = GetStatColor(currentImmune);
            }

            // Health
            targetHealth = overallHealth;
            currentHealth = Mathf.Lerp(currentHealth, targetHealth, Time.deltaTime * smoothSpeed);
            
            if (healthFillBar != null)
                healthFillBar.fillAmount = currentHealth;
            if (healthValueText != null)
                healthValueText.text = $"{Mathf.RoundToInt(overallHealth * 100f)}%";
            if (healthFillBar != null)
                healthFillBar.color = GetStatColor(currentHealth);

            // Inflammation (inverted - low is good)
            if (inflammation != null)
            {
                targetInflammation = inflammation.GetNormalizedValue();
                currentInflammation = Mathf.Lerp(currentInflammation, targetInflammation, Time.deltaTime * smoothSpeed);
                
                if (inflammationFillBar != null)
                    inflammationFillBar.fillAmount = currentInflammation;
                if (inflammationValueText != null)
                    inflammationValueText.text = $"{Mathf.RoundToInt(inflammation.CurrentValue)}%";

                // Inverted color: high inflammation = bad (red)
                if (inflammationFillBar != null)
                    inflammationFillBar.color = GetInflammationColor(currentInflammation);
            }
        }

        private Color GetStatColor(float normalizedValue)
        {
            if (normalizedValue >= 0.7f)
                return new Color(0.2f, 0.85f, 0.4f, 1f); // Green
            else if (normalizedValue >= 0.4f)
                return new Color(1f, 0.75f, 0.2f, 1f);   // Orange/Yellow
            else
                return new Color(0.95f, 0.2f, 0.2f, 1f);  // Red
        }

        private Color GetInflammationColor(float normalizedValue)
        {
            if (normalizedValue <= 0.3f)
                return new Color(0.2f, 0.85f, 0.4f, 1f); // Green (low inflammation good)
            else if (normalizedValue <= 0.6f)
                return new Color(1f, 0.75f, 0.2f, 1f);   // Orange/Yellow
            else
                return new Color(0.95f, 0.2f, 0.2f, 1f);  // Red (high inflammation bad)
        }
    }
}
