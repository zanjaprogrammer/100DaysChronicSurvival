using UnityEngine;
using ChronicSurvival.BodyComponents;

namespace ChronicSurvival.Disease
{
    [CreateAssetMenu(fileName = "DiabetesDisease", menuName = "ChronicSurvival/Diseases/Diabetes")]
    public class DiabetesDisease : Disease
    {
        [Header("Diabetes Specific")]
        [SerializeField] private float bloodSugarWeight = 0.4f;
        [SerializeField] private float insulinEfficiencyWeight = 0.3f;
        [SerializeField] private float metabolismWeight = 0.2f;
        [SerializeField] private float inflammationWeight = 0.1f;

        [Header("Thresholds")]
        [SerializeField] private float criticalBloodSugar = 80f;
        [SerializeField] private float criticalInsulinEfficiency = 30f;

        protected override float CalculateProgressionDelta()
        {
            if (BodyComponentManager.Instance == null)
                return 0f;

            float bloodSugar = BodyComponentManager.Instance.GetComponentValue("BloodSugar");
            float insulinEfficiency = BodyComponentManager.Instance.GetComponentValue("InsulinEfficiency");
            float metabolism = BodyComponentManager.Instance.GetComponentValue("Metabolism");
            float inflammation = BodyComponentManager.Instance.GetComponentValue("Inflammation");

            // High blood sugar increases progression
            float bloodSugarFactor = Mathf.Max(0f, (bloodSugar - 50f) / 50f);
            
            // Low insulin efficiency increases progression
            float insulinFactor = Mathf.Max(0f, (50f - insulinEfficiency) / 50f);
            
            // Low metabolism increases progression
            float metabolismFactor = Mathf.Max(0f, (50f - metabolism) / 50f);
            
            // High inflammation increases progression
            float inflammationFactor = Mathf.Max(0f, (inflammation - 50f) / 50f);

            float totalProgression = 
                (bloodSugarFactor * bloodSugarWeight) +
                (insulinFactor * insulinEfficiencyWeight) +
                (metabolismFactor * metabolismWeight) +
                (inflammationFactor * inflammationWeight);

            // Critical conditions accelerate progression
            if (bloodSugar > criticalBloodSugar || insulinEfficiency < criticalInsulinEfficiency)
            {
                totalProgression *= 2f;
            }

            return totalProgression;
        }

        protected override void ApplyStageEffects()
        {
            base.ApplyStageEffects();

            // Diabetes-specific effects on body components
            if (BodyComponentManager.Instance == null) return;

            switch (currentStage)
            {
                case DiseaseStage.Stage1:
                    // Slightly reduce insulin efficiency
                    BodyComponentManager.Instance.ModifyComponent("InsulinEfficiency", -0.5f);
                    break;

                case DiseaseStage.Stage2:
                    // Reduce insulin efficiency and metabolism
                    BodyComponentManager.Instance.ModifyComponent("InsulinEfficiency", -1f);
                    BodyComponentManager.Instance.ModifyComponent("Metabolism", -0.5f);
                    break;

                case DiseaseStage.Stage3:
                    // Significant reduction in multiple systems
                    BodyComponentManager.Instance.ModifyComponent("InsulinEfficiency", -2f);
                    BodyComponentManager.Instance.ModifyComponent("Metabolism", -1f);
                    BodyComponentManager.Instance.ModifyComponent("Energy", -1f);
                    break;

                case DiseaseStage.Critical:
                    // Severe system-wide effects
                    BodyComponentManager.Instance.ModifyComponent("InsulinEfficiency", -3f);
                    BodyComponentManager.Instance.ModifyComponent("Metabolism", -2f);
                    BodyComponentManager.Instance.ModifyComponent("Energy", -2f);
                    BodyComponentManager.Instance.ModifyComponent("ImmuneStrength", -1f);
                    break;
            }
        }
    }
}
