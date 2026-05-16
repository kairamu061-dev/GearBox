using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class GearBoxSetupWindow : EditorWindow
{
    static readonly string ScenesPath = "Assets/Scenes";

    static readonly (string name, System.Action setup)[] Scenes =
    {
        ("LogoScene",        SetupLogoScene),
        ("TitleScene",       SetupTitleScene),
        ("MapScene",         SetupMapScene),
        ("PreparationScene", SetupPreparationScene),
        ("BattleScene",      SetupBattleScene),
        ("ShopScene",        SetupShopScene),
        ("RefitScene",       SetupRefitScene),
        ("ResultScene",      SetupResultScene),
        ("GameOverScene",    SetupGameOverScene),
    };

    [MenuItem("GearBox/Scene Setup Window")]
    static void Open() => GetWindow<GearBoxSetupWindow>("GearBox Setup");

    void OnGUI()
    {
        GUILayout.Label("GearBox シーンセットアップ", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("初回は「① プレハブ生成」を先に実行してください。", MessageType.Info);
        EditorGUILayout.Space(8);

        if (GUILayout.Button("① プレハブ・ScriptableObject を生成", GUILayout.Height(32)))
            EditorApplication.delayCall += () =>
            {
                GearBoxPrefabBuilder.BuildAll();
                EditorUtility.DisplayDialog("完了", "プレハブ・ScriptableObject の生成が完了しました。", "OK");
            };

        EditorGUILayout.Space(4);

        if (GUILayout.Button("② 全シーンをセットアップ + ビルド設定更新", GUILayout.Height(32)))
            EditorApplication.delayCall += SetupAll;

        EditorGUILayout.Space(8);
        if (GUILayout.Button("③ TMP フォントを一括修正（警告が出る場合）", GUILayout.Height(28)))
            EditorApplication.delayCall += FixTMPFonts;

        EditorGUILayout.Space(8);
        GUILayout.Label("個別セットアップ", EditorStyles.boldLabel);
        foreach (var (name, setup) in Scenes)
        {
            var captured = (name, setup);
            if (GUILayout.Button(captured.name))
                EditorApplication.delayCall += () => RunSetup(captured.name, captured.setup);
        }
    }

    static void SetupAll()
    {
        foreach (var (name, setup) in Scenes)
            RunSetup(name, setup);
        UpdateBuildSettings();
        EditorUtility.DisplayDialog("完了", "全シーンのセットアップが完了しました。", "OK");
    }

    static void RunSetup(string sceneName, System.Action setup)
    {
        if (!Directory.Exists(ScenesPath)) Directory.CreateDirectory(ScenesPath);
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        setup();
        EditorSceneManager.SaveScene(scene, $"{ScenesPath}/{sceneName}.unity");
        Debug.Log($"[GearBox] {sceneName} をセットアップしました。");
    }

    // ────────────────────────────────────────────
    // LogoScene
    // ────────────────────────────────────────────
    static void SetupLogoScene()
    {
        // カメラ（Screen Space Overlay Canvas のみでも必要）
        var camGo = new GameObject("Main Camera");
        var cam = camGo.AddComponent<Camera>();
        cam.backgroundColor = Color.black;
        cam.tag = "MainCamera";
        cam.cullingMask = 0; // 何も描画しない（UI のみ）

        // マネージャ群（DontDestroyOnLoad）
        CreateManager<RunManager>("RunManager");
        CreateManager<SceneTransitionManager>("SceneTransitionManager");
        CreateManager<SaveManager>("SaveManager");
        var audioGo = CreateManager<AudioManager>("AudioManager");
        audioGo.AddComponent<AudioSource>(); // BGM
        audioGo.AddComponent<AudioSource>(); // SE

        // Canvas
        var canvas = CreateCanvas("LogoCanvas");
        CreateEventSystem();

        // 黒背景
        var bg = CreateUIImage(canvas.transform, "Background", Color.black);
        SetStretch(bg.GetComponent<RectTransform>());

        // ロゴ表示用Image
        var logoImg = CreateUIImage(canvas.transform, "LogoImage", Color.white);
        SetStretch(logoImg.GetComponent<RectTransform>());
        logoImg.color = new Color(1, 1, 1, 0);

        // LogoSceneController
        var ctrlGo = new GameObject("LogoSceneController");
        var ctrl = ctrlGo.AddComponent<LogoSceneController>();
        var so = new SerializedObject(ctrl);
        so.FindProperty("logoImage").objectReferenceValue = logoImg;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    // ────────────────────────────────────────────
    // TitleScene
    // ────────────────────────────────────────────
    static void SetupTitleScene()
    {
        CreateUICamera();
        CreateEventSystem();
        var canvas = CreateCanvas("TitleCanvas");
        var ctrl = new GameObject("TitleSceneController").AddComponent<TitleSceneController>();

        // 背景
        var bg = CreateUIImage(canvas.transform, "Background", new Color(0.1f, 0.08f, 0.05f));
        SetStretch(bg.GetComponent<RectTransform>());

        // ボタングループ（データなし）
        var groupNoSave = CreatePanel(canvas.transform, "GroupNoSave", new Vector2(0, -50), new Vector2(200, 300));
        var btnStart   = CreateButton(groupNoSave.transform, "BtnStart",   "はじめる",          new Vector2(0, 60));
        var btnPastRuns1 = CreateButton(groupNoSave.transform, "BtnPastRuns", "過去のラン",       new Vector2(0, 0));
        var btnSettings1 = CreateButton(groupNoSave.transform, "BtnSettings", "せってい",        new Vector2(0, -60));
        var btnQuit1   = CreateButton(groupNoSave.transform, "BtnQuit",    "おわる",            new Vector2(0, -120));

        // ボタングループ（データあり）
        var groupSave = CreatePanel(canvas.transform, "GroupWithSave", new Vector2(0, -50), new Vector2(200, 360));
        groupSave.SetActive(false);
        var btnContinue  = CreateButton(groupSave.transform, "BtnContinue",  "つづきから",       new Vector2(0, 90));
        var btnNewGame   = CreateButton(groupSave.transform, "BtnNewGame",   "あたらしくはじめる", new Vector2(0, 30));
        var btnPastRuns2 = CreateButton(groupSave.transform, "BtnPastRuns",  "過去のラン",       new Vector2(0, -30));
        var btnSettings2 = CreateButton(groupSave.transform, "BtnSettings",  "せってい",         new Vector2(0, -90));
        var btnQuit2     = CreateButton(groupSave.transform, "BtnQuit",      "おわる",           new Vector2(0, -150));

        // 確認ダイアログ
        var confirmDlg = CreatePanel(canvas.transform, "ConfirmNewGameDialog", Vector2.zero, new Vector2(400, 200));
        confirmDlg.SetActive(false);
        CreateLabel(confirmDlg.transform, "Message", "中断データは削除されます。よろしいですか？", new Vector2(0, 40));
        var btnYes = CreateButton(confirmDlg.transform, "BtnYes", "はい",  new Vector2(-70, -30));
        var btnNo  = CreateButton(confirmDlg.transform, "BtnNo",  "いいえ", new Vector2(70, -30));

        // 設定パネル・過去のランパネル（プレースホルダ）
        var settingsPanel = CreatePanel(canvas.transform, "SettingsPanel", Vector2.zero, new Vector2(600, 500));
        settingsPanel.SetActive(false);
        CreateLabel(settingsPanel.transform, "Label", "設定（今後追加）", Vector2.zero);

        var pastRunsPanel = CreatePanel(canvas.transform, "PastRunsPanel", Vector2.zero, new Vector2(700, 600));
        pastRunsPanel.SetActive(false);
        CreateLabel(pastRunsPanel.transform, "Label", "過去のラン（今後追加）", Vector2.zero);

        // 初期タワー設定
        var steamCannon = GearBoxPrefabBuilder.LoadSO<TowerData>("Towers/TowerData_SteamCannon.asset");

        // TitleSceneController に参照をバインド
        var so = new SerializedObject(ctrl);
        so.FindProperty("btnStart").objectReferenceValue       = btnStart.GetComponent<Button>();
        so.FindProperty("btnContinue").objectReferenceValue    = btnContinue.GetComponent<Button>();
        so.FindProperty("btnNewGame").objectReferenceValue     = btnNewGame.GetComponent<Button>();
        so.FindProperty("btnPastRuns").objectReferenceValue    = btnPastRuns1.GetComponent<Button>();
        so.FindProperty("btnSettings").objectReferenceValue    = btnSettings1.GetComponent<Button>();
        so.FindProperty("btnQuit").objectReferenceValue        = btnQuit1.GetComponent<Button>();
        so.FindProperty("confirmNewGameDialog").objectReferenceValue = confirmDlg;
        so.FindProperty("btnConfirmYes").objectReferenceValue  = btnYes.GetComponent<Button>();
        so.FindProperty("btnConfirmNo").objectReferenceValue   = btnNo.GetComponent<Button>();
        so.FindProperty("settingsPanel").objectReferenceValue  = settingsPanel;
        so.FindProperty("pastRunsPanel").objectReferenceValue  = pastRunsPanel;
        if (steamCannon != null)
            so.FindProperty("startTowerData").objectReferenceValue = steamCannon;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    // ────────────────────────────────────────────
    // MapScene
    // ────────────────────────────────────────────
    static void SetupMapScene()
    {
        CreateUICamera();
        CreateEventSystem();
        var canvas = CreateCanvas("MapCanvas");
        var ctrl = new GameObject("MapSceneController").AddComponent<MapSceneController>();

        // 背景
        var bg = CreateUIImage(canvas.transform, "Background", new Color(0.07f, 0.05f, 0.03f));
        SetStretch(bg.GetComponent<RectTransform>());

        // マップコンテナ
        var mapContainer = CreatePanel(canvas.transform, "MapScrollContainer", Vector2.zero, new Vector2(800, 900));
        var graphViewGo = new GameObject("MapGraphView");
        graphViewGo.transform.SetParent(mapContainer.transform, false);
        var graphView = graphViewGo.AddComponent<MapGraphView>();
        var mapContainerRT = mapContainer.GetComponent<RectTransform>();
        var nodeButtonPrefabGO = GearBoxPrefabBuilder.LoadPrefabGO("UI/NodeButtonUI.prefab");
        var nodeButtonPrefab = nodeButtonPrefabGO?.GetComponent<NodeButtonUI>();

        var graphViewSO = new SerializedObject(graphView);
        graphViewSO.FindProperty("nodeButtonPrefab").objectReferenceValue = nodeButtonPrefab;
        graphViewSO.FindProperty("mapContainer").objectReferenceValue = mapContainerRT;
        graphViewSO.ApplyModifiedPropertiesWithoutUndo();

        // HUD（右上）
        var hud = CreatePanel(canvas.transform, "MapHUD", new Vector2(0, 0), new Vector2(300, 60));
        var hudRT = hud.GetComponent<RectTransform>();
        hudRT.anchorMin = hudRT.anchorMax = new Vector2(1, 1);
        hudRT.pivot = new Vector2(1, 1);
        hudRT.anchoredPosition = new Vector2(-10, -10);
        var areaLabel = CreateLabel(hud.transform, "AreaLabel", "エリア 1", new Vector2(-80, 0));
        var scrapText = CreateLabel(hud.transform, "ScrapText", "100 Sc",   new Vector2(60, 0));

        // メニューボタン（右上）
        var menuBtnGo = CreateButton(canvas.transform, "MenuButton", "≡", new Vector2(-40, -40));
        var menuBtnRT = menuBtnGo.GetComponent<RectTransform>();
        menuBtnRT.anchorMin = menuBtnRT.anchorMax = new Vector2(1, 1);
        menuBtnRT.pivot = new Vector2(1, 1);
        menuBtnRT.anchoredPosition = new Vector2(-10, -70);
        menuBtnRT.sizeDelta = new Vector2(60, 60);

        // メニューオーバーレイ
        var menuOverlay = CreatePanel(canvas.transform, "MenuOverlay", Vector2.zero, new Vector2(300, 280));
        menuOverlay.SetActive(false);
        var btnMenuSettings = CreateButton(menuOverlay.transform, "BtnSettings", "設定",        new Vector2(0, 80));
        var btnGiveUp       = CreateButton(menuOverlay.transform, "BtnGiveUp",   "あきらめる",   new Vector2(0, 0));
        var btnSuspend      = CreateButton(menuOverlay.transform, "BtnSuspend",  "中断",         new Vector2(0, -80));

        // あきらめる確認
        var giveUpConfirm = CreatePanel(canvas.transform, "GiveUpConfirm", Vector2.zero, new Vector2(400, 180));
        giveUpConfirm.SetActive(false);
        CreateLabel(giveUpConfirm.transform, "Message", "ランを終了しますか？", new Vector2(0, 40));
        var btnGiveUpYes = CreateButton(giveUpConfirm.transform, "BtnYes", "はい",  new Vector2(-70, -30));
        var btnGiveUpNo  = CreateButton(giveUpConfirm.transform, "BtnNo",  "いいえ", new Vector2(70, -30));

        // 設定パネル
        var settingsPanel = CreatePanel(canvas.transform, "SettingsPanel", Vector2.zero, new Vector2(600, 500));
        settingsPanel.SetActive(false);
        CreateLabel(settingsPanel.transform, "Label", "設定（今後追加）", Vector2.zero);

        // ハテナイベント UI
        var mysteryOverlay = CreatePanel(canvas.transform, "MysteryOverlay", Vector2.zero, new Vector2(600, 500));
        mysteryOverlay.SetActive(false);
        var mysteryUI = mysteryOverlay.AddComponent<MysteryEventUI>();
        var mTitle   = CreateLabel(mysteryOverlay.transform, "TitleText",   "タイトル",   new Vector2(0, 180));
        var mDesc    = CreateLabel(mysteryOverlay.transform, "DescText",    "説明",        new Vector2(0, 100));
        mDesc.GetComponent<RectTransform>().sizeDelta = new Vector2(500, 80);
        var choiceRoot = new GameObject("ChoiceRoot");
        choiceRoot.transform.SetParent(mysteryOverlay.transform, false);
        choiceRoot.AddComponent<RectTransform>().sizeDelta = new Vector2(500, 200);
        choiceRoot.AddComponent<VerticalLayoutGroup>().spacing = 8;
        var closeBtnGo = CreateButton(mysteryOverlay.transform, "BtnClose", "閉じる", new Vector2(0, -180));
        var choiceBtn  = CreateButton(mysteryOverlay.transform, "ChoicePrefab", "選択肢", Vector2.zero);
        choiceBtn.SetActive(false);

        var muSO = new SerializedObject(mysteryUI);
        muSO.FindProperty("overlay").objectReferenceValue     = mysteryOverlay;
        muSO.FindProperty("titleText").objectReferenceValue   = mTitle.GetComponent<TMP_Text>();
        muSO.FindProperty("descText").objectReferenceValue    = mDesc.GetComponent<TMP_Text>();
        muSO.FindProperty("choiceRoot").objectReferenceValue  = choiceRoot.transform;
        muSO.FindProperty("choicePrefab").objectReferenceValue = choiceBtn.GetComponent<Button>();
        muSO.FindProperty("btnClose").objectReferenceValue    = closeBtnGo.GetComponent<Button>();
        muSO.ApplyModifiedPropertiesWithoutUndo();

        // MapSceneController に参照バインド
        var so = new SerializedObject(ctrl);
        so.FindProperty("graphView").objectReferenceValue         = graphView;
        so.FindProperty("mapScrollContainer").objectReferenceValue = mapContainerRT;
        so.FindProperty("areaLabel").objectReferenceValue         = areaLabel.GetComponent<TMP_Text>();
        so.FindProperty("scrapText").objectReferenceValue         = scrapText.GetComponent<TMP_Text>();
        so.FindProperty("menuOverlay").objectReferenceValue       = menuOverlay;
        so.FindProperty("btnMenu").objectReferenceValue           = menuBtnGo.GetComponent<Button>();
        so.FindProperty("btnMenuSettings").objectReferenceValue   = btnMenuSettings.GetComponent<Button>();
        so.FindProperty("btnGiveUp").objectReferenceValue         = btnGiveUp.GetComponent<Button>();
        so.FindProperty("btnSuspend").objectReferenceValue        = btnSuspend.GetComponent<Button>();
        so.FindProperty("giveUpConfirm").objectReferenceValue     = giveUpConfirm;
        so.FindProperty("btnGiveUpYes").objectReferenceValue      = btnGiveUpYes.GetComponent<Button>();
        so.FindProperty("btnGiveUpNo").objectReferenceValue       = btnGiveUpNo.GetComponent<Button>();
        so.FindProperty("settingsPanel").objectReferenceValue     = settingsPanel;
        so.FindProperty("mysteryEventUI").objectReferenceValue    = mysteryUI;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    // ────────────────────────────────────────────
    // 残りのシーン（プレースホルダ）
    // ────────────────────────────────────────────
    static void SetupPreparationScene()
    {
        CreateEventSystem();
        var canvas = CreateCanvas("PreparationCanvas");
        var ctrl = new GameObject("PreparationSceneController")
            .AddComponent<PreparationSceneController>();

        var bg = CreateUIImage(canvas.transform, "Background", new Color(0.07f, 0.05f, 0.03f));
        SetStretch(bg.GetComponent<RectTransform>());

        // タワー一覧（左）
        var listPanel = CreatePanel(canvas.transform, "TowerListPanel", new Vector2(-580, 0), new Vector2(240, 700));
        CreateLabel(listPanel.transform, "ListTitle", "所持タワー", new Vector2(0, 320));
        var scrollView = new GameObject("ScrollView");
        scrollView.transform.SetParent(listPanel.transform, false);
        var svRT = scrollView.AddComponent<RectTransform>();
        svRT.anchoredPosition = new Vector2(0, -20);
        svRT.sizeDelta = new Vector2(220, 620);
        var scroll = scrollView.AddComponent<ScrollRect>();
        scroll.horizontal = false;
        var viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollView.transform, false);
        var vpRT = viewport.AddComponent<RectTransform>();
        vpRT.anchorMin = Vector2.zero; vpRT.anchorMax = Vector2.one;
        vpRT.offsetMin = vpRT.offsetMax = Vector2.zero;
        viewport.AddComponent<Mask>().showMaskGraphic = false;
        viewport.AddComponent<Image>().color = new Color(0, 0, 0, 0.01f);
        var content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        var contentRT = content.AddComponent<RectTransform>();
        contentRT.anchorMin = new Vector2(0, 1); contentRT.anchorMax = new Vector2(1, 1);
        contentRT.pivot = new Vector2(0.5f, 1); contentRT.offsetMin = contentRT.offsetMax = Vector2.zero;
        var vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 6; vlg.padding = new RectOffset(8, 8, 8, 8);
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlWidth = true; vlg.childForceExpandWidth = true;
        content.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        scroll.viewport = vpRT;
        scroll.content = contentRT;

        // グリッドUI（中央）
        var gridPanel = CreatePanel(canvas.transform, "GridPanel", new Vector2(100, 0), new Vector2(500, 500));
        CreateLabel(gridPanel.transform, "GridTitle", "グリッド配置", new Vector2(0, 220));
        var gridRoot = new GameObject("GridRoot");
        gridRoot.transform.SetParent(gridPanel.transform, false);
        gridRoot.AddComponent<RectTransform>().sizeDelta = new Vector2(450, 450);
        var gridUI = gridPanel.AddComponent<GridUI>();

        // ゴーストイメージ
        var ghostGo = new GameObject("GhostImage");
        ghostGo.transform.SetParent(canvas.transform, false);
        var ghost = ghostGo.AddComponent<Image>();
        ghost.color = new Color(1, 1, 1, 0.6f);
        ghost.raycastTarget = false;
        ghostGo.GetComponent<RectTransform>().sizeDelta = new Vector2(70, 70);
        ghostGo.SetActive(false);

        // 出撃ボタン（右下）
        var sortieBtn = CreateButton(canvas.transform, "SortieButton", "出 撃", new Vector2(400, -400));
        sortieBtn.GetComponent<RectTransform>().sizeDelta = new Vector2(220, 70);

        // HUD（右上）
        var scrapLabel = CreateLabel(canvas.transform, "ScrapText", "100 Sc", new Vector2(750, 490));

        // GridCellUI プレハブ取得
        var cellPrefabGO = GearBoxPrefabBuilder.LoadPrefabGO("UI/GridCellUI.prefab");
        var cellPrefab = cellPrefabGO?.GetComponent<GridCellUI>();

        // TowerCardUI プレハブ取得
        var cardPrefabGO = GearBoxPrefabBuilder.LoadPrefabGO("UI/TowerCardUI.prefab");
        var cardPrefab = cardPrefabGO?.GetComponent<TowerCardUI>();

        // GridUI 参照
        var gridSO = new SerializedObject(gridUI);
        gridSO.FindProperty("cellPrefab").objectReferenceValue = cellPrefab;
        gridSO.FindProperty("gridRoot").objectReferenceValue = gridRoot.GetComponent<RectTransform>();
        gridSO.ApplyModifiedPropertiesWithoutUndo();

        // PreparationSceneController 参照
        var so = new SerializedObject(ctrl);
        so.FindProperty("gridUI").objectReferenceValue        = gridUI;
        so.FindProperty("towerListRoot").objectReferenceValue = content.transform;
        so.FindProperty("towerCardPrefab").objectReferenceValue = cardPrefab;
        so.FindProperty("ghostImage").objectReferenceValue    = ghost;
        so.FindProperty("btnSortie").objectReferenceValue     = sortieBtn.GetComponent<Button>();
        so.FindProperty("scrapText").objectReferenceValue     = scrapLabel.GetComponent<TMP_Text>();
        so.ApplyModifiedPropertiesWithoutUndo();

        // 初期タワー（TitleSceneController にも設定）
        var steamCannon = GearBoxPrefabBuilder.LoadSO<TowerData>("Towers/TowerData_SteamCannon.asset");
        if (steamCannon != null)
        {
            var titleCtrl = Object.FindFirstObjectByType<TitleSceneController>();
            if (titleCtrl != null)
            {
                var tso = new SerializedObject(titleCtrl);
                tso.FindProperty("startTowerData").objectReferenceValue = steamCannon;
                tso.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }

    static void SetupBattleScene()
    {
        // カメラ
        var camGo = new GameObject("Main Camera");
        var cam = camGo.AddComponent<Camera>();
        cam.backgroundColor = new Color(0.07f, 0.07f, 0.07f);
        cam.tag = "MainCamera";
        cam.orthographic = true;
        cam.orthographicSize = 12f;
        var cameraFollow = camGo.AddComponent<CameraFollow>();

        // AimProvider
        new GameObject("AimProvider").AddComponent<AimProvider>();

        // スポーンポイント
        var spawnPoint = new GameObject("TankSpawnPoint");

        // FieldGenerator
        var fieldGen = new GameObject("FieldGenerator").AddComponent<FieldGenerator>();
        var tankPrefab      = GearBoxPrefabBuilder.LoadPrefabGO("Battle/Tank.prefab");
        var solidWall       = GearBoxPrefabBuilder.LoadPrefabGO("Battle/SolidWall.prefab");
        var destWall        = GearBoxPrefabBuilder.LoadPrefabGO("Battle/DestructibleWall.prefab");
        var goal            = GearBoxPrefabBuilder.LoadPrefabGO("Battle/GoalTrigger.prefab");
        var enemyChaser     = GearBoxPrefabBuilder.LoadPrefabGO("Enemy/EnemyChaser.prefab");
        var enemyRusher     = GearBoxPrefabBuilder.LoadPrefabGO("Enemy/EnemyRusher.prefab");
        var scrapObj        = GearBoxPrefabBuilder.LoadPrefabGO("Battle/ScrapObject.prefab");
        var enemyDataChaser = GearBoxPrefabBuilder.LoadSO<EnemyData>("Enemies/EnemyData_Chaser.asset");
        var enemyDataRusher = GearBoxPrefabBuilder.LoadSO<EnemyData>("Enemies/EnemyData_Rusher.asset");

        // EnemyController に ScrapObject 参照を注入するため、prefab 側に設定済みの想定
        // FieldGenerator 参照
        var fgSO = new SerializedObject(fieldGen);
        fgSO.FindProperty("solidWallPrefab").objectReferenceValue        = solidWall;
        fgSO.FindProperty("destructibleWallPrefab").objectReferenceValue = destWall;
        fgSO.FindProperty("goalPrefab").objectReferenceValue             = goal;
        var enemyList = fgSO.FindProperty("enemyDataList");
        enemyList.arraySize = 2;
        if (enemyDataChaser) enemyList.GetArrayElementAtIndex(0).objectReferenceValue = enemyDataChaser;
        if (enemyDataRusher) enemyList.GetArrayElementAtIndex(1).objectReferenceValue = enemyDataRusher;
        fgSO.ApplyModifiedPropertiesWithoutUndo();

        CreateEventSystem();

        // Canvas / HUD
        var canvas = CreateCanvas("BattleCanvas");
        var ctrl = new GameObject("BattleSceneController").AddComponent<BattleSceneController>();

        // HP バー
        var hpPanel = CreatePanel(canvas.transform, "HpPanel", new Vector2(-600, 490), new Vector2(300, 40));
        hpPanel.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        var hpBg = CreatePanel(hpPanel.transform, "HpBg", Vector2.zero, new Vector2(300, 30));
        hpBg.GetComponent<Image>().color = new Color(0.2f, 0.1f, 0.1f);
        var hpBarGo = new GameObject("HpBar");
        hpBarGo.transform.SetParent(hpPanel.transform, false);
        var hpSlider = hpBarGo.AddComponent<Slider>();
        var hpRT = hpBarGo.GetComponent<RectTransform>();
        hpRT.sizeDelta = new Vector2(300, 30);
        hpSlider.minValue = 0; hpSlider.maxValue = 100; hpSlider.value = 100;

        // スクラップテキスト
        var scrapTxt = CreateLabel(canvas.transform, "ScrapText", "100 Sc", new Vector2(750, 490));

        // クリア・ゲームオーバーテキスト
        var clearGo    = CreateLabel(canvas.transform, "ClearText",    "BATTLE CLEAR!", Vector2.zero);
        var gameOverGo = CreateLabel(canvas.transform, "GameOverText", "GAME OVER",     Vector2.zero);
        clearGo.fontSize = 48;
        gameOverGo.fontSize = 48;
        gameOverGo.color = new Color(0.9f, 0.2f, 0.2f);
        clearGo.gameObject.SetActive(false);
        gameOverGo.gameObject.SetActive(false);

        // ミニマップ（右下）
        var minimapPanel = CreatePanel(canvas.transform, "Minimap", new Vector2(820, -430), new Vector2(180, 180));
        minimapPanel.GetComponent<Image>().color = new Color(0.05f, 0.05f, 0.05f, 0.8f);
        var minimapUI = minimapPanel.AddComponent<MinimapUI>();
        var playerDot = CreateUIImage(minimapPanel.transform, "PlayerDot", new Color(0.3f, 0.8f, 0.3f));
        playerDot.GetComponent<RectTransform>().sizeDelta = new Vector2(8, 8);
        var goalDot = CreateUIImage(minimapPanel.transform, "GoalDot", new Color(0.3f, 0.3f, 0.9f));
        goalDot.GetComponent<RectTransform>().sizeDelta = new Vector2(8, 8);
        var arrowGo = CreateUIImage(minimapPanel.transform, "GoalArrow", new Color(1f, 0.8f, 0f));
        arrowGo.GetComponent<RectTransform>().sizeDelta = new Vector2(12, 20);
        var minimapSO = new SerializedObject(minimapUI);
        minimapSO.FindProperty("minimapRoot").objectReferenceValue = minimapPanel.GetComponent<RectTransform>();
        minimapSO.FindProperty("playerDot").objectReferenceValue   = playerDot;
        minimapSO.FindProperty("goalDot").objectReferenceValue     = goalDot;
        minimapSO.FindProperty("goalArrow").objectReferenceValue   = arrowGo;
        minimapSO.ApplyModifiedPropertiesWithoutUndo();

        // BattleSceneController 参照
        var so = new SerializedObject(ctrl);
        so.FindProperty("tankPrefab").objectReferenceValue      = tankPrefab;
        so.FindProperty("tankSpawnPoint").objectReferenceValue  = spawnPoint.transform;
        so.FindProperty("hpBar").objectReferenceValue           = hpSlider;
        so.FindProperty("scrapText").objectReferenceValue       = scrapTxt.GetComponent<TMP_Text>();
        so.FindProperty("clearText").objectReferenceValue       = clearGo.gameObject;
        so.FindProperty("gameOverText").objectReferenceValue    = gameOverGo.gameObject;
        so.FindProperty("cameraFollow").objectReferenceValue    = cameraFollow;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static void SetupShopScene()
    {
        CreateUICamera();
        CreateEventSystem();
        var canvas = CreateCanvas("ShopCanvas");
        var ctrl = new GameObject("ShopSceneController").AddComponent<ShopSceneController>();

        var bg = CreateUIImage(canvas.transform, "Background", new Color(0.07f, 0.05f, 0.03f));
        SetStretch(bg.GetComponent<RectTransform>());

        // HUD
        var scrapText = CreateLabel(canvas.transform, "ScrapText", "100 Sc", new Vector2(750, 490));

        // タブボタン
        var btnBuy  = CreateButton(canvas.transform, "BtnTabBuy",  "購入", new Vector2(-100, 430));
        var btnSell = CreateButton(canvas.transform, "BtnTabSell", "売却", new Vector2(100,  430));
        btnBuy.GetComponent<RectTransform>().sizeDelta  = new Vector2(160, 44);
        btnSell.GetComponent<RectTransform>().sizeDelta = new Vector2(160, 44);

        // 購入リスト
        var buyPanel = CreatePanel(canvas.transform, "BuyPanel", new Vector2(0, -30), new Vector2(800, 700));
        var buyContent = CreateScrollContent(buyPanel.transform, "BuyContent");

        // 売却パネル
        var sellPanel = CreatePanel(canvas.transform, "SellPanel", new Vector2(0, -30), new Vector2(800, 700));
        sellPanel.SetActive(false);
        var sellContent = CreateScrollContent(sellPanel.transform, "SellContent");

        // 閉じるボタン
        var btnClose = CreateButton(canvas.transform, "BtnClose", "出発", new Vector2(0, -480));

        // ShopItemUI プレハブ
        var itemPrefabGO = GearBoxPrefabBuilder.LoadPrefabGO("UI/ShopItemUI.prefab");
        var itemPrefab   = itemPrefabGO?.GetComponent<ShopItemUI>();

        var so = new SerializedObject(ctrl);
        so.FindProperty("itemListRoot").objectReferenceValue   = buyContent;
        so.FindProperty("sellListRoot").objectReferenceValue   = sellContent;
        so.FindProperty("sellPanel").objectReferenceValue      = sellPanel;
        so.FindProperty("scrapText").objectReferenceValue      = scrapText.GetComponent<TMP_Text>();
        so.FindProperty("btnClose").objectReferenceValue       = btnClose.GetComponent<Button>();
        so.FindProperty("btnTabBuy").objectReferenceValue      = btnBuy.GetComponent<Button>();
        so.FindProperty("btnTabSell").objectReferenceValue     = btnSell.GetComponent<Button>();
        if (itemPrefab)
        {
            so.FindProperty("itemPrefab").objectReferenceValue     = itemPrefab;
            so.FindProperty("sellCardPrefab").objectReferenceValue = itemPrefab;
        }
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static void SetupRefitScene()
    {
        CreateUICamera();
        CreateEventSystem();
        var canvas = CreateCanvas("RefitCanvas");
        var ctrl = new GameObject("RefitSceneController").AddComponent<RefitSceneController>();

        var bg = CreateUIImage(canvas.transform, "Background", new Color(0.07f, 0.05f, 0.03f));
        SetStretch(bg.GetComponent<RectTransform>());

        // HUD
        var scrapText = CreateLabel(canvas.transform, "ScrapText", "100 Sc", new Vector2(750, 490));

        // タブボタン
        var btnUpgrade = CreateButton(canvas.transform, "BtnTabUpgrade", "強化", new Vector2(-200, 430));
        var btnRepair  = CreateButton(canvas.transform, "BtnTabRepair",  "修理", new Vector2(0,    430));
        var btnExpand  = CreateButton(canvas.transform, "BtnTabExpand",  "拡張", new Vector2(200,  430));
        foreach (var b in new[]{btnUpgrade, btnRepair, btnExpand})
            b.GetComponent<RectTransform>().sizeDelta = new Vector2(160, 44);

        // 強化パネル
        var upgradePanel = CreatePanel(canvas.transform, "UpgradePanel", new Vector2(0, -30), new Vector2(700, 700));
        var upgradeContent = CreateScrollContent(upgradePanel.transform, "UpgradeContent");

        // 修理パネル
        var repairPanel = CreatePanel(canvas.transform, "RepairPanel", new Vector2(0, -30), new Vector2(500, 400));
        repairPanel.SetActive(false);
        CreateLabel(repairPanel.transform, "Title", "HP 修理", new Vector2(0, 150));
        var btnR30   = CreateButton(repairPanel.transform, "BtnRepair30",   "30% 回復  30Sc",    new Vector2(0, 60));
        var btnR70   = CreateButton(repairPanel.transform, "BtnRepair70",   "70% 回復  60Sc",    new Vector2(0, 0));
        var btnRFull = CreateButton(repairPanel.transform, "BtnRepairFull", "全回復    100Sc",   new Vector2(0, -60));

        // 拡張パネル
        var expandPanel = CreatePanel(canvas.transform, "ExpandPanel", new Vector2(0, -30), new Vector2(500, 400));
        expandPanel.SetActive(false);
        var gridSizeText = CreateLabel(expandPanel.transform, "GridSizeText", "現在: 3×3", new Vector2(0, 120));
        var btnCol = CreateButton(expandPanel.transform, "BtnExpandCol", "列を追加  80Sc", new Vector2(0, 30));
        var btnRow = CreateButton(expandPanel.transform, "BtnExpandRow", "行を追加  80Sc", new Vector2(0, -50));

        // 閉じるボタン
        var btnClose = CreateButton(canvas.transform, "BtnClose", "出発", new Vector2(0, -480));

        // ShopItemUI プレハブ（強化リスト流用）
        var itemPrefabGO = GearBoxPrefabBuilder.LoadPrefabGO("UI/ShopItemUI.prefab");
        var itemPrefab   = itemPrefabGO?.GetComponent<ShopItemUI>();

        var so = new SerializedObject(ctrl);
        so.FindProperty("btnTabUpgrade").objectReferenceValue  = btnUpgrade.GetComponent<Button>();
        so.FindProperty("btnTabRepair").objectReferenceValue   = btnRepair.GetComponent<Button>();
        so.FindProperty("btnTabExpand").objectReferenceValue   = btnExpand.GetComponent<Button>();
        so.FindProperty("upgradePanel").objectReferenceValue   = upgradePanel;
        so.FindProperty("repairPanel").objectReferenceValue    = repairPanel;
        so.FindProperty("expandPanel").objectReferenceValue    = expandPanel;
        so.FindProperty("upgradeListRoot").objectReferenceValue = upgradeContent;
        if (itemPrefab) so.FindProperty("upgradeItemPrefab").objectReferenceValue = itemPrefab;
        so.FindProperty("btnRepair30").objectReferenceValue    = btnR30.GetComponent<Button>();
        so.FindProperty("btnRepair70").objectReferenceValue    = btnR70.GetComponent<Button>();
        so.FindProperty("btnRepairFull").objectReferenceValue  = btnRFull.GetComponent<Button>();
        so.FindProperty("btnExpandCol").objectReferenceValue   = btnCol.GetComponent<Button>();
        so.FindProperty("btnExpandRow").objectReferenceValue   = btnRow.GetComponent<Button>();
        so.FindProperty("gridSizeText").objectReferenceValue   = gridSizeText.GetComponent<TMP_Text>();
        so.FindProperty("scrapText").objectReferenceValue      = scrapText.GetComponent<TMP_Text>();
        so.FindProperty("btnClose").objectReferenceValue       = btnClose.GetComponent<Button>();
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static void SetupResultScene()
    {
        CreateUICamera();
        CreateEventSystem();
        var canvas = CreateCanvas("ResultCanvas");
        var ctrl = new GameObject("ResultSceneController").AddComponent<ResultSceneController>();

        var bg = CreateUIImage(canvas.transform, "Background", new Color(0.07f, 0.05f, 0.03f));
        SetStretch(bg.GetComponent<RectTransform>());

        var panel = CreatePanel(canvas.transform, "ResultPanel", Vector2.zero, new Vector2(600, 400));
        CreateLabel(panel.transform, "Title", "RESULT", new Vector2(0, 150));
        var baseScrapText   = CreateLabel(panel.transform, "BaseScrapText",    "基本報酬: -- Sc",      new Vector2(0, 70));
        var pendingScrapText = CreateLabel(panel.transform, "PendingScrapText", "回収スクラップ: -- Sc", new Vector2(0, 10));
        var btnReceive = CreateButton(panel.transform, "BtnReceive", "受け取る", new Vector2(0, -80));

        var so = new SerializedObject(ctrl);
        so.FindProperty("baseScrapText").objectReferenceValue    = baseScrapText.GetComponent<TMP_Text>();
        so.FindProperty("pendingScrapText").objectReferenceValue = pendingScrapText.GetComponent<TMP_Text>();
        so.FindProperty("btnReceive").objectReferenceValue       = btnReceive.GetComponent<Button>();
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static void SetupGameOverScene()
    {
        CreateUICamera();
        CreateEventSystem();
        var canvas = CreateCanvas("GameOverCanvas");
        var ctrl = new GameObject("GameOverSceneController").AddComponent<GameOverSceneController>();

        var bg = CreateUIImage(canvas.transform, "Background", new Color(0.05f, 0.03f, 0.03f));
        SetStretch(bg.GetComponent<RectTransform>());

        var panel = CreatePanel(canvas.transform, "GameOverPanel", Vector2.zero, new Vector2(500, 300));
        CreateLabel(panel.transform, "Title", "GAME OVER", new Vector2(0, 80));
        CreateLabel(panel.transform, "Message", "画面をクリックでタイトルへ", new Vector2(0, 20));
        var btnTitle = CreateButton(panel.transform, "BtnTitle", "タイトルへ", new Vector2(0, -60));

        var so = new SerializedObject(ctrl);
        so.FindProperty("btnTitle").objectReferenceValue = btnTitle.GetComponent<Button>();
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static void PlaceholderLabel(string text)
    {
        var go = new GameObject("PlaceholderLabel");
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = text;
        t.alignment = TextAlignmentOptions.Center;
        t.fontSize = 36;
    }

    // ────────────────────────────────────────────
    // ビルド設定更新
    // ────────────────────────────────────────────
    // ────────────────────────────────────────────
    // TMP フォント一括修正
    // ────────────────────────────────────────────
    static void FixTMPFonts()
    {
        // LiberationSans SDF（特殊文字を持つデフォルト）を主フォントに統一する
        var liberationSans = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
            "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        if (liberationSans == null)
        {
            EditorUtility.DisplayDialog("エラー", "LiberationSans SDF が見つかりません。\nTMP Essentials をインポート済みか確認してください。", "OK");
            return;
        }

        int count = 0;

        // すべてのシーンを開いて TMP を修正
        foreach (var (sceneName, _) in Scenes)
        {
            var scenePath = $"{ScenesPath}/{sceneName}.unity";
            if (!File.Exists(scenePath)) continue;

            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            bool changed = false;

            foreach (var tmp in Object.FindObjectsByType<TMP_Text>(FindObjectsSortMode.None))
            {
                if (tmp.font != null && tmp.font != liberationSans)
                {
                    tmp.font = liberationSans;
                    EditorUtility.SetDirty(tmp);
                    changed = true;
                    count++;
                }
            }

            if (changed) EditorSceneManager.SaveScene(scene);
        }

        // プレハブも修正
        var prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs" });
        foreach (var guid in prefabGuids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;

            bool changed = false;
            foreach (var tmp in prefab.GetComponentsInChildren<TMP_Text>(true))
            {
                if (tmp.font != null && tmp.font != liberationSans)
                {
                    tmp.font = liberationSans;
                    EditorUtility.SetDirty(tmp);
                    changed = true;
                    count++;
                }
            }
            if (changed) AssetDatabase.SaveAssetIfDirty(prefab);
        }

        // LiberationSans SDF に NotoSansJP SDF をフォールバックとして設定
        var notoSans = AssetDatabase.FindAssets("NotoSansJP")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(p => AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(p))
            .FirstOrDefault(f => f != null);

        if (notoSans != null)
        {
            var lsSO = new SerializedObject(liberationSans);
            var fallbacks = lsSO.FindProperty("m_FallbackFontAssetTable");
            bool alreadySet = false;
            for (int i = 0; i < fallbacks.arraySize; i++)
                if (fallbacks.GetArrayElementAtIndex(i).objectReferenceValue == notoSans)
                { alreadySet = true; break; }

            if (!alreadySet)
            {
                fallbacks.arraySize++;
                fallbacks.GetArrayElementAtIndex(fallbacks.arraySize - 1).objectReferenceValue = notoSans;
                lsSO.ApplyModifiedProperties();
                EditorUtility.SetDirty(liberationSans);
            }
        }

        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("完了", $"{count} 件の TMP コンポーネントを LiberationSans SDF に変更しました。\nNotoSansJP フォールバックも設定しました。", "OK");
    }

    static void UpdateBuildSettings()
    {
        var scenes = new List<EditorBuildSettingsScene>();
        foreach (var (name, _) in Scenes)
        {
            var path = $"{ScenesPath}/{name}.unity";
            if (File.Exists(path))
                scenes.Add(new EditorBuildSettingsScene(path, true));
        }
        EditorBuildSettings.scenes = scenes.ToArray();
        Debug.Log("[GearBox] Build Settings を更新しました。");
    }

    // ────────────────────────────────────────────
    // ユーティリティ
    // ────────────────────────────────────────────
    static GameObject CreateManager<T>(string name) where T : Component
    {
        var go = new GameObject(name);
        go.AddComponent<T>();
        return go;
    }

    static void CreateUICamera()
    {
        var camGo = new GameObject("Main Camera");
        var cam = camGo.AddComponent<Camera>();
        cam.backgroundColor = Color.black;
        cam.tag = "MainCamera";
        cam.cullingMask = 0;
    }

    static Canvas CreateCanvas(string name)
    {
        var go = new GameObject(name);
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        go.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    static void CreateEventSystem()
    {
        var go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<StandaloneInputModule>();
    }

    static Image CreateUIImage(Transform parent, string name, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = color;
        return img;
    }

    static GameObject CreatePanel(Transform parent, string name, Vector2 anchoredPos, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<Image>().color = new Color(0.15f, 0.12f, 0.08f, 0.95f);
        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
        return go;
    }

    static TMP_Text CreateLabel(Transform parent, string name, string text, Vector2 anchoredPos)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = text;
        t.alignment = TextAlignmentOptions.Center;
        t.fontSize = 24;
        t.color = new Color(0.91f, 0.835f, 0.64f);
        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(300, 40);
        return t;
    }

    static GameObject CreateButton(Transform parent, string name, string label, Vector2 anchoredPos)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = new Color(0.784f, 0.475f, 0.255f);
        var btn = go.AddComponent<Button>();
        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = new Vector2(200, 50);

        var textGo = new GameObject("Text");
        textGo.transform.SetParent(go.transform, false);
        var t = textGo.AddComponent<TextMeshProUGUI>();
        t.text = label;
        t.alignment = TextAlignmentOptions.Center;
        t.fontSize = 20;
        t.color = new Color(0.91f, 0.835f, 0.64f);
        var trt = textGo.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
        trt.offsetMin = trt.offsetMax = Vector2.zero;

        return go;
    }

    static Transform CreateScrollContent(Transform parent, string name)
    {
        var sv = new GameObject("ScrollView");
        sv.transform.SetParent(parent, false);
        var svRT = sv.AddComponent<RectTransform>();
        svRT.anchorMin = Vector2.zero; svRT.anchorMax = Vector2.one;
        svRT.offsetMin = svRT.offsetMax = Vector2.zero;
        var scroll = sv.AddComponent<ScrollRect>();
        scroll.horizontal = false;

        var vp = new GameObject("Viewport");
        vp.transform.SetParent(sv.transform, false);
        var vpRT = vp.AddComponent<RectTransform>();
        vpRT.anchorMin = Vector2.zero; vpRT.anchorMax = Vector2.one;
        vpRT.offsetMin = vpRT.offsetMax = Vector2.zero;
        vp.AddComponent<Mask>().showMaskGraphic = false;
        vp.AddComponent<Image>().color = new Color(0, 0, 0, 0.01f);

        var content = new GameObject(name);
        content.transform.SetParent(vp.transform, false);
        var cRT = content.AddComponent<RectTransform>();
        cRT.anchorMin = new Vector2(0, 1); cRT.anchorMax = new Vector2(1, 1);
        cRT.pivot = new Vector2(0.5f, 1);
        cRT.offsetMin = cRT.offsetMax = Vector2.zero;
        var vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 6; vlg.padding = new RectOffset(8, 8, 8, 8);
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childControlWidth = true; vlg.childForceExpandWidth = true;
        content.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        scroll.viewport = vpRT;
        scroll.content  = cRT;

        return content.transform;
    }

    static void SetStretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }
}
