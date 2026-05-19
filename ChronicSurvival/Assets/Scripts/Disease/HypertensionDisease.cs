using UnityEngine;
using ChronicSurvival.BodyComponents;

namespace ChronicSurvival.Disease
{
    [CreateAssetMenu(fileName = "HypertensionDisease", menuName = "ChronicSurvival/Diseases/Hypertension")]
    public class HypertensionDisease : Disease
    {
        [Header("Hypertension Specific")]
        [SerializeField] private float bloodPressureWeight = 0.5f;
        [SerializeField] private float stressWeight = 0.3f;
        [SerializeField] private float heartStabilityWeight = 0.2f;

        [Header("Thresholds")]
        [SerializeField] private float criticalBloodPressure = 85f;
        [SerializeField] private float criticalStress = 80f;

        protected override float CalculateProgressionDelta()
        {
            if (BodyComponentManager.Instance == null)
                return 0f;

            float bloodPressure = BodyComponentManager.Instance.GetComponentValue("BloodPressure");
            float stress = BodyComponentManager.Instance.GetComponentValue("Stress");
            float heartStability = BodyComponentManager.Instance.GetComponentValue("HeartStability");

            // High blood pressure increases progression
            float bloodPressureFactor = Mathf.Max(0f, (bloodPressure - 50f) / 50f);
            
            // High stress increases progression
            float stressFactor = Mathf.Max(0f, (stress - 50f) / 50f);
            
            // Low heart stability increases progression
            float heartFactor = Mathf.Max(0f, (50f - heartStability) / 50f);

            float totalProgression = 
                (bloodPressureFactor * bloodPressureWeight) +
                (stressFactor * stressWeight) +
                (heartFactor * heartStabilityWeight);

            // Critical conditions accelerate progression
            if (bloodPressure > criticalBloodPressure || stress > criticalStress)
            {
                totalProgression *= 2f;
            }

            return totalProgression;
        }

        protected override void ApplyStageEffects()
        {
            base.ApplyStageEffects();

            if (BodyComponentManager.Instance == null) return;

            switch (currentStage)
            {
                case DiseaseStage.Stage1:
                    BodyComponentManager.Instance.ModifyComponent("HeartStability", -0.5f);
                    break;

                case DiseaseStage.Stage2:
                    BodyComponentManager.Instance.ModifyComponent("HeartStability", -1f);
                    BodyComponentManager.Instance.ModifyComponent("Energy", -0.5f);
                    break;

                case DiseaseStage.Stage3:
                    BodyComponentManager.Instance.ModifyComponent("HeartStability", -2f);
                    BodyComponentManager.Instance.ModifyComponent("Energy", -1f);
                    BodyComponentManager.Instance.ModifyComponent("OxygenLevel", -1f);
                    break;

                case DiseaseStage.Critical:
                    BodyComponentManager.Instance.ModifyComponent("HeartStability", -3f);
                    BodyComponentManager.Instance.ModifyComponent("Energy", -2f);
                    BodyComponentManager.Instance.ModifyComponent("OxygenLevel", -2f);
                    BodyComponentManager.Instance.ModifyComponent("ImmuneStrength", -1f);
                    break;
            }
        }
    }
}
