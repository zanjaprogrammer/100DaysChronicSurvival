using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Collections.Generic;
using ChronicSurvival.Core;
using ChronicSurvival.Arena;
using ChronicSurvival.Cards;
using ChronicSurvival.BodyComponents;
using ChronicSurvival.Battle;
using ChronicSurvival.Disease;

namespace ChronicSurvival.Editor
{
    /// <summary>
    /// Optional editor helper: arena background + managers.
    /// UI is built automatically at runtime by GameplayUICreator (NeonShooter-style).
    /// </summary>
    public class SceneSetupHelper : EditorWindow
    {
        [MenuItem("Chronic Survival/Complete Scene Setup")]
        public static void SetupScene()
        {
            Debug.Log("[SceneSetupHelper] Setup managers + arena (UI dibuat otomatis saat Play)...");

            Canvas canvas = Object.FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                canvasObj.AddComponent<ChronicSurvival.UI.GameplayUIInstaller>();
                Undo.RegisterCreatedObjectUndo(canvasObj, "Create Canvas");
            }

            var scaler = canvas.GetComponent<UnityEngine.UI.CanvasScaler>();
            if (scaler != null)
            {
                scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
            }

            RectTransform canvasRt = canvas.GetComponent<RectTransform>();
            if (canvasRt != null && canvasRt.localScale == Vector3.zero)
            {
                canvasRt.localScale = Vector3.one;
            }

            if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject esObj = new GameObject("EventSystem");
                esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                Undo.RegisterCreatedObjectUndo(esObj, "Create EventSystem");
            }

            SetupArenaBackground();

            GameObject managersObj = GameObject.Find("_Managers");
            if (managersObj == null)
            {
                managersObj = new GameObject("_Managers");
                Undo.RegisterCreatedObjectUndo(managersObj, "Create Managers");
            }

            GetOrAddComponent<GameManager>(managersObj);
            GetOrAddComponent<EventManager>(managersObj);
            GetOrAddComponent<AudioManager>(managersObj);
            GetOrAddComponent<PoolManager>(managersObj);
            GetOrAddComponent<SceneLoader>(managersObj);
            GetOrAddComponent<BodyComponentManager>(managersObj);
            CardManager cardManager = GetOrAddComponent<CardManager>(managersObj);
            GetOrAddComponent<CardBuffManager>(managersObj);
            GetOrAddComponent<DiseaseManager>(managersObj);
            GetOrAddComponent<BattleManager>(managersObj);

            WireCardManagerFromResources(cardManager);

            // Persist UI in scene for editor preview (optional — Play Mode rebuilds anyway)
            GameplayUILayoutSetup.BuildAll(showDialog: false);

            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

            EditorUtility.DisplayDialog("Setup Complete",
                "Arena + manager siap.\n\n" +
                "UI tidak perlu di-setup manual lagi — tekan Play dan GameplayUICreator akan membangun semua panel secara otomatis (seperti NeonShooter).",
                "OK");
        }

        private static void SetupArenaBackground()
        {
            GameObject bgObj = GameObject.Find("ArenaBackground");
            if (bgObj == null)
            {
                bgObj = new GameObject("ArenaBackground");
                Undo.RegisterCreatedObjectUndo(bgObj, "Create ArenaBackground");
            }

            SpriteRenderer sr = GetOrAddComponent<SpriteRenderer>(bgObj);
            ArenaSetup setup = GetOrAddComponent<ArenaSetup>(bgObj);
            ArenaWalkableMask mask = GetOrAddComponent<ArenaWalkableMask>(bgObj);

            Sprite arenaSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Background/ArenaBranched.png");
            Texture2D maskTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Art/Background/ArenaBranched_WalkableMask.png");

            if (arenaSprite == null) Debug.LogError("[SceneSetupHelper] Arena sprite not found!");
            if (maskTexture == null) Debug.LogError("[SceneSetupHelper] Walkable mask not found!");

            SerializedObject setupSO = new SerializedObject(setup);
            setupSO.FindProperty("arenaSprite").objectReferenceValue = arenaSprite;
            setupSO.FindProperty("sortingOrder").intValue = -100;
            setupSO.FindProperty("autoScaleToCamera").boolValue = true;
            setupSO.ApplyModifiedProperties();

            SerializedObject maskSO = new SerializedObject(mask);
            maskSO.FindProperty("walkableMask").objectReferenceValue = maskTexture;
            maskSO.FindProperty("arenaBoundsMin").vector2Value = new Vector2(-9.6f, -5.4f);
            maskSO.FindProperty("arenaBoundsMax").vector2Value = new Vector2(9.6f, 5.4f);
            maskSO.FindProperty("walkableThreshold").floatValue = 0.3f;
            maskSO.FindProperty("debugMode").boolValue = true;
            maskSO.ApplyModifiedProperties();
        }

        private static void WireCardManagerFromResources(CardManager cardManager)
        {
            if (cardManager == null) return;

            var database = AssetDatabase.LoadAssetAtPath<ActionCardDatabase>("Assets/Resources/ActionCardDatabase.asset");
            var guids = AssetDatabase.FindAssets("t:ActionCard", new[] { "Assets/Resources/Cards" });
            var cards = new List<ActionCard>();
            foreach (var guid in guids)
            {
                var card = AssetDatabase.LoadAssetAtPath<ActionCard>(AssetDatabase.GUIDToAssetPath(guid));
                if (card != null && card.cardType != CardType.Debuff)
                {
                    cards.Add(card);
                }
            }

            SerializedObject so = new SerializedObject(cardManager);
            so.FindProperty("cardDatabase").objectReferenceValue = database;
            var listProp = so.FindProperty("allCards");
            listProp.ClearArray();
            for (int i = 0; i < cards.Count; i++)
            {
                listProp.InsertArrayElementAtIndex(i);
                listProp.GetArrayElementAtIndex(i).objectReferenceValue = cards[i];
            }
            so.ApplyModifiedProperties();
        }

        private static T GetOrAddComponent<T>(GameObject obj) where T : Component
        {
            T comp = obj.GetComponent<T>();
            if (comp == null)
            {
                comp = obj.AddComponent<T>();
            }
            return comp;
        }
    }
}
