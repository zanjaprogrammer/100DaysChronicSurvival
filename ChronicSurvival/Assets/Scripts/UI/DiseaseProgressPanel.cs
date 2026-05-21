using UnityEngine;
using System.Collections.Generic;
using ChronicSurvival.Disease;
using ChronicSurvival.Units;

namespace ChronicSurvival.UI
{
    /// <summary>
    /// Manages the HUD display of all diseases' progress bars.
    /// Wire-compatible with the field in UIManager.cs.
    /// </summary>
    public class DiseaseProgressPanel : MonoBehaviour
    {
        [Header("Prefabs & Layout")]
        [SerializeField] private GameObject diseaseProgressPrefab;
        [SerializeField] private Transform container;

        private Dictionary<Units.DiseaseType, DiseaseProgressUI> activeBars = new Dictionary<Units.DiseaseType, DiseaseProgressUI>();

        private void Start()
        {
            InitializeProgressBars();
        }

        private void OnEnable()
        {
            // If initialized already, refresh visibility
            RefreshProgressBars();
        }

        private void InitializeProgressBars()
        {
            // Clear existing elements in the container
            if (container != null)
            {
                foreach (Transform child in container)
                {
                    Destroy(child.gameObject);
                }
            }
            activeBars.Clear();

            if (DiseaseManager.Instance == null)
            {
                Debug.LogWarning("[DiseaseProgressPanel] DiseaseManager Instance is missing!");
                return;
            }

            if (diseaseProgressPrefab == null || container == null)
            {
                Debug.LogWarning("[DiseaseProgressPanel] Prefab or Container not assigned!");
                return;
            }

            var allDiseases = DiseaseManager.Instance.GetAllDiseases();

            foreach (var kvp in allDiseases)
            {
                Units.DiseaseType type = kvp.Key;
                ChronicSurvival.Disease.Disease disease = kvp.Value;

                if (disease == null) continue;

                GameObject barObj = Instantiate(diseaseProgressPrefab, container);
                DiseaseProgressUI progressUI = barObj.GetComponent<DiseaseProgressUI>();

                if (progressUI != null)
                {
                    progressUI.Initialize(disease);
                    activeBars[type] = progressUI;
                }
                else
                {
                    Debug.LogError("[DiseaseProgressPanel] Prefab does not have a DiseaseProgressUI component!");
                    Destroy(barObj);
                }
            }
        }

        private void RefreshProgressBars()
        {
            // In case we want to re-initialize on enable, or just double check
            if (activeBars.Count == 0 && DiseaseManager.Instance != null && diseaseProgressPrefab != null && container != null)
            {
                InitializeProgressBars();
            }
        }
    }
}
