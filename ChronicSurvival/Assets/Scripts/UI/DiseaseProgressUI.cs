using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ChronicSurvival.Disease;

namespace ChronicSurvival.UI
{
    /// <summary>
    /// Controls a single disease's progress bar in the HUD.
    /// Wire-compatible with DiseaseProgressUI.prefab fields.
    /// </summary>
    public class DiseaseProgressUI : MonoBehaviour
    {
        [Header("Texts")]
        [SerializeField] private TextMeshProUGUI diseaseNameText;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private TextMeshProUGUI stageText;

        [Header("Images")]
        [SerializeField] private Image fillBar;
        [SerializeField] private Image iconImage;

        [Header("Settings")]
        [SerializeField] private float animationSpeed = 5f;
        [SerializeField] private Color safeColor = Color.green;
        [SerializeField] private Color warningColor = Color.yellow;
        [SerializeField] private Color criticalColor = Color.red;

        private ChronicSurvival.Disease.Disease diseaseData;
        private float targetFillAmount = 0f;
        private float currentFillAmount = 0f;

        public void Initialize(ChronicSurvival.Disease.Disease disease)
        {
            diseaseData = disease;

            if (diseaseData == null)
            {
                Debug.LogError("[DiseaseProgressUI] Disease data is null!");
                return;
            }

            if (diseaseNameText != null)
            {
                diseaseNameText.text = diseaseData.DiseaseName;
            }

            // Instantly sync initial value
            targetFillAmount = diseaseData.CurrentProgression / 100f;
            currentFillAmount = targetFillAmount;
            if (fillBar != null)
            {
                fillBar.fillAmount = currentFillAmount;
            }

            UpdateDisplay(diseaseData.CurrentProgression);
        }

        private void Update()
        {
            if (diseaseData == null) return;

            // Smoothly animate progress bar fill towards target progression
            targetFillAmount = diseaseData.CurrentProgression / 100f;
            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, Time.deltaTime * animationSpeed);

            if (fillBar != null)
            {
                fillBar.fillAmount = currentFillAmount;
            }

            UpdateDisplay(diseaseData.CurrentProgression);
        }

        private void UpdateDisplay(float progression)
        {
            if (diseaseData == null) return;

            // Set percentage text
            if (progressText != null)
            {
                progressText.text = $"{Mathf.RoundToInt(progression)}%";
            }

            // Set stage text
            if (stageText != null)
            {
                stageText.text = diseaseData.GetStageDescription();
            }

            // Color shift based on stage/progression
            Color targetColor = safeColor;
            if (diseaseData.CurrentStage == DiseaseStage.Critical || progression >= 90f)
            {
                targetColor = criticalColor;
            }
            else if (diseaseData.CurrentStage == DiseaseStage.Stage2 || diseaseData.CurrentStage == DiseaseStage.Stage3 || progression >= 50f)
            {
                targetColor = warningColor;
            }

            if (fillBar != null)
            {
                fillBar.color = targetColor;
            }

            if (progressText != null)
            {
                progressText.color = targetColor;
            }
        }
    }
}
