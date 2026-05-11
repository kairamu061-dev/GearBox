using System.Collections.Generic;
using System.IO;
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
        EditorGUILayout.HelpBox("各ボタンでシーンを自動生成します。既存シーンは上書きされます。", MessageType.Info);
        EditorGUILayout.Space(8);

        if (GUILayout.Button("★ 全シーンをセットアップ + ビルド設定更新", GUILayout.Height(36)))
            SetupAll();

        EditorGUILayout.Space(8);
        GUILayout.Label("個別セットアップ", EditorStyles.boldLabel);
        foreach (var (name, setup) in Scenes)
            if (GUILayout.Button(name)) RunSetup(name, setup);
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
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    // ────────────────────────────────────────────
    // MapScene
    // ────────────────────────────────────────────
    static void SetupMapScene()
    {
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
        var graphViewSO = new SerializedObject(graphView);
        graphViewSO.FindProperty("mapContainer").objectReferenceValue = mapContainerRT;
        graphViewSO.ApplyModifiedPropertiesWithoutUndo();

        // HUD（右上）
        var hud = CreatePanel(canvas.transform, "MapHUD", new Vector2(0, 0), new Vector2(300, 60));
        var hudRT = hud.GetComponent<RectTransform>();
        hudRT.anchorMin = hudRT.anchorMax = new Vector2(1, 1);
        hudRT.pivot = new Vector2(1, 1);
        hudRT.anchoredPosition = new Vector2(-10, -10);
        var areaLabel = CreateLabel(hud.transform, "AreaLabel", "エリア 1", new Vector2(-80, 0));
        var scrapText = CreateLabel(hud.transform, "ScrapText", "100 ⚙",   new Vector2(60, 0));

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
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    // ────────────────────────────────────────────
    // 残りのシーン（プレースホルダ）
    // ────────────────────────────────────────────
    static void SetupPreparationScene()
    {
        CreateEventSystem();
        CreateCanvas("PreparationCanvas");
        var label = new GameObject("Label").AddComponent<TMP_Text>();
        label.text = "PreparationScene - 実装中";
    }

    static void SetupBattleScene()
    {
        var cam = new GameObject("Main Camera").AddComponent<Camera>();
        cam.backgroundColor = new Color(0.1f, 0.1f, 0.1f);
        cam.tag = "MainCamera";
        CreateEventSystem();
        CreateCanvas("BattleCanvas");
    }

    static void SetupShopScene()
    {
        CreateEventSystem();
        CreateCanvas("ShopCanvas");
        PlaceholderLabel("ShopScene - 実装中");
    }

    static void SetupRefitScene()
    {
        CreateEventSystem();
        CreateCanvas("RefitCanvas");
        PlaceholderLabel("RefitScene - 実装中");
    }

    static void SetupResultScene()
    {
        CreateEventSystem();
        CreateCanvas("ResultCanvas");
        PlaceholderLabel("ResultScene - 実装中");
    }

    static void SetupGameOverScene()
    {
        CreateEventSystem();
        CreateCanvas("GameOverCanvas");
        PlaceholderLabel("GameOverScene - 実装中");
    }

    static void PlaceholderLabel(string text)
    {
        var go = new GameObject("PlaceholderLabel");
        var t = go.AddComponent<TMP_Text>();
        t.text = text;
        t.alignment = TextAlignmentOptions.Center;
        t.fontSize = 36;
    }

    // ────────────────────────────────────────────
    // ビルド設定更新
    // ────────────────────────────────────────────
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
        var t = go.AddComponent<TMP_Text>();
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
        var t = textGo.AddComponent<TMP_Text>();
        t.text = label;
        t.alignment = TextAlignmentOptions.Center;
        t.fontSize = 20;
        t.color = new Color(0.91f, 0.835f, 0.64f);
        var trt = textGo.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
        trt.offsetMin = trt.offsetMax = Vector2.zero;

        return go;
    }

    static void SetStretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }
}
