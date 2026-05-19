#if UNITY_EDITOR || DEVELOPMENT_BUILD
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 全シーン共通のデバッグメニュー。エディタ・開発ビルドのみ有効。
/// RuntimeInitializeOnLoadMethod で自動生成される。
/// </summary>
public class DebugMenuController : MonoBehaviour
{
    static DebugMenuController instance;

    Canvas   debugCanvas;
    GameObject menuPanel;
    TMP_Text statusText;

    // ────────────────────────────────────────────
    // 自動生成
    // ────────────────────────────────────────────
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoSpawn()
    {
        if (instance != null) return;
        var go = new GameObject("[DebugMenu]");
        instance = go.AddComponent<DebugMenuController>();
        DontDestroyOnLoad(go);
    }

    void Awake() => BuildUI();

    // ────────────────────────────────────────────
    // UI 構築
    // ────────────────────────────────────────────
    void BuildUI()
    {
        // Canvas（最前面）
        var canvasGo = new GameObject("DebugCanvas");
        canvasGo.transform.SetParent(transform);
        debugCanvas = canvasGo.AddComponent<Canvas>();
        debugCanvas.renderMode  = RenderMode.ScreenSpaceOverlay;
        debugCanvas.sortingOrder = 9999;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGo.AddComponent<GraphicRaycaster>();

        // EventSystem が存在しない場合だけ追加
        if (FindFirstObjectByType<EventSystem>() == null)
        {
            var esGo = new GameObject("DebugEventSystem");
            esGo.transform.SetParent(transform);
            esGo.AddComponent<EventSystem>();
            esGo.AddComponent<StandaloneInputModule>();
        }

        BuildTriggerButton(canvasGo.transform);
        BuildMenuPanel(canvasGo.transform);
        menuPanel.SetActive(false);
    }

    // 右上の小さいトリガーボタン（ほぼ透明）
    void BuildTriggerButton(Transform parent)
    {
        var go = new GameObject("DebugTrigger");
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = new Color(1f, 0f, 0f, 0.15f); // 開発中は薄く見える
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot     = new Vector2(1f, 1f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(60, 60);
        var btn = go.AddComponent<Button>();
        btn.onClick.AddListener(() => menuPanel?.SetActive(true));
    }

    // デバッグメニューパネル
    void BuildMenuPanel(Transform parent)
    {
        // 全画面ブロッカー
        menuPanel = new GameObject("DebugMenuRoot");
        menuPanel.transform.SetParent(parent, false);
        var rootRT = menuPanel.AddComponent<RectTransform>();
        rootRT.anchorMin = Vector2.zero; rootRT.anchorMax = Vector2.one;
        rootRT.offsetMin = rootRT.offsetMax = Vector2.zero;

        // ブロッカー（背景タップで閉じる）
        var blocker = new GameObject("Blocker");
        blocker.transform.SetParent(menuPanel.transform, false);
        var blockerImg = blocker.AddComponent<Image>();
        blockerImg.color = new Color(0f, 0f, 0f, 0.6f);
        var blockerRT = blocker.GetComponent<RectTransform>();
        blockerRT.anchorMin = Vector2.zero; blockerRT.anchorMax = Vector2.one;
        blockerRT.offsetMin = blockerRT.offsetMax = Vector2.zero;
        var blockerBtn = blocker.AddComponent<Button>();
        blockerBtn.transition = Selectable.Transition.None;
        blockerBtn.onClick.AddListener(() => menuPanel.SetActive(false));

        // パネル本体（右側に配置）
        var panel = new GameObject("Panel");
        panel.transform.SetParent(menuPanel.transform, false);
        var panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
        var panelRT = panel.GetComponent<RectTransform>();
        panelRT.anchorMin = panelRT.anchorMax = new Vector2(1f, 1f);
        panelRT.pivot     = new Vector2(1f, 1f);
        panelRT.anchoredPosition = new Vector2(-10f, -10f);
        panelRT.sizeDelta = new Vector2(300f, 400f);

        // タイトル
        AddLabel(panel.transform, "■ Debug Menu", new Vector2(0, -20), 18);

        // ステータス（ログ件数など）
        statusText = AddLabel(panel.transform, "", new Vector2(0, -55), 13);

        // 区切り線
        AddSeparator(panel.transform, -80);

        // ボタン群
        AddButton(panel.transform, "ログをファイルに出力", new Vector2(0, -120), OnDumpLog);
        AddButton(panel.transform, "ログをクリア",         new Vector2(0, -180), OnClearLog);

        // 閉じるボタン
        AddButton(panel.transform, "閉じる", new Vector2(0, -360), () => menuPanel.SetActive(false));
    }

    // ────────────────────────────────────────────
    // ボタンイベント
    // ────────────────────────────────────────────
    void OnDumpLog()
    {
        GameLogger.DumpToFile();
        UpdateStatus();
    }

    void OnClearLog()
    {
        // GameLogger に Clear メソッドを追加して呼ぶ
        GameLogger.Clear();
        UpdateStatus();
    }

    void OnEnable() => UpdateStatus();

    void UpdateStatus()
    {
        if (statusText)
            statusText.text = $"プールログ: {GameLogger.EntryCount} 件";
    }

    // ────────────────────────────────────────────
    // UI ヘルパー
    // ────────────────────────────────────────────
    TMP_Text AddLabel(Transform parent, string text, Vector2 pos, float fontSize = 20)
    {
        var go = new GameObject("Label");
        go.transform.SetParent(parent, false);
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text      = text;
        t.fontSize  = fontSize;
        t.color     = new Color(0.9f, 0.9f, 0.9f);
        t.alignment = TextAlignmentOptions.Center;
        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(280, 30);
        return t;
    }

    void AddSeparator(Transform parent, float posY)
    {
        var go = new GameObject("Sep");
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = new Color(0.4f, 0.4f, 0.4f);
        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, posY);
        rt.sizeDelta = new Vector2(280, 1);
    }

    void AddButton(Transform parent, string label, Vector2 pos, UnityEngine.Events.UnityAction action)
    {
        var go = new GameObject(label);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = new Color(0.25f, 0.25f, 0.25f);
        var btn = go.AddComponent<Button>();
        btn.onClick.AddListener(action);
        var rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta = new Vector2(260, 44);

        var textGo = new GameObject("Text");
        textGo.transform.SetParent(go.transform, false);
        var t = textGo.AddComponent<TextMeshProUGUI>();
        t.text      = label;
        t.fontSize  = 20;
        t.color     = new Color(0.9f, 0.9f, 0.9f);
        t.alignment = TextAlignmentOptions.Center;
        var trt = textGo.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
        trt.offsetMin = trt.offsetMax = Vector2.zero;
    }
}
#endif
