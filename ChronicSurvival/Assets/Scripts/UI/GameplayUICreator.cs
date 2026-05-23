using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ChronicSurvival.Core;

namespace ChronicSurvival.UI
{
    /// <summary>
    /// Builds the full gameplay UI at runtime (pattern from NeonShooter UISetup / StartScreenManager.CreateUI).
    /// No editor menu required — Play Mode creates panels automatically.
    /// </summary>
    public static class GameplayUICreator
    {
        static readonly Color COL_BG = new Color(0.035f, 0.045f, 0.065f, 0.98f);
        static readonly Color COL_PANEL = new Color(0.085f, 0.105f, 0.135f, 0.82f);
        static readonly Color COL_PANEL_DARK = new Color(0.035f, 0.045f, 0.065f, 0.92f);
        static readonly Color COL_BORDER = new Color(0.22f, 0.62f, 0.95f, 0.45f);
        static readonly Color COL_HEADER = new Color(0.94f, 0.97f, 1f, 1f);
        static readonly Color COL_SUB = new Color(0.55f, 0.62f, 0.70f, 1f);
        static readonly Color COL_DIM = new Color(0.005f, 0.008f, 0.012f, 0.84f);
        static readonly Color COL_BTN = new Color(0.08f, 0.22f, 0.34f, 0.95f);
        static readonly Color COL_GREEN = new Color(0.22f, 0.83f, 0.54f, 1f);
        static readonly Color COL_TEAL = new Color(0.23f, 0.72f, 1f, 1f);
        static readonly Color COL_WARN = new Color(0.94f, 0.53f, 0.22f, 1f);
        static readonly Color COL_CRIT = new Color(1f, 0.34f, 0.30f, 1f);
        static readonly Color COL_TRACK = new Color(0.07f, 0.085f, 0.11f, 0.95f);

        static GameObject cardTemplate;
        static GameObject chipTemplate;
        static GameObject statBarTemplate;
        static GameObject diseaseTemplate;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AutoBuildIfMissing()
        {
            Canvas canvas = Object.FindFirstObjectByType<Canvas>(FindObjectsInactive.Include);
            if (canvas == null)
            {
                canvas = EnsureCanvas();
            }

            Transform battle = canvas.transform.Find("BattleHUDPanel");
            bool hasV2Hud = canvas.transform.Find("MainMenuPanel") != null
                && battle != null
                && battle.Find("RoundPanel") != null;

            if (!hasV2Hud)
            {
                BuildAll(canvas.transform);
                Debug.Log("[GameplayUICreator] AutoBuild: UI v2 selesai dibangun.");
            }

            if (canvas.GetComponent<GameplayUIInstaller>() == null)
            {
                canvas.gameObject.AddComponent<GameplayUIInstaller>();
            }

            canvas.gameObject.SetActive(true);
            UIManager ui = canvas.GetComponent<UIManager>();
            if (ui == null) ui = canvas.gameObject.AddComponent<UIManager>();
            ui.BindPanels(
                FindPanel(canvas.transform, "MainMenuPanel"),
                FindPanel(canvas.transform, "BattleHUDPanel"),
                FindPanel(canvas.transform, "CardSelectionPanel"),
                FindPanel(canvas.transform, "RandomEventPanel"),
                FindPanel(canvas.transform, "GameOverPanel"),
                FindPanel(canvas.transform, "VictoryPanel"),
                FindPanel(canvas.transform, "PausePanel"));
            ui.ConnectToGameManager();
            ui.RefreshForCurrentState();
        }

        public static Canvas EnsureCanvas()
        {
            Canvas canvas = Object.FindFirstObjectByType<Canvas>(FindObjectsInactive.Include);
            if (canvas == null)
            {
                GameObject go = new GameObject("Canvas");
                canvas = go.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 100;

                CanvasScaler scaler = go.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;

                go.AddComponent<GraphicRaycaster>();
                go.AddComponent<GameplayUIInstaller>();
            }

            FixCanvasLayout(canvas);
            EnsureEventSystem();
            return canvas;
        }

        /// <summary>
        /// SampleScene Canvas had zero-size rect (anchors collapsed) — UI rendered invisible.
        /// </summary>
        public static void FixCanvasLayout(Canvas canvas)
        {
            if (canvas == null) return;

            RectTransform rt = canvas.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.localScale = Vector3.one;
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = Vector2.zero;
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }

            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
            }

            // Required for TextMeshPro on uGUI
            canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1
                | AdditionalCanvasShaderChannels.Normal
                | AdditionalCanvasShaderChannels.Tangent;
        }

        public static void BuildAll(Transform canvasRoot)
        {
            if (canvasRoot == null) return;

            EnsureEventSystem();
            EnsureTemplates();

            DestroyPanel(canvasRoot, "MainMenuPanel");
            DestroyPanel(canvasRoot, "BattleHUDPanel");
            DestroyPanel(canvasRoot, "CardSelectionPanel");
            DestroyPanel(canvasRoot, "RandomEventPanel");
            DestroyPanel(canvasRoot, "GameOverPanel");
            DestroyPanel(canvasRoot, "VictoryPanel");
            DestroyPanel(canvasRoot, "PausePanel");
            DestroyPanel(canvasRoot, "BodyStatsExpanded");

            GameObject mainMenu = BuildMainMenu(canvasRoot);
            GameObject battleHud = BuildBattleHUD(canvasRoot);
            GameObject cardPanel = BuildCardSelection(canvasRoot);
            BuildRandomEventPanel(canvasRoot);
            BuildGameOverPanel(canvasRoot);
            BuildVictoryPanel(canvasRoot);
            BuildPausePanel(canvasRoot);

            Canvas canvas = canvasRoot.GetComponent<Canvas>();
            if (canvas != null) FixCanvasLayout(canvas);

            UIManager existingUi = Object.FindFirstObjectByType<UIManager>(FindObjectsInactive.Include);
            if (existingUi != null && existingUi.transform != canvasRoot && existingUi.GetComponent<Canvas>() == null)
            {
                Object.Destroy(existingUi);
            }

            UIManager ui = canvasRoot.GetComponent<UIManager>();
            if (ui == null) ui = canvasRoot.gameObject.AddComponent<UIManager>();

            if (canvasRoot.GetComponent<GameplayUIInstaller>() == null)
            {
                var inst = canvasRoot.gameObject.AddComponent<GameplayUIInstaller>();
                UiFieldBinder.Set(inst, "autoBuildIfMissing", false);
            }

            ui.BindPanels(
                mainMenu,
                battleHud,
                cardPanel,
                FindPanel(canvasRoot, "RandomEventPanel"),
                FindPanel(canvasRoot, "GameOverPanel"),
                FindPanel(canvasRoot, "VictoryPanel"),
                FindPanel(canvasRoot, "PausePanel"));

            mainMenu.SetActive(false);
            battleHud.SetActive(false);
            cardPanel.SetActive(false);
            FindPanel(canvasRoot, "RandomEventPanel")?.SetActive(false);
            FindPanel(canvasRoot, "GameOverPanel")?.SetActive(false);
            FindPanel(canvasRoot, "VictoryPanel")?.SetActive(false);
            FindPanel(canvasRoot, "PausePanel")?.SetActive(false);

            ui.ConnectToGameManager();
            ui.RefreshForCurrentState();
        }

        static void EnsureTemplates()
        {
            if (cardTemplate == null) cardTemplate = BuildCardTemplate();
            if (chipTemplate == null) chipTemplate = BuildChipTemplate();
            if (statBarTemplate == null) statBarTemplate = BuildStatBarTemplate();
            if (diseaseTemplate == null) diseaseTemplate = BuildDiseaseTemplate();
        }

        static void EnsureEventSystem()
        {
            if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>(FindObjectsInactive.Include) != null)
            {
                return;
            }

            GameObject es = new GameObject("EventSystem");
            es.AddComponent<UnityEngine.EventSystems.EventSystem>();
            es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        static GameObject FindPanel(Transform parent, string name)
        {
            Transform t = parent.Find(name);
            return t != null ? t.gameObject : null;
        }

        static void DestroyPanel(Transform parent, string name)
        {
            Transform t = parent.Find(name);
            if (t == null) return;

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Object.DestroyImmediate(t.gameObject);
                return;
            }
#endif
            Object.Destroy(t.gameObject);
        }

        // ─── MAIN MENU ───
        static GameObject BuildMainMenu(Transform parent)
        {
            GameObject panel = CreatePanel(parent, "MainMenuPanel");
            Stretch(panel);
            SetImage(panel, COL_BG);

            CanvasGroup cg = panel.AddComponent<CanvasGroup>();
            MainMenuUI menu = panel.AddComponent<MainMenuUI>();

            CreateStretchImage(panel.transform, "BioMeshWash", new Color(0.02f, 0.14f, 0.18f, 0.28f));
            GameObject scan = CreatePanel(panel.transform, "ScanLine");
            SetAnchored(scan, 0.5f, 0.5f, new Vector2(0, 22), new Vector2(1180, 2));
            SetImage(scan, new Color(COL_TEAL.r, COL_TEAL.g, COL_TEAL.b, 0.45f));

            GameObject frame = CreateGlassPanel(panel.transform, "CommandFrame", new Vector2(-330, -20), new Vector2(560, 520), 0.5f, 0.5f);
            CreateTelemetryLabel(frame.transform, "Section", "SYSTEM BOOT", new Vector2(-210, 220), new Vector2(220, 24), 12);
            CreateDivider(frame.transform, "Divider", new Vector2(0, 192), new Vector2(500, 2));

            TextMeshProUGUI title = CreateTMP(frame.transform, "TitleText", "CHRONIC\nSURVIVAL", 54, COL_HEADER, FontStyles.Bold);
            SetAnchored(title.gameObject, 0.5f, 0.5f, new Vector2(-5, 105), new Vector2(500, 130));
            title.alignment = TextAlignmentOptions.Left;
            title.characterSpacing = 8;

            TextMeshProUGUI subtitle = CreateTMP(frame.transform, "SubtitleText",
                "TACTICAL CELLULAR DEFENSE SIMULATION\nStabilize body systems. Survive 100 days.", 15, COL_SUB);
            SetAnchored(subtitle.gameObject, 0.5f, 0.5f, new Vector2(0, 22), new Vector2(500, 54));
            subtitle.alignment = TextAlignmentOptions.Left;

            Button startBtn = CreateButton(frame.transform, "StartButton", "[ 01 // START SYSTEM ]", new Vector2(0, -58), new Vector2(500, 58), COL_BTN);
            CreateButton(frame.transform, "ArchivesButton", "[ 02 // ACCESS ARCHIVES ]", new Vector2(0, -128), new Vector2(500, 48), new Color(0.055f, 0.075f, 0.10f, 0.86f)).interactable = false;
            CreateButton(frame.transform, "CalibrationButton", "[ 03 // CALIBRATION ]", new Vector2(0, -188), new Vector2(500, 48), new Color(0.055f, 0.075f, 0.10f, 0.86f)).interactable = false;
            Button quitBtn = CreateButton(frame.transform, "QuitButton", "[ 04 // TERMINATE ]", new Vector2(0, -248), new Vector2(500, 48), new Color(0.18f, 0.08f, 0.09f, 0.95f));

            GameObject status = CreateGlassPanel(panel.transform, "StatusReadout", new Vector2(360, -185), new Vector2(420, 210), 0.5f, 0.5f);
            CreateTelemetryLabel(status.transform, "StatusTitle", "MISSION PARAMETERS", new Vector2(-150, 78), new Vector2(260, 22), 12);
            TextMeshProUGUI readout = CreateTMP(status.transform, "Readout", "TARGET: 100 DAYS\nTHREATS: DIABETES / HYPERTENSION / CANCER\nINTERFACE: BIO-TELEMETRY ONLINE", 14, COL_SUB);
            SetAnchored(readout.gameObject, 0.5f, 0.5f, new Vector2(0, 5), new Vector2(360, 100));
            readout.alignment = TextAlignmentOptions.Left;
            readout.lineSpacing = 16;

            CreateTMP(panel.transform, "VersionText", "v0.2 // UI TELEMETRY REDESIGN", 12, new Color(COL_SUB.r, COL_SUB.g, COL_SUB.b, 0.75f))
                .rectTransform.anchoredPosition = new Vector2(0, -380);

            UiFieldBinder.Set(menu, "startButton", startBtn);
            UiFieldBinder.Set(menu, "quitButton", quitBtn);
            UiFieldBinder.Set(menu, "canvasGroup", cg);
            UiFieldBinder.Set(menu, "titleText", title);
            return panel;
        }


        // ─── BATTLE HUD ───
        static GameObject BuildBattleHUD(Transform parent)
        {
            GameObject panel = CreatePanel(parent, "BattleHUDPanel");
            Stretch(panel);
            SetImage(panel, Color.clear);

            CanvasGroup hudCg = panel.AddComponent<CanvasGroup>();
            GameplayHUDController hudCtrl = panel.AddComponent<GameplayHUDController>();
            UiFieldBinder.Set(hudCtrl, "hudCanvasGroup", hudCg);

            GameObject topCenter = CreateGlassPanel(panel.transform, "RoundPanel", new Vector2(0, -54), new Vector2(380, 96), 0.5f, 1f);
            DayCounterUI dayUI = topCenter.AddComponent<DayCounterUI>();
            CreateTelemetryLabel(topCenter.transform, "PhaseLabel", "ROUND TIME REMAINING", new Vector2(-110, 30), new Vector2(220, 20), 10);
            TextMeshProUGUI timerTxt = CreateTMP(topCenter.transform, "TimerText", "00:00", 34, COL_HEADER, FontStyles.Bold);
            SetAnchored(timerTxt.gameObject, 0.5f, 0.5f, new Vector2(64, 15), new Vector2(170, 46));
            timerTxt.characterSpacing = 6;
            TextMeshProUGUI dayTxt = CreateTMP(topCenter.transform, "DayText", "RONDE 1", 14, COL_TEAL, FontStyles.Bold);
            SetAnchored(dayTxt.gameObject, 0.5f, 0.5f, new Vector2(-118, -22), new Vector2(180, 26));
            dayTxt.alignment = TextAlignmentOptions.Left;
            Button pauseBtn = CreateButton(topCenter.transform, "PauseBtn", "II", new Vector2(155, -22), new Vector2(46, 34), new Color(0.06f, 0.12f, 0.17f, 0.95f));
            UiFieldBinder.Set(dayUI, "dayText", dayTxt);
            UiFieldBinder.Set(dayUI, "timerText", timerTxt);
            UiFieldBinder.Set(dayUI, "pauseButton", pauseBtn);

            GameObject left = CreateGlassPanel(panel.transform, "BodyStatsBottom", new Vector2(150, -305), new Vector2(300, 520), 0f, 1f);
            CreateTelemetryLabel(left.transform, "Label", "PHYSIOLOGICAL OVERVIEW", new Vector2(-80, 230), new Vector2(210, 22), 11);
            CreateDivider(left.transform, "BodyDivider", new Vector2(0, 205), new Vector2(260, 2));
            Button expandBtn = CreateButton(left.transform, "ExpandBtn", "DETAIL", new Vector2(94, 230), new Vector2(82, 28), new Color(0.05f, 0.17f, 0.22f, 0.95f));
            TextMeshProUGUI expandTxt = expandBtn.GetComponentInChildren<TextMeshProUGUI>();
            GameObject chipsRow = CreatePanel(left.transform, "CompactChips");
            RectTransform chipsRect = chipsRow.GetComponent<RectTransform>();
            chipsRect.anchorMin = new Vector2(0f, 0f);
            chipsRect.anchorMax = new Vector2(1f, 1f);
            chipsRect.offsetMin = new Vector2(18, 145);
            chipsRect.offsetMax = new Vector2(-18, -60);
            VerticalLayoutGroup chipVLG = chipsRow.AddComponent<VerticalLayoutGroup>();
            chipVLG.spacing = 8;
            chipVLG.childForceExpandWidth = true;
            chipVLG.childControlHeight = true;
            chipVLG.childForceExpandHeight = false;

            GameObject diseasePanel = CreateGlassPanel(panel.transform, "DiseaseTopPanel", new Vector2(150, -655), new Vector2(300, 160), 0f, 1f);
            CreateTelemetryLabel(diseasePanel.transform, "Title", "DISEASE PROGRESSION", new Vector2(-78, 58), new Vector2(220, 20), 11);
            CreateDivider(diseasePanel.transform, "DiseaseDivider", new Vector2(0, 35), new Vector2(260, 2));
            GameObject dpContainer = CreatePanel(diseasePanel.transform, "Container");
            Stretch(dpContainer);
            dpContainer.GetComponent<RectTransform>().offsetMin = new Vector2(16, 12);
            dpContainer.GetComponent<RectTransform>().offsetMax = new Vector2(-16, -50);
            VerticalLayoutGroup dpVLG = dpContainer.AddComponent<VerticalLayoutGroup>();
            dpVLG.spacing = 6;
            dpVLG.childForceExpandWidth = true;
            dpVLG.childForceExpandHeight = false;
            DiseaseProgressPanel diseaseProg = diseasePanel.AddComponent<DiseaseProgressPanel>();
            UiFieldBinder.Set(diseaseProg, "diseaseProgressPrefab", diseaseTemplate);
            UiFieldBinder.Set(diseaseProg, "container", dpContainer.transform);

            GameObject topBar = CreateGlassPanel(panel.transform, "TopStatusBar", new Vector2(0, -148), new Vector2(560, 66), 0.5f, 1f);
            TopStatusBarUI topStatus = topBar.AddComponent<TopStatusBarUI>();
            HorizontalLayoutGroup hlgTop = topBar.AddComponent<HorizontalLayoutGroup>();
            hlgTop.spacing = 10;
            hlgTop.padding = new RectOffset(12, 12, 8, 8);
            hlgTop.childForceExpandWidth = true;
            hlgTop.childForceExpandHeight = true;
            SummarySlot immuneSlot = CreateSummarySlot(topBar.transform, "Imun", "IMMUNE", COL_TEAL);
            SummarySlot healthSlot = CreateSummarySlot(topBar.transform, "Health", "HEALTH", COL_GREEN);
            SummarySlot inflSlot = CreateSummarySlot(topBar.transform, "Inflam", "INFLAMMATION", COL_WARN);
            UiFieldBinder.Set(topStatus, "immuneValueText", immuneSlot.valueText);
            UiFieldBinder.Set(topStatus, "immuneFillBar", immuneSlot.fillImage);
            UiFieldBinder.Set(topStatus, "healthValueText", healthSlot.valueText);
            UiFieldBinder.Set(topStatus, "healthFillBar", healthSlot.fillImage);
            UiFieldBinder.Set(topStatus, "inflammationValueText", inflSlot.valueText);
            UiFieldBinder.Set(topStatus, "inflammationFillBar", inflSlot.fillImage);

            GameObject eventPanel = CreateGlassPanel(panel.transform, "ActiveEventPanel", new Vector2(-170, -238), new Vector2(340, 190), 1f, 1f);
            ActiveEventUI activeEv = eventPanel.AddComponent<ActiveEventUI>();
            TextMeshProUGUI evH = CreateTelemetryLabel(eventPanel.transform, "Header", "LIVE BIO-FEEDBACK", new Vector2(-92, 70), new Vector2(210, 20), 11);
            TextMeshProUGUI evN = CreateTMP(eventPanel.transform, "Name", "SYSTEM STABLE", 16, COL_HEADER, FontStyles.Bold);
            SetAnchored(evN.gameObject, 0.5f, 0.5f, new Vector2(0, 20), new Vector2(290, 32));
            evN.alignment = TextAlignmentOptions.Left;
            TextMeshProUGUI evE = CreateTMP(eventPanel.transform, "Effect", "No active adverse events detected.", 12, COL_SUB);
            SetAnchored(evE.gameObject, 0.5f, 0.5f, new Vector2(0, -35), new Vector2(290, 70));
            evE.alignment = TextAlignmentOptions.Left;
            UiFieldBinder.Set(activeEv, "headerText", evH);
            UiFieldBinder.Set(activeEv, "eventNameText", evN);
            UiFieldBinder.Set(activeEv, "eventEffectText", evE);

            GameObject legend = CreateGlassPanel(panel.transform, "ArenaLegend", new Vector2(-170, 120), new Vector2(340, 205), 1f, 0f);
            CreateTelemetryLabel(legend.transform, "LegendTitle", "ARENA LEGEND", new Vector2(-92, 75), new Vector2(210, 20), 11);
            TextMeshProUGUI legendText = CreateTMP(legend.transform, "LegendText", "FLOW_NORMAL     stable blood stream\nSTICKY_ZONE     high glucose friction\nINFLAMED_AREA   movement penalty\nMUTATION_FIELD  cancer risk zone\nBLOCKED_FLOW    oxygen routing threat", 12, COL_SUB);
            SetAnchored(legendText.gameObject, 0.5f, 0.5f, new Vector2(4, -18), new Vector2(290, 130));
            legendText.alignment = TextAlignmentOptions.Left;
            legendText.lineSpacing = 10;

            GameObject drawer = CreatePanel(panel.transform, "BodyStatsExpanded");
            RectTransform drawerRect = drawer.GetComponent<RectTransform>();
            drawerRect.anchorMin = new Vector2(0.5f, 0f);
            drawerRect.anchorMax = new Vector2(0.5f, 0f);
            drawerRect.pivot = new Vector2(0.5f, 0f);
            drawerRect.anchoredPosition = new Vector2(0, -340);
            drawerRect.sizeDelta = new Vector2(760, 340);
            StylePanel(drawer);
            CanvasGroup drawerCg = drawer.AddComponent<CanvasGroup>();
            CreateTelemetryLabel(drawer.transform, "Title", "BODY COMPONENT DETAIL", new Vector2(-260, 150), new Vector2(300, 22), 12);
            GameObject fullContainer = CreatePanel(drawer.transform, "FullStatsContainer");
            Stretch(fullContainer);
            fullContainer.GetComponent<RectTransform>().offsetMin = new Vector2(18, 18);
            fullContainer.GetComponent<RectTransform>().offsetMax = new Vector2(-18, -50);
            VerticalLayoutGroup vlg = fullContainer.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 8;
            vlg.childForceExpandWidth = true;
            vlg.childControlHeight = false;
            BodyStatsPanel bodyPanel = drawer.AddComponent<BodyStatsPanel>();
            UiFieldBinder.Set(bodyPanel, "statBarPrefab", statBarTemplate);
            UiFieldBinder.Set(bodyPanel, "statsContainer", fullContainer.transform);
            UiFieldBinder.Set(bodyPanel, "showOnlyImportant", false);

            BodyStatsDrawerUI drawerUI = left.AddComponent<BodyStatsDrawerUI>();
            UiFieldBinder.Set(drawerUI, "compactBarRoot", left.GetComponent<RectTransform>());
            UiFieldBinder.Set(drawerUI, "compactChipsContainer", chipsRow.transform);
            UiFieldBinder.Set(drawerUI, "compactChipPrefab", chipTemplate);
            UiFieldBinder.Set(drawerUI, "expandedDrawer", drawerRect);
            UiFieldBinder.Set(drawerUI, "expandedStatsPanel", bodyPanel);
            UiFieldBinder.Set(drawerUI, "expandedCanvasGroup", drawerCg);
            UiFieldBinder.Set(drawerUI, "expandButton", expandBtn);
            UiFieldBinder.Set(drawerUI, "expandArrowText", expandTxt);

            panel.transform.SetAsLastSibling();
            return panel;
        }


        static GameObject BuildCardSelection(Transform parent)
        {
            GameObject panel = CreatePanel(parent, "CardSelectionPanel");
            Stretch(panel);
            SetImage(panel, Color.clear);

            CanvasGroup cg = panel.AddComponent<CanvasGroup>();
            CardSelectionUI cardUI = panel.AddComponent<CardSelectionUI>();

            Image dim = CreateStretchImage(panel.transform, "FullScreenDim", COL_DIM);

            GameObject modal = CreateGlassPanel(panel.transform, "TacticalDeck", new Vector2(0, 88), new Vector2(860, 330), 0.5f, 0f);
            CreateTelemetryLabel(modal.transform, "Header", "SELECT 1 TACTICAL ACTION", new Vector2(-290, 128), new Vector2(320, 24), 13);
            TextMeshProUGUI header = modal.transform.Find("Header").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI sub = CreateTMP(modal.transform, "Subtitle", "Choose one intervention. Effects apply immediately to body telemetry.", 13, COL_SUB);
            SetAnchored(sub.gameObject, 0.5f, 0.5f, new Vector2(-118, 102), new Vector2(560, 24));
            sub.alignment = TextAlignmentOptions.Left;
            CreateDivider(modal.transform, "DeckDivider", new Vector2(0, 82), new Vector2(810, 2));

            GameObject cards = CreatePanel(modal.transform, "CardsContainer");
            RectTransform cr = cards.GetComponent<RectTransform>();
            cr.anchorMin = new Vector2(0f, 0f);
            cr.anchorMax = new Vector2(1f, 1f);
            cr.offsetMin = new Vector2(28, 28);
            cr.offsetMax = new Vector2(-28, -88);
            HorizontalLayoutGroup hlg = cards.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 20;
            hlg.childForceExpandWidth = true;
            hlg.childForceExpandHeight = true;
            hlg.childAlignment = TextAnchor.MiddleCenter;

            UiFieldBinder.Set(cardUI, "cardUIPrefab", cardTemplate);
            UiFieldBinder.Set(cardUI, "cardsContainer", cards.transform);
            UiFieldBinder.Set(cardUI, "fullScreenDim", dim);
            UiFieldBinder.Set(cardUI, "modalPanel", modal.GetComponent<RectTransform>());
            UiFieldBinder.Set(cardUI, "modalCanvasGroup", modal.AddComponent<CanvasGroup>());
            UiFieldBinder.Set(cardUI, "headerText", header);
            UiFieldBinder.Set(cardUI, "subtitleText", sub);
            return panel;
        }


        static GameObject BuildRandomEventPanel(Transform parent)
        {
            GameObject panel = CreatePanel(parent, "RandomEventPanel");
            Stretch(panel);
            SetImage(panel, COL_DIM);
            RandomEventUI ui = panel.AddComponent<RandomEventUI>();

            GameObject box = CreateGlassPanel(panel.transform, "EventReport", Vector2.zero, new Vector2(620, 340), 0.5f, 0.5f);
            CreateTelemetryLabel(box.transform, "ReportLabel", "DAILY BIOLOGICAL REPORT", new Vector2(-210, 130), new Vector2(320, 24), 13);
            CreateDivider(box.transform, "ReportDivider", new Vector2(0, 100), new Vector2(560, 2));
            TextMeshProUGUI title = CreateTMP(box.transform, "TitleText", "RINGKASAN HARI", 32, COL_HEADER, FontStyles.Bold);
            SetAnchored(title.gameObject, 0.5f, 0.5f, new Vector2(0, 52), new Vector2(540, 50));
            TextMeshProUGUI desc = CreateTMP(box.transform, "DescriptionText", "Peristiwa tubuh...", 17, COL_SUB);
            SetAnchored(desc.gameObject, 0.5f, 0.5f, new Vector2(0, -36), new Vector2(520, 120));
            desc.alignment = TextAlignmentOptions.Center;
            Button cont = CreateButton(box.transform, "ContinueButton", "[ CONTINUE ]", new Vector2(0, -132), new Vector2(260, 48), COL_BTN);
            UiFieldBinder.Set(ui, "titleText", title);
            UiFieldBinder.Set(ui, "eventDescriptionText", desc);
            UiFieldBinder.Set(ui, "continueButton", cont);
            return panel;
        }


        static GameObject BuildGameOverPanel(Transform parent)
        {
            GameObject panel = CreatePanel(parent, "GameOverPanel");
            Stretch(panel);
            SetImage(panel, COL_DIM);
            GameOverUI ui = panel.AddComponent<GameOverUI>();
            GameObject box = CreateGlassPanel(panel.transform, "FailureReport", Vector2.zero, new Vector2(560, 310), 0.5f, 0.5f);
            CreateTelemetryLabel(box.transform, "FailureLabel", "SYSTEM FAILURE", new Vector2(-180, 112), new Vector2(260, 24), 13);
            TextMeshProUGUI title = CreateTMP(box.transform, "Title", "BODY SYSTEM COLLAPSE", 34, COL_CRIT, FontStyles.Bold);
            SetAnchored(title.gameObject, 0.5f, 0.5f, new Vector2(0, 55), new Vector2(500, 60));
            TextMeshProUGUI days = CreateTMP(box.transform, "Days", "Kamu bertahan 0 hari.", 20, COL_SUB);
            SetAnchored(days.gameObject, 0.5f, 0.5f, new Vector2(0, 0), new Vector2(460, 40));
            Button restart = CreateButton(box.transform, "Restart", "[ REBOOT RUN ]", new Vector2(0, -90), new Vector2(280, 52), new Color(0.20f, 0.08f, 0.09f, 0.95f));
            UiFieldBinder.Set(ui, "daysSurvivedText", days);
            UiFieldBinder.Set(ui, "restartButton", restart);
            return panel;
        }


        static GameObject BuildVictoryPanel(Transform parent)
        {
            GameObject panel = CreatePanel(parent, "VictoryPanel");
            Stretch(panel);
            SetImage(panel, COL_DIM);
            VictoryUI ui = panel.AddComponent<VictoryUI>();
            GameObject box = CreateGlassPanel(panel.transform, "SurvivalReport", Vector2.zero, new Vector2(560, 300), 0.5f, 0.5f);
            CreateTelemetryLabel(box.transform, "SuccessLabel", "SURVIVAL COMPLETE", new Vector2(-178, 105), new Vector2(260, 24), 13);
            TextMeshProUGUI title = CreateTMP(box.transform, "Title", "100 DAYS STABILIZED", 36, COL_GREEN, FontStyles.Bold);
            SetAnchored(title.gameObject, 0.5f, 0.5f, new Vector2(0, 45), new Vector2(500, 62));
            TextMeshProUGUI sub = CreateTMP(box.transform, "Subtitle", "Chronic threats contained. Body telemetry remains operational.", 16, COL_SUB);
            SetAnchored(sub.gameObject, 0.5f, 0.5f, new Vector2(0, -20), new Vector2(460, 60));
            Button restart = CreateButton(box.transform, "Restart", "[ RUN NEW SIMULATION ]", new Vector2(0, -96), new Vector2(320, 52), COL_BTN);
            UiFieldBinder.Set(ui, "restartButton", restart);
            return panel;
        }


        static GameObject BuildPausePanel(Transform parent)
        {
            GameObject panel = CreatePanel(parent, "PausePanel");
            Stretch(panel);
            SetImage(panel, COL_DIM);
            PauseMenuUI ui = panel.AddComponent<PauseMenuUI>();
            GameObject box = CreateGlassPanel(panel.transform, "PauseBox", Vector2.zero, new Vector2(420, 230), 0.5f, 0.5f);
            CreateTelemetryLabel(box.transform, "PauseLabel", "SIMULATION PAUSED", new Vector2(-135, 70), new Vector2(240, 24), 13);
            CreateTMP(box.transform, "Title", "PAUSE", 34, COL_HEADER, FontStyles.Bold).rectTransform.anchoredPosition = new Vector2(0, 22);
            Button resume = CreateButton(box.transform, "Resume", "[ RESUME ]", new Vector2(0, -35), new Vector2(260, 46), COL_BTN);
            Button quit = CreateButton(box.transform, "Quit", "[ EXIT TO MENU ]", new Vector2(0, -92), new Vector2(260, 46), new Color(0.16f, 0.08f, 0.09f, 0.95f));
            UiFieldBinder.Set(ui, "resumeButton", resume);
            UiFieldBinder.Set(ui, "quitButton", quit);
            return panel;
        }


        // ─── RUNTIME TEMPLATES ───
        static GameObject BuildCardTemplate()
        {
            GameObject root = new GameObject("CardUI_Template", typeof(RectTransform), typeof(LayoutElement), typeof(CanvasGroup), typeof(CardUI));
            root.SetActive(false);
            Object.DontDestroyOnLoad(root);

            LayoutElement le = root.GetComponent<LayoutElement>();
            le.minWidth = 245;
            le.preferredWidth = 260;
            le.flexibleWidth = 1;

            SetImage(root, COL_PANEL_DARK);
            Image outline = CreateStretchImage(root.transform, "Outline", new Color(COL_TEAL.r, COL_TEAL.g, COL_TEAL.b, 0.55f));
            outline.GetComponent<RectTransform>().offsetMin = new Vector2(-2, -2);
            outline.GetComponent<RectTransform>().offsetMax = new Vector2(2, 2);
            outline.transform.SetAsFirstSibling();
            Image glow = CreateStretchImage(root.transform, "Glow", new Color(COL_TEAL.r, COL_TEAL.g, COL_TEAL.b, 0.10f));
            glow.GetComponent<RectTransform>().offsetMin = new Vector2(-8, -8);
            glow.GetComponent<RectTransform>().offsetMax = new Vector2(8, 8);
            glow.transform.SetAsFirstSibling();

            GameObject icon = CreatePanel(root.transform, "CategoryStripe");
            RectTransform stripe = icon.GetComponent<RectTransform>();
            stripe.anchorMin = new Vector2(0, 1);
            stripe.anchorMax = new Vector2(1, 1);
            stripe.pivot = new Vector2(0.5f, 1f);
            stripe.anchoredPosition = Vector2.zero;
            stripe.sizeDelta = new Vector2(0, 5);
            SetImage(icon, COL_TEAL);

            TextMeshProUGUI title = CreateTMP(root.transform, "Title", "ACTION", 18, COL_HEADER, FontStyles.Bold);
            SetAnchored(title.gameObject, 0.5f, 1f, new Vector2(0, -32), new Vector2(220, 42));
            title.alignment = TextAlignmentOptions.Left;
            title.characterSpacing = 4;
            TextMeshProUGUI effects = CreateTMP(root.transform, "Effects", "IMPACT:\n+ Energy\n- Stress", 13, COL_GREEN);
            effects.alignment = TextAlignmentOptions.Left;
            SetAnchored(effects.gameObject, 0.5f, 0.5f, new Vector2(0, 18), new Vector2(215, 100));
            TextMeshProUGUI desc = CreateTMP(root.transform, "Desc", "Clinical action description.", 11, COL_SUB);
            desc.alignment = TextAlignmentOptions.Left;
            SetAnchored(desc.gameObject, 0.5f, 0.5f, new Vector2(0, -70), new Vector2(215, 60));
            Button footerBtn = CreateButton(root.transform, "SelectBtn", "[ CONFIRM ]", new Vector2(0, -142), new Vector2(200, 38), COL_BTN);

            CardUI card = root.GetComponent<CardUI>();
            UiFieldBinder.Set(card, "titleText", title);
            UiFieldBinder.Set(card, "descriptionText", desc);
            UiFieldBinder.Set(card, "effectsText", effects);
            UiFieldBinder.Set(card, "footerButtonText", footerBtn.GetComponentInChildren<TextMeshProUGUI>());
            UiFieldBinder.Set(card, "bgImage", root.GetComponent<Image>());
            UiFieldBinder.Set(card, "outlineImage", outline);
            UiFieldBinder.Set(card, "glowImage", glow);
            UiFieldBinder.Set(card, "iconPlaceholder", icon.GetComponent<Image>());
            UiFieldBinder.Set(card, "selectButton", footerBtn);
            UiFieldBinder.Set(card, "canvasGroup", root.GetComponent<CanvasGroup>());
            return root;
        }


        static GameObject BuildChipTemplate()
        {
            GameObject root = new GameObject("CompactStatChip_Template", typeof(RectTransform), typeof(LayoutElement), typeof(CompactStatChipUI));
            root.SetActive(false);
            Object.DontDestroyOnLoad(root);

            LayoutElement le = root.GetComponent<LayoutElement>();
            le.minHeight = 32;
            le.preferredHeight = 34;
            le.flexibleWidth = 1;
            SetImage(root, new Color(0.045f, 0.06f, 0.08f, 0.78f));

            TextMeshProUGUI label = CreateTMP(root.transform, "Label", "ENERGY", 10, COL_HEADER, FontStyles.Bold);
            label.alignment = TextAlignmentOptions.Left;
            label.rectTransform.anchorMin = new Vector2(0, 0.45f);
            label.rectTransform.anchorMax = new Vector2(0.7f, 1f);
            label.rectTransform.offsetMin = new Vector2(8, 0);
            label.rectTransform.offsetMax = new Vector2(0, -2);
            label.characterSpacing = 3;

            TextMeshProUGUI val = CreateTMP(root.transform, "Value", "70%", 10, COL_TEAL, FontStyles.Bold);
            val.alignment = TextAlignmentOptions.Right;
            val.rectTransform.anchorMin = new Vector2(0.65f, 0.45f);
            val.rectTransform.anchorMax = new Vector2(1, 1f);
            val.rectTransform.offsetMax = new Vector2(-8, -2);

            GameObject track = CreatePanel(root.transform, "Track");
            RectTransform tr = track.GetComponent<RectTransform>();
            tr.anchorMin = new Vector2(0.03f, 0.16f);
            tr.anchorMax = new Vector2(0.97f, 0.27f);
            tr.offsetMin = Vector2.zero;
            tr.offsetMax = Vector2.zero;
            SetImage(track, COL_TRACK);

            GameObject fill = CreatePanel(track.transform, "Fill");
            Stretch(fill);
            Image fillImg = fill.GetComponent<Image>();
            fillImg.color = COL_GREEN;
            fillImg.type = Image.Type.Filled;
            fillImg.fillMethod = Image.FillMethod.Horizontal;

            CompactStatChipUI chip = root.GetComponent<CompactStatChipUI>();
            UiFieldBinder.Set(chip, "labelText", label);
            UiFieldBinder.Set(chip, "valueText", val);
            UiFieldBinder.Set(chip, "fillBar", fillImg);
            return root;
        }


        static GameObject BuildStatBarTemplate()
        {
            GameObject root = new GameObject("BodyStatBar_Template", typeof(RectTransform), typeof(LayoutElement), typeof(BodyStatBar));
            root.SetActive(false);
            Object.DontDestroyOnLoad(root);

            LayoutElement le = root.GetComponent<LayoutElement>();
            le.minHeight = 40;
            le.preferredHeight = 42;
            SetImage(root, new Color(0.045f, 0.06f, 0.08f, 0.78f));

            TextMeshProUGUI name = CreateTMP(root.transform, "Name", "STAT", 12, COL_HEADER, FontStyles.Bold);
            name.alignment = TextAlignmentOptions.Left;
            name.characterSpacing = 3;
            name.rectTransform.anchorMin = new Vector2(0, 0.45f);
            name.rectTransform.anchorMax = new Vector2(0.45f, 1f);
            name.rectTransform.offsetMin = new Vector2(10, 0);

            TextMeshProUGUI value = CreateTMP(root.transform, "Value", "50%", 12, COL_TEAL, FontStyles.Bold);
            value.alignment = TextAlignmentOptions.Right;
            value.rectTransform.anchorMin = new Vector2(0.72f, 0.45f);
            value.rectTransform.anchorMax = new Vector2(1, 1f);
            value.rectTransform.offsetMax = new Vector2(-10, -2);

            GameObject track = CreatePanel(root.transform, "Track");
            RectTransform tr = track.GetComponent<RectTransform>();
            tr.anchorMin = new Vector2(0.03f, 0.16f);
            tr.anchorMax = new Vector2(0.97f, 0.28f);
            tr.offsetMin = Vector2.zero;
            tr.offsetMax = Vector2.zero;
            SetImage(track, COL_TRACK);

            GameObject fill = CreatePanel(track.transform, "Fill");
            Stretch(fill);
            Image fillImg = fill.GetComponent<Image>();
            fillImg.color = COL_GREEN;
            fillImg.type = Image.Type.Filled;
            fillImg.fillMethod = Image.FillMethod.Horizontal;

            BodyStatBar bar = root.GetComponent<BodyStatBar>();
            UiFieldBinder.Set(bar, "nameText", name);
            UiFieldBinder.Set(bar, "valueText", value);
            UiFieldBinder.Set(bar, "fillBar", fillImg);
            return root;
        }


        static GameObject BuildDiseaseTemplate()
        {
            GameObject root = new GameObject("DiseaseProgress_Template", typeof(RectTransform), typeof(LayoutElement), typeof(DiseaseProgressUI));
            root.SetActive(false);
            Object.DontDestroyOnLoad(root);

            LayoutElement le = root.GetComponent<LayoutElement>();
            le.flexibleWidth = 1;
            le.minHeight = 26;
            le.preferredHeight = 28;
            SetImage(root, new Color(0.045f, 0.06f, 0.08f, 0.70f));

            TextMeshProUGUI name = CreateTMP(root.transform, "Name", "DIABETES", 10, COL_HEADER, FontStyles.Bold);
            name.alignment = TextAlignmentOptions.Left;
            name.rectTransform.anchorMin = new Vector2(0, 0.38f);
            name.rectTransform.anchorMax = new Vector2(0.55f, 1f);
            name.rectTransform.offsetMin = new Vector2(8, 0);
            TextMeshProUGUI progress = CreateTMP(root.transform, "Progress", "0%", 10, COL_TEAL, FontStyles.Bold);
            progress.alignment = TextAlignmentOptions.Right;
            progress.rectTransform.anchorMin = new Vector2(0.72f, 0.38f);
            progress.rectTransform.anchorMax = new Vector2(1, 1f);
            progress.rectTransform.offsetMax = new Vector2(-8, 0);
            TextMeshProUGUI stage = CreateTMP(root.transform, "Stage", "AMAN", 8, COL_SUB);
            stage.alignment = TextAlignmentOptions.Left;
            stage.rectTransform.anchorMin = new Vector2(0.55f, 0.38f);
            stage.rectTransform.anchorMax = new Vector2(0.78f, 1f);

            GameObject track = CreatePanel(root.transform, "Track");
            RectTransform tr = track.GetComponent<RectTransform>();
            tr.anchorMin = new Vector2(0.03f, 0.14f);
            tr.anchorMax = new Vector2(0.97f, 0.25f);
            tr.offsetMin = Vector2.zero;
            tr.offsetMax = Vector2.zero;
            SetImage(track, COL_TRACK);

            GameObject fill = CreatePanel(track.transform, "Fill");
            Stretch(fill);
            Image fillImg = fill.GetComponent<Image>();
            fillImg.color = COL_GREEN;
            fillImg.type = Image.Type.Filled;
            fillImg.fillMethod = Image.FillMethod.Horizontal;

            DiseaseProgressUI ui = root.GetComponent<DiseaseProgressUI>();
            UiFieldBinder.Set(ui, "diseaseNameText", name);
            UiFieldBinder.Set(ui, "progressText", progress);
            UiFieldBinder.Set(ui, "stageText", stage);
            UiFieldBinder.Set(ui, "fillBar", fillImg);
            return root;
        }


        // ─── HELPERS ───
        struct SummarySlot
        {
            public TextMeshProUGUI valueText;
            public Image fillImage;
        }

        static GameObject CreatePanel(Transform parent, string name)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            go.AddComponent<CanvasRenderer>();
            go.AddComponent<Image>();
            return go;
        }

        static void Stretch(GameObject go)
        {
            RectTransform r = go.GetComponent<RectTransform>();
            r.anchorMin = Vector2.zero;
            r.anchorMax = Vector2.one;
            r.offsetMin = Vector2.zero;
            r.offsetMax = Vector2.zero;
        }

        static void SetAnchored(GameObject go, float ax, float ay, Vector2 pos, Vector2 size)
        {
            RectTransform r = go.GetComponent<RectTransform>();
            r.anchorMin = r.anchorMax = new Vector2(ax, ay);
            r.pivot = new Vector2(0.5f, 0.5f);
            r.anchoredPosition = pos;
            r.sizeDelta = size;
        }

        static void SetImage(GameObject go, Color c)
        {
            Image img = go.GetComponent<Image>();
            if (img == null) img = go.AddComponent<Image>();
            img.color = c;
        }

        static Image CreateStretchImage(Transform parent, string name, Color c)
        {
            GameObject go = CreatePanel(parent, name);
            Stretch(go);
            SetImage(go, c);
            return go.GetComponent<Image>();
        }

        static void StylePanel(GameObject go)
        {
            StylePanel(go, COL_PANEL, COL_BORDER);
        }

        static void StylePanel(GameObject go, Color fill, Color border)
        {
            SetImage(go, fill);
            Image outline = CreateStretchImage(go.transform, "Border", border);
            outline.raycastTarget = false;
            outline.GetComponent<RectTransform>().offsetMin = new Vector2(-1.5f, -1.5f);
            outline.GetComponent<RectTransform>().offsetMax = new Vector2(1.5f, 1.5f);
            outline.transform.SetAsFirstSibling();
        }

        static GameObject CreateGlassPanel(Transform parent, string name, Vector2 pos, Vector2 size, float ax, float ay)
        {
            GameObject go = CreatePanel(parent, name);
            SetAnchored(go, ax, ay, pos, size);
            StylePanel(go);
            return go;
        }

        static Image CreateDivider(Transform parent, string name, Vector2 pos, Vector2 size)
        {
            GameObject go = CreatePanel(parent, name);
            SetAnchored(go, 0.5f, 0.5f, pos, size);
            SetImage(go, new Color(COL_BORDER.r, COL_BORDER.g, COL_BORDER.b, 0.55f));
            return go.GetComponent<Image>();
        }

        static TextMeshProUGUI CreateTelemetryLabel(Transform parent, string name, string text, Vector2 pos, Vector2 size, float fontSize = 12f)
        {
            TextMeshProUGUI label = CreateTMP(parent, name, text.ToUpperInvariant(), fontSize, COL_SUB, FontStyles.Bold);
            SetAnchored(label.gameObject, 0.5f, 0.5f, pos, size);
            label.alignment = TextAlignmentOptions.Left;
            label.characterSpacing = 8;
            return label;
        }

        static TextMeshProUGUI CreateTMP(Transform parent, string name, string text, float size, Color col, FontStyles style = FontStyles.Normal)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.color = col;
            tmp.fontStyle = style;
            tmp.alignment = TextAlignmentOptions.Center;
            return tmp;
        }

        static Button CreateButton(Transform parent, string name, string label, Vector2 pos, Vector2 size, Color bg)
        {
            GameObject go = CreatePanel(parent, name);
            SetAnchored(go, 0.5f, 0.5f, pos, size);
            StylePanel(go, bg, new Color(COL_BORDER.r, COL_BORDER.g, COL_BORDER.b, 0.7f));
            Button btn = go.AddComponent<Button>();
            TextMeshProUGUI txt = CreateTMP(go.transform, "Text", label, 16, COL_HEADER, FontStyles.Bold);
            Stretch(txt.gameObject);
            txt.characterSpacing = 4;
            return btn;
        }

        static SummarySlot CreateSummarySlot(Transform parent, string id, string label, Color barColor)
        {
            GameObject slot = CreatePanel(parent, id + "Slot");
            LayoutElement le = slot.AddComponent<LayoutElement>();
            le.flexibleWidth = 1;
            le.minHeight = 36;

            CreateTMP(slot.transform, "Label", label, 9, COL_SUB, FontStyles.Bold)
                .rectTransform.anchoredPosition = new Vector2(0, 10);

            TextMeshProUGUI val = CreateTMP(slot.transform, "Value", "0%", 12, COL_HEADER, FontStyles.Bold);
            val.rectTransform.anchoredPosition = new Vector2(0, -2);

            GameObject track = CreatePanel(slot.transform, "Track");
            RectTransform tr = track.GetComponent<RectTransform>();
            tr.anchorMin = new Vector2(0.05f, 0f);
            tr.anchorMax = new Vector2(0.95f, 0f);
            tr.pivot = new Vector2(0.5f, 0f);
            tr.anchoredPosition = new Vector2(0, 4);
            tr.sizeDelta = new Vector2(0, 6);
            SetImage(track, new Color(0.12f, 0.08f, 0.1f, 0.9f));

            GameObject fill = CreatePanel(track.transform, "Fill");
            Stretch(fill);
            Image fillImg = fill.GetComponent<Image>();
            fillImg.color = barColor;
            fillImg.type = Image.Type.Filled;
            fillImg.fillMethod = Image.FillMethod.Horizontal;

            return new SummarySlot { valueText = val, fillImage = fillImg };
        }
    }
}
