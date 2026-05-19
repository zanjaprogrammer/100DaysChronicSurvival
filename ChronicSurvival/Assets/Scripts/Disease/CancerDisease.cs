using UnityEngine;
using ChronicSurvival.BodyComponents;

namespace ChronicSurvival.Disease
{
    [CreateAssetMenu(fileName = "CancerDisease", menuName = "ChronicSurvival/Diseases/Cancer")]
    public class CancerDisease : Disease
    {
        [Header("Cancer Specific")]
        [SerializeField] private float immuneStrengthWeight = 0.4f;
        [SerializeField] private float inflammationWeight = 0.3f;
        [SerializeField] private float toxicityWeight = 0.2f;
        [SerializeField] private float stressWeight = 0.1f;

        [Header("Thresholds")]
        [SerializeField] private float criticalImmuneStrength = 30f;
        [SerializeField] private float criticalInflammation = 80f;

        protected override float CalculateProgressionDelta()
        {
            if (BodyComponentManager.Instance == null)
                return 0f;

            float immuneStrength = BodyComponentManager.Instance.GetComponentValue("ImmuneStrength");
            float inflammation = BodyComponentManager.Instance.GetComponentValue("Inflammation");
            float toxicity = BodyComponentManager.Instance.GetComponentValue("Toxicity");
            float stress = BodyComponentManager.Instance.GetComponentValue("Stress");

            // Low immune strength increases progression
            float immuneFactor = Mathf.Max(0f, (50f - immuneStrength) / 50f);
            
            // High inflammation increases progression
            float inflammationFactor = Mathf.Max(0f, (inflammation - 50f) / 50f);
            
            // High toxicity increases progression
            float toxicityFactor = Mathf.Max(0f, (toxicity - 50f) / 50f);
            
            // High stress increases progression
            float stressFactor = Mathf.Max(0f, (stress - 50f) / 50f);

            float totalProgression = 
                (immuneFactor * immuneStrengthWeight) +
                (inflammationFactor * inflammationWeight) +
                (toxicityFactor * toxicityWeight) +
                (stressFactor * stressWeight);

            // Critical conditions accelerate progression
            if (immuneStrength < criticalImmuneStrength || inflammation > criticalInflammation)
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
                    BodyComponentManager.Instance.ModifyComponent("ImmuneStrength", -0.5f);
                    BodyComponentManager.Instance.ModifyComponent("Energy", -0.3f);
                    break;

                case DiseaseStage.Stage2:
                    BodyComponentManager.Instance.ModifyComponent("ImmuneStrength", -1f);
                    BodyComponentManager.Instance.ModifyComponent("Energy", -0.7f);
                    BodyComponentManager.Instance.ModifyComponent("Metabolism", -0.5f);
                    break;

                case DiseaseStage.Stage3:
                    BodyComponentManager.Instance.ModifyComponent("ImmuneStrength", -2f);
                    BodyComponentManager.Instance.ModifyComponent("Energy", -1.5f);
                    BodyComponentManager.Instance.ModifyComponent("Metabolism", -1f);
                    BodyComponentManager.Instance.ModifyComponent("Toxicity", 1f);
                    break;

                case DiseaseStage.Critical:
                    BodyComponentManager.Instance.ModifyComponent("ImmuneStrength", -3f);
                    BodyComponentManager.Instance.ModifyComponent("Energy", -2.5f);
                    BodyComponentManager.Instance.ModifyComponent("Metabolism", -2f);
                    BodyComponentManager.Instance.ModifyComponent("Toxicity", 2f);
                    BodyComponentManager.Instance.ModifyComponent("Health", -1f);
                    break;
            }
        }
    }
}
