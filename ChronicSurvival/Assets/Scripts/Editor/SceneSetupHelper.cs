using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using ChronicSurvival.Core;
using ChronicSurvival.UI;
using ChronicSurvival.Arena;

namespace ChronicSurvival.Editor
{
    /// <summary>
    /// Editor tool that programmatically builds the complete game scene with premium UI.
    /// Menu: Chronic Survival > Complete Scene Setup
    /// </summary>
    public class SceneSetupHelper : EditorWindow
    {
        // ─────────────── Color Palette ───────────────
        static readonly Color COL_PANEL_BG    = new Color(0.08f, 0.04f, 0.06f, 0.92f);   // Dark crimson-black
        static readonly Color COL_PANEL_BORDER = new Color(0.55f, 0.15f, 0.20f, 0.6f);    // Muted red border
        static readonly Color COL_HEADER      = new Color(0.95f, 0.85f, 0.80f, 1f);       // Warm off-white
        static readonly Color COL_SUBHEADER   = new Color(0.80f, 0.55f, 0.55f, 1f);       // Warm muted pink
        static readonly Color COL_VALUE       = new Color(0.95f, 0.95f, 0.90f, 1f);       // Bright text
        static readonly Color COL_BAR_BG      = new Color(0.12f, 0.06f, 0.08f, 0.85f);    // Dark bar background
        static readonly Color COL_GREEN       = new Color(0.20f, 0.85f, 0.40f, 1f);       // Stat bar green
        static readonly Color COL_RED         = new Color(0.95f, 0.20f, 0.20f, 1f);       // Stat bar red
        static readonly Color COL_ORANGE      = new Color(1f, 0.65f, 0.15f, 1f);          // Warning orange
        static readonly Color COL_BUTTON_BG   = new Color(0.55f, 0.10f, 0.15f, 1f);       // Button red
        static readonly Color COL_BUTTON_QUIT = new Color(0.20f, 0.10f, 0.12f, 1f);       // Dark button
        static readonly Color COL_TRANSPARENT = new Color(0f, 0f, 0f, 0f);
        static readonly Color COL_CARD_BG     = new Color(0.12f, 0.06f, 0.10f, 0.95f);    // Card selection bg
        static readonly Color COL_OVERLAY     = new Color(0f, 0f, 0f, 0.70f);             // Overlay dim

        [MenuItem("Chronic Survival/Complete Scene Setup")]
        public static void SetupScene()
        {
            Debug.Log("[SceneSetupHelper] Starting premium scene setup...");

            // 1. Canvas
            Canvas canvas = FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
                Undo.RegisterCreatedObjectUndo(canvasObj, "Create Canvas");
            }

            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
            }

            // EventSystem
            UnityEngine.EventSystems.EventSystem eventSystem = FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>();
            if (eventSystem == null)
            {
                GameObject esObj = new GameObject("EventSystem");
                esObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                Undo.RegisterCreatedObjectUndo(esObj, "Create EventSystem");
            }

            // 2. Background
            SetupArenaBackground();

            // 3. Managers
            GameObject managersObj = GameObject.Find("_Managers");
            if (managersObj == null)
            {
                managersObj = new GameObject("_Managers");
                Undo.RegisterCreatedObjectUndo(managersObj, "Create Managers");
            }

            GameManager gameManager = GetOrAddComponent<GameManager>(managersObj);
            EventManager eventManager = GetOrAddComponent<EventManager>(managersObj);
            AudioManager audioManager = GetOrAddComponent<AudioManager>(managersObj);
            PoolManager poolManager = GetOrAddComponent<PoolManager>(managersObj);
            SceneLoader sceneLoader = GetOrAddComponent<SceneLoader>(managersObj);
            UIManager uiManager = GetOrAddComponent<UIManager>(managersObj);

            // 4. Load Prefabs
            GameObject cardUiPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/CardUI.prefab");
            GameObject statBarPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/BodyStatBar.prefab");
            GameObject diseaseProgressPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/DiseaseProgressUI.prefab");

            if (cardUiPrefab == null) Debug.LogWarning("[SceneSetupHelper] CardUI prefab not found - card selection will be text only");
            if (statBarPrefab == null) Debug.LogWarning("[SceneSetupHelper] BodyStatBar prefab not found - stats will use fallback");
            if (diseaseProgressPrefab == null) Debug.LogWarning("[SceneSetupHelper] DiseaseProgressUI prefab not found");

            // 5. Build UI Panels
            GameObject mainMenuPanel      = SetupMainMenuPanel(canvas.transform);
            GameObject battleHUDPanel     = SetupBattleHUDPanel(canvas.transform, statBarPrefab, diseaseProgressPrefab);
            GameObject cardSelectionPanel = SetupCardSelectionPanel(canvas.transform, cardUiPrefab);
            GameObject randomEventPanel   = SetupRandomEventPanel(canvas.transform);
            GameObject gameOverPanel      = SetupGameOverPanel(canvas.transform);
            GameObject victoryPanel       = SetupVictoryPanel(canvas.transform);
            GameObject pausePanel         = SetupPausePanel(canvas.transform);

            // 6. Wire UIManager
            SerializedObject uiManagerSO = new SerializedObject(uiManager);
            
            uiManagerSO.FindProperty("mainMenuPanel").objectReferenceValue = mainMenuPanel;
            uiManagerSO.FindProperty("battleHUDPanel").objectReferenceValue = battleHUDPanel;
            uiManagerSO.FindProperty("cardSelectionPanel").objectReferenceValue = cardSelectionPanel;
            uiManagerSO.FindProperty("randomEventPanel").objectReferenceValue = randomEventPanel;
            uiManagerSO.FindProperty("gameOverPanel").objectReferenceValue = gameOverPanel;
            uiManagerSO.FindProperty("victoryPanel").objectReferenceValue = victoryPanel;
            uiManagerSO.FindProperty("pausePanel").objectReferenceValue = pausePanel;

            if (battleHUDPanel != null)
            {
                uiManagerSO.FindProperty("dayCounter").objectReferenceValue = battleHUDPanel.GetComponentInChildren<DayCounterUI>(true);
                uiManagerSO.FindProperty("bodyStatsPanel").objectReferenceValue = battleHUDPanel.GetComponentInChildren<BodyStatsPanel>(true);
                uiManagerSO.FindProperty("diseaseProgressPanel").objectReferenceValue = battleHUDPanel.GetComponentInChildren<DiseaseProgressPanel>(true);
                uiManagerSO.FindProperty("topStatusBar").objectReferenceValue = battleHUDPanel.GetComponentInChildren<TopStatusBarUI>(true);
                uiManagerSO.FindProperty("activeEventUI").objectReferenceValue = battleHUDPanel.GetComponentInChildren<ActiveEventUI>(true);
            }

            uiManagerSO.ApplyModifiedProperties();

            // Set panels inactive
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (battleHUDPanel != null) battleHUDPanel.SetActive(false);
            if (cardSelectionPanel != null) cardSelectionPanel.SetActive(false);
            if (randomEventPanel != null) randomEventPanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (victoryPanel != null) victoryPanel.SetActive(false);
            if (pausePanel != null) pausePanel.SetActive(false);

            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

            Debug.Log("[SceneSetupHelper] Premium UI setup complete!");
            EditorUtility.DisplayDialog("Setup Complete", 
                "Premium UI has been built!\n\n" +
                "• Battle HUD with top status bar, round timer, body stats, disease progress\n" +
                "• Card selection as overlay on battle HUD\n" +
                "• Active event indicator\n" +
                "• All panels styled with premium dark theme\n\n" +
                "Press Play to start!", "OK");
        }

        // ════════════════════════════════════════════════════════════════════
        //  ARENA BACKGROUND
        // ════════════════════════════════════════════════════════════════════

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

            if (arenaSprite != null) sr.sprite = arenaSprite;
            sr.sortingOrder = -100;
        }

        // ════════════════════════════════════════════════════════════════════
        //  MAIN MENU PANEL
        // ════════════════════════════════════════════════════════════════════

        private static GameObject SetupMainMenuPanel(Transform parent)
        {
            GameObject panel = GetOrCreatePanel(parent, "MainMenuPanel");
            StylePanelBackground(panel, new Color(0.06f, 0.02f, 0.03f, 0.97f));

            MainMenuUI menuUI = GetOrAddComponent<MainMenuUI>(panel);

            // Vignette / gradient overlay
            GameObject vignetteObj = CreateUIElement(panel.transform, "Vignette");
            RectTransform vigRect = vignetteObj.GetComponent<RectTransform>();
            vigRect.anchorMin = Vector2.zero;
            vigRect.anchorMax = Vector2.one;
            vigRect.sizeDelta = Vector2.zero;
            Image vigImg = GetOrAddComponent<Image>(vignetteObj);
            vigImg.color = new Color(0.08f, 0.02f, 0.04f, 0.5f);

            // Title
            GameObject titleObj = CreateText(panel.transform, "TitleText", "CHRONIC SURVIVAL", 72f, COL_HEADER, new Vector2(0, 180));
            TextMeshProUGUI titleTMP = titleObj.GetComponent<TextMeshProUGUI>();
            titleTMP.fontStyle = FontStyles.Bold;

            // Subtitle
            CreateText(panel.transform, "SubtitleText", 
                "Pertahankan tubuh dari ancaman penyakit kronis.\nBertahan 100 hari.", 
                24f, COL_SUBHEADER, new Vector2(0, 90));

            // Decorative line
            GameObject lineObj = CreateUIElement(panel.transform, "DecorLine");
            RectTransform lineRect = lineObj.GetComponent<RectTransform>();
            lineRect.anchoredPosition = new Vector2(0, 40);
            lineRect.sizeDelta = new Vector2(400, 2);
            Image lineImg = GetOrAddComponent<Image>(lineObj);
            lineImg.color = COL_PANEL_BORDER;

            // Buttons
            GameObject startBtnObj = CreateStyledButton(panel.transform, "StartButton", "▶  MULAI PERMAINAN", 
                new Vector2(0, -40), new Vector2(380, 65), COL_BUTTON_BG);
            GameObject quitBtnObj = CreateStyledButton(panel.transform, "QuitButton", "✕  KELUAR", 
                new Vector2(0, -130), new Vector2(380, 55), COL_BUTTON_QUIT);

            // Version text
            CreateText(panel.transform, "VersionText", "v0.1 Alpha — 100 Days Chronic Survival", 
                14f, new Color(0.4f, 0.3f, 0.3f, 0.7f), new Vector2(0, -420));

            SerializedObject menuSO = new SerializedObject(menuUI);
            menuSO.FindProperty("startButton").objectReferenceValue = startBtnObj.GetComponent<Button>();
            menuSO.FindProperty("quitButton").objectReferenceValue = quitBtnObj.GetComponent<Button>();
            menuSO.ApplyModifiedProperties();

            return panel;
        }

        // ════════════════════════════════════════════════════════════════════
        //  BATTLE HUD PANEL (main gameplay UI)
        // ════════════════════════════════════════════════════════════════════

        private static GameObject SetupBattleHUDPanel(Transform parent, GameObject statBarPrefab, GameObject diseaseProgressPrefab)
        {
            GameObject panel = GetOrCreatePanel(parent, "BattleHUDPanel");
            StylePanelBackground(panel, COL_TRANSPARENT);

            // ═══ TOP-LEFT: Round Counter + Pause ═══
            GameObject topLeftObj = CreateSubPanel(panel.transform, "RoundTimerPanel",
                new Vector2(0f, 1f), new Vector2(0f, 1f),    // anchor top-left
                new Vector2(130, -50), new Vector2(220, 90),  // position & size
                COL_PANEL_BG);
            AddPanelBorder(topLeftObj, COL_PANEL_BORDER);

            DayCounterUI dayCounter = GetOrAddComponent<DayCounterUI>(topLeftObj);

            // Pause button
            GameObject pauseBtnObj = CreateUIElement(topLeftObj.transform, "PauseButton");
            RectTransform pauseRect = pauseBtnObj.GetComponent<RectTransform>();
            pauseRect.anchorMin = new Vector2(0f, 0.5f);
            pauseRect.anchorMax = new Vector2(0f, 0.5f);
            pauseRect.anchoredPosition = new Vector2(25, 8);
            pauseRect.sizeDelta = new Vector2(36, 36);
            Image pauseImg = GetOrAddComponent<Image>(pauseBtnObj);
            pauseImg.color = new Color(0.3f, 0.15f, 0.18f, 0.9f);
            Button pauseBtn = GetOrAddComponent<Button>(pauseBtnObj);
            
            // Pause icon text
            GameObject pauseIconText = CreateText(pauseBtnObj.transform, "PauseIcon", "⏸", 20f, COL_HEADER, Vector2.zero);
            RectTransform piRect = pauseIconText.GetComponent<RectTransform>();
            piRect.anchorMin = Vector2.zero;
            piRect.anchorMax = Vector2.one;
            piRect.sizeDelta = Vector2.zero;
            piRect.anchoredPosition = Vector2.zero;

            // Day/Round text
            GameObject dayTextObj = CreateText(topLeftObj.transform, "DayText", "RONDE 1", 24f, COL_HEADER, new Vector2(20, 15));
            TextMeshProUGUI dayTMP = dayTextObj.GetComponent<TextMeshProUGUI>();
            dayTMP.fontStyle = FontStyles.Bold;
            dayTMP.alignment = TextAlignmentOptions.Left;
            RectTransform dayTextRect = dayTextObj.GetComponent<RectTransform>();
            dayTextRect.anchorMin = new Vector2(0.25f, 0.5f);
            dayTextRect.anchorMax = new Vector2(1f, 1f);
            dayTextRect.anchoredPosition = Vector2.zero;
            dayTextRect.sizeDelta = Vector2.zero;

            // Timer text
            GameObject timerTextObj = CreateText(topLeftObj.transform, "TimerText", "⏱ 00:00", 16f, COL_SUBHEADER, new Vector2(20, -15));
            TextMeshProUGUI timerTMP = timerTextObj.GetComponent<TextMeshProUGUI>();
            timerTMP.alignment = TextAlignmentOptions.Left;
            RectTransform timerRect = timerTextObj.GetComponent<RectTransform>();
            timerRect.anchorMin = new Vector2(0.25f, 0f);
            timerRect.anchorMax = new Vector2(1f, 0.5f);
            timerRect.anchoredPosition = Vector2.zero;
            timerRect.sizeDelta = Vector2.zero;

            // Progress bar (thin line at bottom)
            GameObject progressBarBg = CreateUIElement(topLeftObj.transform, "ProgressBarBg");
            RectTransform progBgRect = progressBarBg.GetComponent<RectTransform>();
            progBgRect.anchorMin = new Vector2(0.05f, 0.02f);
            progBgRect.anchorMax = new Vector2(0.95f, 0.08f);
            progBgRect.anchoredPosition = Vector2.zero;
            progBgRect.sizeDelta = Vector2.zero;
            Image progBgImg = GetOrAddComponent<Image>(progressBarBg);
            progBgImg.color = COL_BAR_BG;

            GameObject progressBarFill = CreateUIElement(progressBarBg.transform, "ProgressBar");
            RectTransform progFillRect = progressBarFill.GetComponent<RectTransform>();
            progFillRect.anchorMin = Vector2.zero;
            progFillRect.anchorMax = Vector2.one;
            progFillRect.anchoredPosition = Vector2.zero;
            progFillRect.sizeDelta = Vector2.zero;
            Image progFillImg = GetOrAddComponent<Image>(progressBarFill);
            progFillImg.color = COL_BUTTON_BG;
            progFillImg.type = Image.Type.Filled;
            progFillImg.fillMethod = Image.FillMethod.Horizontal;

            // Hidden progress text for data binding
            GameObject progressTextObj = CreateText(topLeftObj.transform, "ProgressText", "1%", 1f, COL_TRANSPARENT, Vector2.zero);

            // Wire DayCounterUI
            SerializedObject dcSO = new SerializedObject(dayCounter);
            dcSO.FindProperty("dayText").objectReferenceValue = dayTextObj.GetComponent<TextMeshProUGUI>();
            dcSO.FindProperty("progressText").objectReferenceValue = progressTextObj.GetComponent<TextMeshProUGUI>();
            dcSO.FindProperty("progressBar").objectReferenceValue = progFillImg;
            dcSO.FindProperty("timerText").objectReferenceValue = timerTextObj.GetComponent<TextMeshProUGUI>();
            dcSO.FindProperty("pauseButton").objectReferenceValue = pauseBtn;
            dcSO.ApplyModifiedProperties();


            // ═══ TOP-CENTER: Three Summary Status Bars ═══
            GameObject topCenterObj = CreateSubPanel(panel.transform, "TopStatusBar",
                new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0, -50), new Vector2(650, 65),
                COL_PANEL_BG);
            AddPanelBorder(topCenterObj, COL_PANEL_BORDER);

            TopStatusBarUI topStatusBar = GetOrAddComponent<TopStatusBarUI>(topCenterObj);

            // Create 3 horizontal stat slots
            HorizontalLayoutGroup topHLG = GetOrAddComponent<HorizontalLayoutGroup>(topCenterObj);
            topHLG.spacing = 8f;
            topHLG.padding = new RectOffset(12, 12, 8, 8);
            topHLG.childAlignment = TextAnchor.MiddleCenter;
            topHLG.childControlWidth = true;
            topHLG.childControlHeight = true;
            topHLG.childForceExpandWidth = true;
            topHLG.childForceExpandHeight = true;

            var immuneSlot = CreateTopStatSlot(topCenterObj.transform, "ImmuneSlot", "🛡", "SISTEM IMUN", "68%", COL_GREEN);
            var healthSlot = CreateTopStatSlot(topCenterObj.transform, "HealthSlot", "❤", "KESEHATAN", "72%", COL_RED);
            var inflammSlot = CreateTopStatSlot(topCenterObj.transform, "InflammationSlot", "🔥", "INFLAMASI", "45%", COL_ORANGE);

            // Wire TopStatusBarUI
            SerializedObject tsSO = new SerializedObject(topStatusBar);
            tsSO.FindProperty("immuneLabel").objectReferenceValue = immuneSlot.label;
            tsSO.FindProperty("immuneValueText").objectReferenceValue = immuneSlot.value;
            tsSO.FindProperty("immuneFillBar").objectReferenceValue = immuneSlot.fill;
            tsSO.FindProperty("healthLabel").objectReferenceValue = healthSlot.label;
            tsSO.FindProperty("healthValueText").objectReferenceValue = healthSlot.value;
            tsSO.FindProperty("healthFillBar").objectReferenceValue = healthSlot.fill;
            tsSO.FindProperty("inflammationLabel").objectReferenceValue = inflammSlot.label;
            tsSO.FindProperty("inflammationValueText").objectReferenceValue = inflammSlot.value;
            tsSO.FindProperty("inflammationFillBar").objectReferenceValue = inflammSlot.fill;
            tsSO.ApplyModifiedProperties();


            // ═══ TOP-RIGHT: Active Event Panel ═══
            GameObject topRightObj = CreateSubPanel(panel.transform, "ActiveEventPanel",
                new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(-140, -50), new Vector2(260, 75),
                COL_PANEL_BG);
            AddPanelBorder(topRightObj, COL_PANEL_BORDER);

            ActiveEventUI activeEventUI = GetOrAddComponent<ActiveEventUI>(topRightObj);

            GameObject aeHeaderObj = CreateText(topRightObj.transform, "HeaderText", "EVENT AKTIF", 11f, COL_SUBHEADER, new Vector2(0, 22));
            TextMeshProUGUI aeHeaderTMP = aeHeaderObj.GetComponent<TextMeshProUGUI>();
            aeHeaderTMP.fontStyle = FontStyles.Bold;

            GameObject aeNameObj = CreateText(topRightObj.transform, "EventNameText", "—", 14f, COL_HEADER, new Vector2(0, 2));
            TextMeshProUGUI aeNameTMP = aeNameObj.GetComponent<TextMeshProUGUI>();
            aeNameTMP.fontStyle = FontStyles.Bold;

            GameObject aeEffectObj = CreateText(topRightObj.transform, "EventEffectText", "", 11f, COL_SUBHEADER, new Vector2(0, -16));

            SerializedObject aeSO = new SerializedObject(activeEventUI);
            aeSO.FindProperty("headerText").objectReferenceValue = aeHeaderTMP;
            aeSO.FindProperty("eventNameText").objectReferenceValue = aeNameTMP;
            aeSO.FindProperty("eventEffectText").objectReferenceValue = aeEffectObj.GetComponent<TextMeshProUGUI>();
            aeSO.ApplyModifiedProperties();


            // ═══ LEFT PANEL: Body Components (KOMPONEN TUBUH) ═══
            GameObject bodyStatsObj = CreateSubPanel(panel.transform, "BodyStatsPanel",
                new Vector2(0f, 0.5f), new Vector2(0f, 0.5f),
                new Vector2(115, 50), new Vector2(210, 360),
                COL_PANEL_BG);
            AddPanelBorder(bodyStatsObj, COL_PANEL_BORDER);

            // Title
            GameObject bsTitleObj = CreateText(bodyStatsObj.transform, "Title", "KOMPONEN TUBUH", 13f, COL_HEADER, Vector2.zero);
            TextMeshProUGUI bsTitleTMP = bsTitleObj.GetComponent<TextMeshProUGUI>();
            bsTitleTMP.fontStyle = FontStyles.Bold;
            RectTransform bsTitleRect = bsTitleObj.GetComponent<RectTransform>();
            bsTitleRect.anchorMin = new Vector2(0f, 1f);
            bsTitleRect.anchorMax = new Vector2(1f, 1f);
            bsTitleRect.anchoredPosition = new Vector2(0, -16);
            bsTitleRect.sizeDelta = new Vector2(-16, 24);

            // Container
            GameObject bsContainer = CreateUIElement(bodyStatsObj.transform, "Container");
            RectTransform bscRect = bsContainer.GetComponent<RectTransform>();
            bscRect.anchorMin = new Vector2(0.04f, 0.03f);
            bscRect.anchorMax = new Vector2(0.96f, 0.90f);
            bscRect.anchoredPosition = Vector2.zero;
            bscRect.sizeDelta = Vector2.zero;

            VerticalLayoutGroup bsVLG = GetOrAddComponent<VerticalLayoutGroup>(bsContainer);
            bsVLG.childForceExpandHeight = false;
            bsVLG.childForceExpandWidth = true;
            bsVLG.childControlHeight = false;
            bsVLG.childControlWidth = true;
            bsVLG.spacing = 3f;
            bsVLG.padding = new RectOffset(4, 4, 2, 2);

            BodyStatsPanel bodyStatsPanel = GetOrAddComponent<BodyStatsPanel>(bodyStatsObj);
            SerializedObject bsSO = new SerializedObject(bodyStatsPanel);
            bsSO.FindProperty("statBarPrefab").objectReferenceValue = statBarPrefab;
            bsSO.FindProperty("statsContainer").objectReferenceValue = bsContainer.transform;
            bsSO.FindProperty("showOnlyImportant").boolValue = false;
            
            // Update important stats to match reference: Energy, GulaDarah, TekananDarah, SistemImun, Peradangan, Tidur, Stress, Toksin, InsulinEff
            SerializedProperty importantProp = bsSO.FindProperty("importantStats");
            importantProp.ClearArray();
            string[] referenceStats = { "Energy", "BloodSugar", "BloodPressure", "ImmuneStrength", "Inflammation", "SleepQuality", "Stress", "Toxicity", "InsulinEfficiency" };
            for (int i = 0; i < referenceStats.Length; i++)
            {
                importantProp.InsertArrayElementAtIndex(i);
                importantProp.GetArrayElementAtIndex(i).stringValue = referenceStats[i];
            }
            bsSO.ApplyModifiedProperties();


            // ═══ LEFT-BOTTOM: Disease Progress (PROGRESS PENYAKIT) ═══
            GameObject diseasePanelObj = CreateSubPanel(panel.transform, "DiseaseProgressPanel",
                new Vector2(0f, 0f), new Vector2(0f, 0f),
                new Vector2(115, 130), new Vector2(210, 155),
                COL_PANEL_BG);
            AddPanelBorder(diseasePanelObj, COL_PANEL_BORDER);

            // Title
            GameObject dpTitleObj = CreateText(diseasePanelObj.transform, "Title", "PROGRESS PENYAKIT", 12f, COL_SUBHEADER, Vector2.zero);
            TextMeshProUGUI dpTitleTMP = dpTitleObj.GetComponent<TextMeshProUGUI>();
            dpTitleTMP.fontStyle = FontStyles.Bold;
            RectTransform dpTitleRect = dpTitleObj.GetComponent<RectTransform>();
            dpTitleRect.anchorMin = new Vector2(0f, 1f);
            dpTitleRect.anchorMax = new Vector2(1f, 1f);
            dpTitleRect.anchoredPosition = new Vector2(0, -14);
            dpTitleRect.sizeDelta = new Vector2(-12, 20);

            // Container
            GameObject dpContainer = CreateUIElement(diseasePanelObj.transform, "Container");
            RectTransform dpcRect = dpContainer.GetComponent<RectTransform>();
            dpcRect.anchorMin = new Vector2(0.04f, 0.04f);
            dpcRect.anchorMax = new Vector2(0.96f, 0.82f);
            dpcRect.anchoredPosition = Vector2.zero;
            dpcRect.sizeDelta = Vector2.zero;

            VerticalLayoutGroup dpVLG = GetOrAddComponent<VerticalLayoutGroup>(dpContainer);
            dpVLG.childForceExpandHeight = false;
            dpVLG.childForceExpandWidth = true;
            dpVLG.childControlHeight = false;
            dpVLG.childControlWidth = true;
            dpVLG.spacing = 4f;
            dpVLG.padding = new RectOffset(4, 4, 2, 2);

            DiseaseProgressPanel diseaseProgressPanel = GetOrAddComponent<DiseaseProgressPanel>(diseasePanelObj);
            SerializedObject dpSO = new SerializedObject(diseaseProgressPanel);
            dpSO.FindProperty("diseaseProgressPrefab").objectReferenceValue = diseaseProgressPrefab;
            dpSO.FindProperty("container").objectReferenceValue = dpContainer.transform;
            dpSO.ApplyModifiedProperties();

            return panel;
        }

        // ════════════════════════════════════════════════════════════════════
        //  CARD SELECTION PANEL (Overlay on Battle HUD)
        // ════════════════════════════════════════════════════════════════════

        private static GameObject SetupCardSelectionPanel(Transform parent, GameObject cardUiPrefab)
        {
            GameObject panel = GetOrCreatePanel(parent, "CardSelectionPanel");
            // Semi-transparent dark overlay at bottom
            StylePanelBackground(panel, COL_TRANSPARENT);

            CardSelectionUI cardUI = GetOrAddComponent<CardSelectionUI>(panel);

            // Bottom overlay area
            GameObject overlayBg = CreateSubPanel(panel.transform, "CardOverlayBg",
                new Vector2(0.15f, 0f), new Vector2(0.85f, 0f),
                new Vector2(0, 140), new Vector2(0, 250),
                new Color(0.06f, 0.03f, 0.05f, 0.94f));
            RectTransform overlayRect = overlayBg.GetComponent<RectTransform>();
            overlayRect.sizeDelta = new Vector2(0, 250);
            // Override anchors to stretch width
            overlayRect.anchorMin = new Vector2(0.12f, 0f);
            overlayRect.anchorMax = new Vector2(0.65f, 0f);
            overlayRect.anchoredPosition = new Vector2(0, 140);
            AddPanelBorder(overlayBg, new Color(0.6f, 0.3f, 0.1f, 0.5f));

            // Title
            GameObject titleObj = CreateText(overlayBg.transform, "TitleText", "PILIH 1 KARTU TINDAKAN", 18f, COL_HEADER, Vector2.zero);
            TextMeshProUGUI titleTMP = titleObj.GetComponent<TextMeshProUGUI>();
            titleTMP.fontStyle = FontStyles.Bold;
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.anchoredPosition = new Vector2(0, -18);
            titleRect.sizeDelta = new Vector2(0, 30);

            // Cards container
            GameObject cardsContainer = CreateUIElement(overlayBg.transform, "CardsContainer");
            RectTransform cRect = cardsContainer.GetComponent<RectTransform>();
            cRect.anchorMin = new Vector2(0.03f, 0.05f);
            cRect.anchorMax = new Vector2(0.97f, 0.82f);
            cRect.anchoredPosition = Vector2.zero;
            cRect.sizeDelta = Vector2.zero;

            HorizontalLayoutGroup hlg = GetOrAddComponent<HorizontalLayoutGroup>(cardsContainer);
            hlg.spacing = 16f;
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.childControlHeight = true;
            hlg.childControlWidth = true;
            hlg.childForceExpandHeight = true;
            hlg.childForceExpandWidth = true;
            hlg.padding = new RectOffset(8, 8, 4, 4);

            SerializedObject cardSO = new SerializedObject(cardUI);
            cardSO.FindProperty("cardUIPrefab").objectReferenceValue = cardUiPrefab;
            cardSO.FindProperty("cardsContainer").objectReferenceValue = cardsContainer.transform;
            cardSO.ApplyModifiedProperties();

            return panel;
        }

        // ════════════════════════════════════════════════════════════════════
        //  RANDOM EVENT PANEL
        // ════════════════════════════════════════════════════════════════════

        private static GameObject SetupRandomEventPanel(Transform parent)
        {
            GameObject panel = GetOrCreatePanel(parent, "RandomEventPanel");
            StylePanelBackground(panel, new Color(0.04f, 0.02f, 0.03f, 0.97f));

            RandomEventUI eventUI = GetOrAddComponent<RandomEventUI>(panel);

            // Center card-like container
            GameObject cardContainer = CreateSubPanel(panel.transform, "EventCard",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0, 20), new Vector2(600, 420),
                new Color(0.10f, 0.05f, 0.07f, 0.95f));
            AddPanelBorder(cardContainer, COL_PANEL_BORDER);

            // Title
            GameObject titleObj = CreateText(cardContainer.transform, "TitleText", "Ringkasan Hari", 32f, COL_HEADER, new Vector2(0, 160));
            TextMeshProUGUI titleTMP = titleObj.GetComponent<TextMeshProUGUI>();
            titleTMP.fontStyle = FontStyles.Bold;

            // Decorative line
            GameObject lineObj = CreateUIElement(cardContainer.transform, "DecorLine");
            RectTransform lineRect = lineObj.GetComponent<RectTransform>();
            lineRect.anchoredPosition = new Vector2(0, 120);
            lineRect.sizeDelta = new Vector2(400, 2);
            Image lineImg = GetOrAddComponent<Image>(lineObj);
            lineImg.color = COL_PANEL_BORDER;

            // Description
            GameObject descObj = CreateText(cardContainer.transform, "DescriptionText", 
                "Peristiwa simulasi terjadi di dalam tubuh.", 20f, COL_VALUE, new Vector2(0, 0));
            RectTransform descRect = descObj.GetComponent<RectTransform>();
            descRect.sizeDelta = new Vector2(500, 180);
            TextMeshProUGUI descTMP = descObj.GetComponent<TextMeshProUGUI>();
            descTMP.alignment = TextAlignmentOptions.Center;

            // Continue Button
            GameObject continueBtnObj = CreateStyledButton(cardContainer.transform, "ContinueButton", 
                "LANJUTKAN  ▶", new Vector2(0, -160), new Vector2(300, 55), new Color(0.15f, 0.5f, 0.25f, 1f));

            SerializedObject eventSO = new SerializedObject(eventUI);
            eventSO.FindProperty("titleText").objectReferenceValue = titleTMP;
            eventSO.FindProperty("eventDescriptionText").objectReferenceValue = descTMP;
            eventSO.FindProperty("continueButton").objectReferenceValue = continueBtnObj.GetComponent<Button>();
            eventSO.ApplyModifiedProperties();

            return panel;
        }

        // ════════════════════════════════════════════════════════════════════
        //  GAME OVER PANEL
        // ════════════════════════════════════════════════════════════════════

        private static GameObject SetupGameOverPanel(Transform parent)
        {
            GameObject panel = GetOrCreatePanel(parent, "GameOverPanel");
            StylePanelBackground(panel, new Color(0.12f, 0.02f, 0.02f, 0.98f));

            GameOverUI gameOverUI = GetOrAddComponent<GameOverUI>(panel);

            // Center container
            GameObject container = CreateSubPanel(panel.transform, "Container",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0, 0), new Vector2(600, 400),
                new Color(0.08f, 0.02f, 0.02f, 0.95f));
            AddPanelBorder(container, new Color(0.8f, 0.1f, 0.1f, 0.6f));

            // Death icon
            CreateText(container.transform, "DeathIcon", "☠", 80f, COL_RED, new Vector2(0, 130));

            GameObject titleObj = CreateText(container.transform, "TitleText", "KEGAGALAN ORGAN", 42f, COL_RED, new Vector2(0, 50));
            TextMeshProUGUI titleTMP = titleObj.GetComponent<TextMeshProUGUI>();
            titleTMP.fontStyle = FontStyles.Bold;

            GameObject descObj = CreateText(container.transform, "DaysText", "Kamu bertahan 0 hari.", 22f, COL_VALUE, new Vector2(0, -10));

            // Decorative line
            GameObject lineObj = CreateUIElement(container.transform, "DecorLine");
            RectTransform lineRect = lineObj.GetComponent<RectTransform>();
            lineRect.anchoredPosition = new Vector2(0, -40);
            lineRect.sizeDelta = new Vector2(350, 2);
            Image lineImg = GetOrAddComponent<Image>(lineObj);
            lineImg.color = COL_PANEL_BORDER;

            GameObject restartBtnObj = CreateStyledButton(container.transform, "RestartButton", "⟳  COBA LAGI", 
                new Vector2(0, -80), new Vector2(300, 55), COL_BUTTON_BG);
            GameObject quitBtnObj = CreateStyledButton(container.transform, "MainMenuButton", "↩  MENU UTAMA", 
                new Vector2(0, -150), new Vector2(300, 50), COL_BUTTON_QUIT);

            SerializedObject goSO = new SerializedObject(gameOverUI);
            goSO.FindProperty("daysSurvivedText").objectReferenceValue = descObj.GetComponent<TextMeshProUGUI>();
            goSO.FindProperty("restartButton").objectReferenceValue = restartBtnObj.GetComponent<Button>();
            goSO.FindProperty("mainMenuButton").objectReferenceValue = quitBtnObj.GetComponent<Button>();
            goSO.ApplyModifiedProperties();

            return panel;
        }

        // ════════════════════════════════════════════════════════════════════
        //  VICTORY PANEL
        // ════════════════════════════════════════════════════════════════════

        private static GameObject SetupVictoryPanel(Transform parent)
        {
            GameObject panel = GetOrCreatePanel(parent, "VictoryPanel");
            StylePanelBackground(panel, new Color(0.02f, 0.08f, 0.04f, 0.98f));

            VictoryUI victoryUI = GetOrAddComponent<VictoryUI>(panel);

            // Center container
            GameObject container = CreateSubPanel(panel.transform, "Container",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0, 0), new Vector2(600, 400),
                new Color(0.03f, 0.08f, 0.04f, 0.95f));
            AddPanelBorder(container, new Color(0.2f, 0.8f, 0.3f, 0.5f));

            // Victory icon
            CreateText(container.transform, "VictoryIcon", "🏆", 80f, new Color(1f, 0.85f, 0.2f), new Vector2(0, 130));

            GameObject titleObj = CreateText(container.transform, "TitleText", "100 HARI BERTAHAN!", 42f, COL_GREEN, new Vector2(0, 50));
            TextMeshProUGUI titleTMP = titleObj.GetComponent<TextMeshProUGUI>();
            titleTMP.fontStyle = FontStyles.Bold;

            CreateText(container.transform, "DescText", 
                "Mekanisme pertahanan tubuh berhasil menstabilkan\nkondisi selama 100 hari.", 
                20f, COL_VALUE, new Vector2(0, -10));

            // Decorative line
            GameObject lineObj = CreateUIElement(container.transform, "DecorLine");
            RectTransform lineRect = lineObj.GetComponent<RectTransform>();
            lineRect.anchoredPosition = new Vector2(0, -50);
            lineRect.sizeDelta = new Vector2(350, 2);
            Image lineImg = GetOrAddComponent<Image>(lineObj);
            lineImg.color = new Color(0.2f, 0.7f, 0.3f, 0.5f);

            GameObject restartBtnObj = CreateStyledButton(container.transform, "RestartButton", "⟳  MAIN LAGI", 
                new Vector2(0, -90), new Vector2(300, 55), new Color(0.1f, 0.5f, 0.2f, 1f));
            GameObject quitBtnObj = CreateStyledButton(container.transform, "MainMenuButton", "↩  MENU UTAMA", 
                new Vector2(0, -160), new Vector2(300, 50), COL_BUTTON_QUIT);

            SerializedObject vicSO = new SerializedObject(victoryUI);
            vicSO.FindProperty("restartButton").objectReferenceValue = restartBtnObj.GetComponent<Button>();
            vicSO.FindProperty("mainMenuButton").objectReferenceValue = quitBtnObj.GetComponent<Button>();
            vicSO.ApplyModifiedProperties();

            return panel;
        }

        // ════════════════════════════════════════════════════════════════════
        //  PAUSE PANEL
        // ════════════════════════════════════════════════════════════════════

        private static GameObject SetupPausePanel(Transform parent)
        {
            GameObject panel = GetOrCreatePanel(parent, "PausePanel");
            StylePanelBackground(panel, COL_OVERLAY);

            PauseMenuUI pauseUI = GetOrAddComponent<PauseMenuUI>(panel);

            // Center container
            GameObject container = CreateSubPanel(panel.transform, "Container",
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0, 0), new Vector2(450, 320),
                COL_PANEL_BG);
            AddPanelBorder(container, COL_PANEL_BORDER);

            CreateText(container.transform, "TitleText", "GAME DIJEDA", 38f, COL_HEADER, new Vector2(0, 100));

            // Decorative line
            GameObject lineObj = CreateUIElement(container.transform, "DecorLine");
            RectTransform lineRect = lineObj.GetComponent<RectTransform>();
            lineRect.anchoredPosition = new Vector2(0, 60);
            lineRect.sizeDelta = new Vector2(300, 2);
            Image lineImg = GetOrAddComponent<Image>(lineObj);
            lineImg.color = COL_PANEL_BORDER;

            GameObject resumeBtnObj = CreateStyledButton(container.transform, "ResumeButton", "▶  LANJUTKAN", 
                new Vector2(0, 10), new Vector2(300, 55), new Color(0.15f, 0.45f, 0.25f, 1f));
            GameObject quitBtnObj = CreateStyledButton(container.transform, "QuitButton", "✕  KELUAR KE MENU", 
                new Vector2(0, -60), new Vector2(300, 50), COL_BUTTON_BG);

            SerializedObject pauseSO = new SerializedObject(pauseUI);
            pauseSO.FindProperty("resumeButton").objectReferenceValue = resumeBtnObj.GetComponent<Button>();
            pauseSO.FindProperty("quitButton").objectReferenceValue = quitBtnObj.GetComponent<Button>();
            pauseSO.ApplyModifiedProperties();

            return panel;
        }


        // ════════════════════════════════════════════════════════════════════
        //  HELPER: Top Status Bar Slot
        // ════════════════════════════════════════════════════════════════════

        private struct TopStatSlotRefs
        {
            public TextMeshProUGUI label;
            public TextMeshProUGUI value;
            public Image fill;
        }

        private static TopStatSlotRefs CreateTopStatSlot(Transform parent, string name, string icon, string labelText, string valueText, Color barColor)
        {
            GameObject slot = CreateUIElement(parent, name);
            // Let horizontal layout group handle sizing
            LayoutElement le = GetOrAddComponent<LayoutElement>(slot);
            le.flexibleWidth = 1f;
            le.minHeight = 45f;

            // Background for this slot
            Image slotBg = GetOrAddComponent<Image>(slot);
            slotBg.color = new Color(0.06f, 0.03f, 0.05f, 0.7f);

            // Icon
            GameObject iconObj = CreateText(slot.transform, "Icon", icon, 18f, COL_VALUE, Vector2.zero);
            RectTransform iconRect = iconObj.GetComponent<RectTransform>();
            iconRect.anchorMin = new Vector2(0f, 0f);
            iconRect.anchorMax = new Vector2(0.15f, 1f);
            iconRect.anchoredPosition = Vector2.zero;
            iconRect.sizeDelta = Vector2.zero;

            // Label
            GameObject labelObj = CreateText(slot.transform, "Label", labelText, 10f, COL_SUBHEADER, Vector2.zero);
            TextMeshProUGUI labelTMP = labelObj.GetComponent<TextMeshProUGUI>();
            labelTMP.fontStyle = FontStyles.Bold;
            labelTMP.alignment = TextAlignmentOptions.Left;
            RectTransform labelRect = labelObj.GetComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0.15f, 0.55f);
            labelRect.anchorMax = new Vector2(0.75f, 1f);
            labelRect.anchoredPosition = Vector2.zero;
            labelRect.sizeDelta = Vector2.zero;

            // Value text (percentage)
            GameObject valueObj = CreateText(slot.transform, "Value", valueText, 13f, COL_VALUE, Vector2.zero);
            TextMeshProUGUI valueTMP = valueObj.GetComponent<TextMeshProUGUI>();
            valueTMP.fontStyle = FontStyles.Bold;
            valueTMP.alignment = TextAlignmentOptions.Right;
            RectTransform valueRect = valueObj.GetComponent<RectTransform>();
            valueRect.anchorMin = new Vector2(0.75f, 0.55f);
            valueRect.anchorMax = new Vector2(0.98f, 1f);
            valueRect.anchoredPosition = Vector2.zero;
            valueRect.sizeDelta = Vector2.zero;

            // Bar background
            GameObject barBg = CreateUIElement(slot.transform, "BarBg");
            RectTransform barBgRect = barBg.GetComponent<RectTransform>();
            barBgRect.anchorMin = new Vector2(0.15f, 0.15f);
            barBgRect.anchorMax = new Vector2(0.98f, 0.45f);
            barBgRect.anchoredPosition = Vector2.zero;
            barBgRect.sizeDelta = Vector2.zero;
            Image barBgImg = GetOrAddComponent<Image>(barBg);
            barBgImg.color = COL_BAR_BG;

            // Bar fill
            GameObject barFill = CreateUIElement(barBg.transform, "Fill");
            RectTransform barFillRect = barFill.GetComponent<RectTransform>();
            barFillRect.anchorMin = Vector2.zero;
            barFillRect.anchorMax = Vector2.one;
            barFillRect.anchoredPosition = Vector2.zero;
            barFillRect.sizeDelta = Vector2.zero;
            Image barFillImg = GetOrAddComponent<Image>(barFill);
            barFillImg.color = barColor;
            barFillImg.type = Image.Type.Filled;
            barFillImg.fillMethod = Image.FillMethod.Horizontal;
            barFillImg.fillAmount = 0.7f;

            return new TopStatSlotRefs
            {
                label = labelTMP,
                value = valueTMP,
                fill = barFillImg
            };
        }


        // ════════════════════════════════════════════════════════════════════
        //  HELPER METHODS
        // ════════════════════════════════════════════════════════════════════

        private static GameObject GetOrCreatePanel(Transform parent, string name)
        {
            Transform t = parent.Find(name);
            if (t != null)
            {
                foreach (Transform child in t)
                {
                    DestroyImmediate(child.gameObject);
                }
                return t.gameObject;
            }

            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;

            Undo.RegisterCreatedObjectUndo(obj, "Create Panel " + name);
            return obj;
        }

        private static GameObject CreateSubPanel(Transform parent, string name, 
            Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size, Color bgColor)
        {
            GameObject obj = CreateUIElement(parent, name);
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            Image img = GetOrAddComponent<Image>(obj);
            img.color = bgColor;

            return obj;
        }

        private static void AddPanelBorder(GameObject panel, Color borderColor)
        {
            Outline outline = GetOrAddComponent<Outline>(panel);
            outline.effectColor = borderColor;
            outline.effectDistance = new Vector2(1.5f, -1.5f);
        }

        private static void StylePanelBackground(GameObject panel, Color color)
        {
            Image img = GetOrAddComponent<Image>(panel);
            img.color = color;
        }

        private static GameObject CreateUIElement(Transform parent, string name)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(100, 30);
            return obj;
        }

        private static GameObject CreateText(Transform parent, string name, string text, float fontSize, Color color, Vector2 pos)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(600, 80);

            TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.color = color;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = false;
            tmp.overflowMode = TextOverflowModes.Ellipsis;

            return obj;
        }

        private static GameObject CreateStyledButton(Transform parent, string name, string label, Vector2 pos, Vector2 size, Color normalColor)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);
            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchoredPosition = pos;
            rect.sizeDelta = size;

            Image img = obj.AddComponent<Image>();
            img.color = normalColor;

            Button btn = obj.AddComponent<Button>();

            // Button color transition
            ColorBlock colors = btn.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.2f, 1.1f, 1.1f, 1f);
            colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            colors.selectedColor = Color.white;
            colors.fadeDuration = 0.1f;
            btn.colors = colors;

            // Add outline for a subtle border effect
            Outline outline = obj.AddComponent<Outline>();
            outline.effectColor = new Color(1f, 1f, 1f, 0.08f);
            outline.effectDistance = new Vector2(1f, -1f);

            // Text child
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(obj.transform, false);
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;

            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.fontSize = 18f;
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontStyle = FontStyles.Bold;
            tmp.raycastTarget = false;

            return obj;
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
