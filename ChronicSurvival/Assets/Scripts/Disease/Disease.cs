using UnityEngine;
using ChronicSurvival.Core;
using ChronicSurvival.Units;

namespace ChronicSurvival.Disease
{
    /// <summary>
    /// Base class for all diseases (Diabetes, Hypertension, Cancer)
    /// Handles progression, stages, and triggers
    /// </summary>
    public abstract class Disease : ScriptableObject
    {
        [Header("Disease Info")]
        [SerializeField] protected string diseaseName;
        [SerializeField] protected Units.DiseaseType diseaseType;
        [TextArea(3, 5)]
        [SerializeField] protected string description;

        [Header("Progression")]
        [SerializeField] protected float progressionRate = 1f;
        [SerializeField] protected float currentProgression = 0f; // 0-100
        [SerializeField] protected DiseaseStage currentStage = DiseaseStage.None;

        [Header("Stage Thresholds")]
        [SerializeField] protected float stage1Threshold = 25f;
        [SerializeField] protected float stage2Threshold = 50f;
        [SerializeField] protected float stage3Threshold = 75f;
        [SerializeField] protected float stage4Threshold = 90f;

        [Header("Effects")]
        [SerializeField] protected float damageMultiplier = 1f;
        [SerializeField] protected float spawnRateMultiplier = 1f;

        // Events
        public System.Action<float> OnProgressionChanged;
        public System.Action<DiseaseStage> OnStageChanged;
        public System.Action OnDiseaseCritical;

        // Properties
        public string DiseaseName => diseaseName;
        public Units.DiseaseType DiseaseType => diseaseType;
        public float CurrentProgression => currentProgression;
        public DiseaseStage CurrentStage => currentStage;
        public float DamageMultiplier => damageMultiplier;
        public float SpawnRateMultiplier => spawnRateMultiplier;

        public virtual void Initialize()
        {
            currentProgression = 0f;
            currentStage = DiseaseStage.None;
            damageMultiplier = 1f;
            spawnRateMultiplier = 1f;
        }

        public virtual void UpdateProgression(float deltaTime)
        {
            float oldProgression = currentProgression;
            
            // Calculate progression based on body components
            float progressionDelta = CalculateProgressionDelta() * progressionRate * deltaTime;
            currentProgression = Mathf.Clamp(currentProgression + progressionDelta, 0f, 100f);

            if (Mathf.Abs(currentProgression - oldProgression) > 0.01f)
            {
                OnProgressionChanged?.Invoke(currentProgression);
                EventManager.TriggerEvent(GameEvents.DISEASE_PROGRESSED, this);
            }

            UpdateStage();
        }

        protected virtual void UpdateStage()
        {
            DiseaseStage newStage = CalculateStage();

            if (newStage != currentStage)
            {
                DiseaseStage oldStage = currentStage;
                currentStage = newStage;

                OnStageChanged?.Invoke(currentStage);
                EventManager.TriggerEvent(GameEvents.DISEASE_LEVEL_CHANGED, this);

                ApplyStageEffects();

                if (currentStage == DiseaseStage.Critical)
                {
                    OnDiseaseCritical?.Invoke();
                }

                Debug.Log($"[Disease] {diseaseName} stage changed: {oldStage} → {newStage}");
            }
        }

        protected virtual DiseaseStage CalculateStage()
        {
            if (currentProgression >= stage4Threshold)
                return DiseaseStage.Critical;
            else if (currentProgression >= stage3Threshold)
                return DiseaseStage.Stage3;
            else if (currentProgression >= stage2Threshold)
                return DiseaseStage.Stage2;
            else if (currentProgression >= stage1Threshold)
                return DiseaseStage.Stage1;
            else
                return DiseaseStage.None;
        }

        protected abstract float CalculateProgressionDelta();

        protected virtual void ApplyStageEffects()
        {
            switch (currentStage)
            {
                case DiseaseStage.None:
                    damageMultiplier = 1f;
                    spawnRateMultiplier = 1f;
                    break;

                case DiseaseStage.Stage1:
                    damageMultiplier = 1.2f;
                    spawnRateMultiplier = 1.1f;
                    break;

                case DiseaseStage.Stage2:
                    damageMultiplier = 1.5f;
                    spawnRateMultiplier = 1.3f;
                    break;

                case DiseaseStage.Stage3:
                    damageMultiplier = 2f;
                    spawnRateMultiplier = 1.6f;
                    break;

                case DiseaseStage.Critical:
                    damageMultiplier = 3f;
                    spawnRateMultiplier = 2f;
                    break;
            }
        }

        public virtual void ReduceProgression(float amount)
        {
            currentProgression = Mathf.Max(0f, currentProgression - amount);
            OnProgressionChanged?.Invoke(currentProgression);
            UpdateStage();
        }

        public virtual void IncreaseProgression(float amount)
        {
            currentProgression = Mathf.Min(100f, currentProgression + amount);
            OnProgressionChanged?.Invoke(currentProgression);
            UpdateStage();
        }

        public virtual bool IsCritical()
        {
            return currentStage == DiseaseStage.Critical;
        }

        public virtual string GetStageDescription()
        {
            switch (currentStage)
            {
                case DiseaseStage.None:
                    return "Healthy";
                case DiseaseStage.Stage1:
                    return "Early Stage";
                case DiseaseStage.Stage2:
                    return "Moderate";
                case DiseaseStage.Stage3:
                    return "Advanced";
                case DiseaseStage.Critical:
                    return "Critical";
                default:
                    return "Unknown";
            }
        }
    }

    public enum DiseaseStage
    {
        None,
        Stage1,
        Stage2,
        Stage3,
        Critical
    }
}
