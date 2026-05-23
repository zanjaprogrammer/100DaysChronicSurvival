using UnityEditor;
using UnityEngine;
using ChronicSurvival.UI;

namespace ChronicSurvival.Editor
{
    /// <summary>
    /// Editor shortcut — delegates to runtime GameplayUICreator (same as Play Mode auto-build).
    /// </summary>
    public static class GameplayUILayoutSetup
    {
        [MenuItem("ChronicSurvival/Build Gameplay UI v2")]
        public static void BuildAllMenu()
        {
            BuildAll(showDialog: true);
        }

        public static void BuildAll(bool showDialog = false)
        {
            Canvas canvas = Object.FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                canvas = GameplayUICreator.EnsureCanvas();
            }

            GameplayUICreator.BuildAll(canvas.transform);

            if (canvas.GetComponent<GameplayUIInstaller>() == null)
            {
                var inst = canvas.gameObject.AddComponent<GameplayUIInstaller>();
                var instSO = new SerializedObject(inst);
                instSO.FindProperty("autoBuildIfMissing").boolValue = false;
                instSO.ApplyModifiedProperties();
            }

            EditorUtility.SetDirty(canvas.gameObject);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene());

            if (showDialog)
            {
                EditorUtility.DisplayDialog("UI v2",
                    "Gameplay UI dibangun (runtime code, sama seperti NeonShooter).\n\n" +
                    "Saat Play, UI juga otomatis dibuat — tidak perlu Complete Scene Setup.", "OK");
            }
        }
    }
}
