using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapSceneController : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] MapGraphView graphView;
    [SerializeField] RectTransform mapScrollContainer;
    [SerializeField] Camera mapCamera;

    [Header("HUD")]
    [SerializeField] TMP_Text areaLabel;
    [SerializeField] TMP_Text scrapText;

    [Header("メニュー")]
    [SerializeField] GameObject menuOverlay;
    [SerializeField] Button btnMenu;
    [SerializeField] Button btnMenuSettings;
    [SerializeField] Button btnGiveUp;
    [SerializeField] Button btnSuspend;
    [SerializeField] GameObject giveUpConfirm;
    [SerializeField] Button btnGiveUpYes;
    [SerializeField] Button btnGiveUpNo;

    [Header("設定")]
    [SerializeField] GameObject settingsPanel;

    [Header("マップ導入アニメーション")]
    [SerializeField] float introScrollDuration = 2f;

    bool interactable;
    bool isNewArea;

    void Start()
    {
        var rm = RunManager.Instance;

        if (rm.CurrentMapGraph == null)
        {
            var graph = MapGenerator.Generate();
            rm.StartNewArea(graph);
            isNewArea = true;
        }

        graphView.OnNodeSelected = OnNodeSelected;
        graphView.Build(rm.CurrentMapGraph);

        UpdateHUD();
        BindMenu();

        if (isNewArea)
            StartCoroutine(IntroAnimation());
        else
        {
            interactable = true;
            SceneTransitionManager.Instance.FadeIn(0.4f);
        }
    }

    void UpdateHUD()
    {
        if (scrapText) scrapText.text = $"{RunManager.Instance.Scrap} ⚙";
        if (areaLabel) areaLabel.text = "エリア 1"; // TODO: エリア番号管理
    }

    IEnumerator IntroAnimation()
    {
        interactable = false;

        // マップ最下部から最上部へスクロール
        var startY = -mapScrollContainer.rect.height;
        var endY = mapScrollContainer.rect.height;
        float elapsed = 0f;
        while (elapsed < introScrollDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / introScrollDuration);
            mapScrollContainer.anchoredPosition = new Vector2(0f, Mathf.Lerp(startY, endY, t));
            yield return null;
        }

        // スタートノードにフォーカス
        yield return new WaitForSeconds(0.3f);
        mapScrollContainer.anchoredPosition = Vector2.zero;

        interactable = true;
        SceneTransitionManager.Instance.FadeIn(0.4f);
    }

    void OnNodeSelected(MapNode node)
    {
        if (!interactable) return;
        interactable = false;

        RunManager.Instance.CompleteNode(node.id);

        switch (node.type)
        {
            case NodeType.Battle:
            case NodeType.Boss:
                SceneTransitionManager.Instance.TransitionTo("PreparationScene");
                break;
            case NodeType.Shop:
                SceneTransitionManager.Instance.TransitionTo("ShopScene");
                break;
            case NodeType.Refit:
                SceneTransitionManager.Instance.TransitionTo("RefitScene");
                break;
            case NodeType.Mystery:
                // TODO: ハテナオーバーレイ
                interactable = true;
                break;
        }
    }

    void BindMenu()
    {
        btnMenu?.onClick.AddListener(() => menuOverlay?.SetActive(true));
        btnMenuSettings?.onClick.AddListener(() => { menuOverlay?.SetActive(false); settingsPanel?.SetActive(true); });
        btnGiveUp?.onClick.AddListener(() => giveUpConfirm?.SetActive(true));
        btnGiveUpYes?.onClick.AddListener(OnGiveUp);
        btnGiveUpNo?.onClick.AddListener(() => giveUpConfirm?.SetActive(false));
        btnSuspend?.onClick.AddListener(OnSuspend);
    }

    void OnGiveUp()
    {
        menuOverlay?.SetActive(false);
        RunManager.Instance.ResetRun();
        SceneTransitionManager.Instance.TransitionTo("GameOverScene");
    }

    void OnSuspend()
    {
        SaveManager.Instance.SaveRun(new RunSaveData
        {
            currentHp = RunManager.Instance.CurrentHp,
            maxHp = RunManager.Instance.MaxHp,
            scrap = RunManager.Instance.Scrap,
            resumeSceneName = "MapScene",
        });
        SceneTransitionManager.Instance.TransitionTo("TitleScene");
    }
}
