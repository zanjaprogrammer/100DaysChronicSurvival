using UnityEngine;
using System.Collections.Generic;
using ChronicSurvival.Core;

namespace ChronicSurvival.Disease
{
    public class DiseaseManager : MonoBehaviour
    {
        public static DiseaseManager Instance { get; private set; }

        [Header("Disease Assets")]
        [SerializeField] private DiabetesDisease diabetesDisease;
        [SerializeField] private HypertensionDisease hypertensionDisease;
        [SerializeField] private CancerDisease cancerDisease;

        [Header("Settings")]
        [SerializeField] private bool updateDiseases = false;
        [SerializeField] private bool triggerGameOverOnCritical = false;
        [SerializeField] private float updateInterval = 1f;

        [Header("Debug")]
        [SerializeField] private bool debugMode = true;

        private Dictionary<Units.DiseaseType, Disease> diseases = new Dictionary<Units.DiseaseType, Disease>();
        private float updateTimer;

        public System.Action<Disease, DiseaseStage> OnAnyDiseaseStageChanged;
        public System.Action<Disease> OnAnyDiseaseCritical;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeDiseases();
        }

        private void InitializeDiseases()
        {
            EnsureRuntimeDefaultDiseases();
            RegisterDisease(Units.DiseaseType.Diabetes, diabetesDisease);
            RegisterDisease(Units.DiseaseType.Hypertension, hypertensionDisease);
            RegisterDisease(Units.DiseaseType.Cancer, cancerDisease);

            if (debugMode) Debug.Log($"[DiseaseManager] Initialized {diseases.Count} diseases");
        }

        private void EnsureRuntimeDefaultDiseases()
        {
            if (diabetesDisease == null)
            {
                diabetesDisease = CreateRuntimeDisease<DiabetesDisease>("Diabetes", Units.DiseaseType.Diabetes, "Gangguan metabolisme gula darah dan efisiensi insulin.");
            }

            if (hypertensionDisease == null)
            {
                hypertensionDisease = CreateRuntimeDisease<HypertensionDisease>("Hypertension", Units.DiseaseType.Hypertension, "Tekanan darah tinggi yang membebani stabilitas jantung.");
            }

            if (cancerDisease == null)
            {
                cancerDisease = CreateRuntimeDisease<CancerDisease>("Cancer", Units.DiseaseType.Cancer, "Pertumbuhan sel abnormal yang menekan sistem imun tubuh.");
            }
        }

        private T CreateRuntimeDisease<T>(string diseaseName, Units.DiseaseType type, string description) where T : Disease
        {
            T disease = ScriptableObject.CreateInstance<T>();
            disease.ConfigureRuntimeDefaults(diseaseName, type, description);
            return disease;
        }

        private void RegisterDisease(Units.DiseaseType type, Disease disease)
        {
            if (disease == null) return;

            disease.Initialize();
            diseases[type] = disease;
            disease.OnStageChanged += (stage) => OnDiseaseStageChanged(disease, stage);
            disease.OnDiseaseCritical += () => OnDiseaseCritical(disease);
        }

        private void Update()
        {
            if (!updateDiseases) return;

            updateTimer += Time.deltaTime;

            if (updateTimer >= updateInterval)
            {
                UpdateAllDiseases(updateTimer);
                updateTimer = 0f;
            }
        }

        private void UpdateAllDiseases(float deltaTime)
        {
            foreach (var disease in diseases.Values)
            {
                disease.UpdateProgression(deltaTime);
            }
        }

        public Disease GetDisease(Units.DiseaseType type)
        {
            return diseases.ContainsKey(type) ? diseases[type] : null;
        }

        public float GetDiseaseProgression(Units.DiseaseType type)
        {
            Disease disease = GetDisease(type);
            return disease != null ? disease.CurrentProgression : 0f;
        }

        public DiseaseStage GetDiseaseStage(Units.DiseaseType type)
        {
            Disease disease = GetDisease(type);
            return disease != null ? disease.CurrentStage : DiseaseStage.None;
        }

        public bool IsAnyDiseaseCritical()
        {
            foreach (var disease in diseases.Values)
            {
                if (disease.IsCritical())
                    return true;
            }
            return false;
        }

        public void ReduceDiseaseProgression(Units.DiseaseType type, float amount)
        {
            Disease disease = GetDisease(type);
            if (disease != null)
            {
                disease.ReduceProgression(amount);
                if (debugMode) Debug.Log($"[DiseaseManager] Reduced {type} by {amount}");
            }
        }

        public void IncreaseDiseaseProgression(Units.DiseaseType type, float amount)
        {
            Disease disease = GetDisease(type);
            if (disease != null)
            {
                disease.IncreaseProgression(amount);
                if (debugMode) Debug.Log($"[DiseaseManager] Increased {type} by {amount}");
            }
        }

        private void OnDiseaseStageChanged(Disease disease, DiseaseStage newStage)
        {
            OnAnyDiseaseStageChanged?.Invoke(disease, newStage);

            if (debugMode)
            {
                Debug.Log($"[DiseaseManager] {disease.DiseaseName} reached {newStage}");
            }
        }

        private void OnDiseaseCritical(Disease disease)
        {
            OnAnyDiseaseCritical?.Invoke(disease);

            if (debugMode)
            {
                Debug.LogWarning($"[DiseaseManager] {disease.DiseaseName} is CRITICAL!");
            }

            if (triggerGameOverOnCritical && GameManager.Instance != null)
            {
                GameManager.Instance.ChangeState(GameState.GameOver);
            }
        }

        public Dictionary<Units.DiseaseType, Disease> GetAllDiseases()
        {
            return diseases;
        }

        public void ResetAllDiseases()
        {
            foreach (var disease in diseases.Values)
            {
                disease.Initialize();
            }

            if (debugMode) Debug.Log("[DiseaseManager] Reset all diseases");
        }
    }
}
