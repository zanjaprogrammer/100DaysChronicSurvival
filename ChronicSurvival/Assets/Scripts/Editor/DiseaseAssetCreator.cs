using UnityEngine;
using UnityEditor;
using ChronicSurvival.Disease;

namespace ChronicSurvival.Editor
{
    public class DiseaseAssetCreator
    {
        [MenuItem("ChronicSurvival/Create Disease Assets")]
        public static void CreateDiseaseAssets()
        {
            // Create Diabetes
            var diabetes = ScriptableObject.CreateInstance<DiabetesDisease>();
            AssetDatabase.CreateAsset(diabetes, "Assets/Data/Diseases/Diabetes.asset");

            // Create Hypertension
            var hypertension = ScriptableObject.CreateInstance<HypertensionDisease>();
            AssetDatabase.CreateAsset(hypertension, "Assets/Data/Diseases/Hypertension.asset");

            // Create Cancer
            var cancer = ScriptableObject.CreateInstance<CancerDisease>();
            AssetDatabase.CreateAsset(cancer, "Assets/Data/Diseases/Cancer.asset");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[DiseaseAssetCreator] Created 3 disease assets in Assets/Data/Diseases/");
        }
    }
}
