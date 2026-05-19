using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultSceneController : MonoBehaviour
{
    [Header("基本報酬")]
    [SerializeField] TMP_Text baseScrapText;

    [Header("タワー3択")]
    [SerializeField] Transform towerChoiceRoot;
    [SerializeField] ResultTowerChoiceUI towerChoicePrefab;
    [SerializeField] TMP_Text towerChoiceLabel;

    [Header("ドロップ選択")]
    [SerializeField] Transform dropListRoot;
    [SerializeField] ResultDropItemUI dropItemPrefab;

    [Header("ボタン")]
    [SerializeField] Button btnReceive;

    readonly List<ResultDropItem> drops = new();
    ResultTowerChoiceUI selectedTowerCard;
    TowerData selectedTower;

    void Start()
    {
        SceneTransitionManager.Instance?.FadeIn(0.4f);

        int baseScrap = CalcBaseReward();
        if (baseScrapText) baseScrapText.text = $"基本報酬: {baseScrap} Sc";
        RunManager.Instance.AddScrap(baseScrap);

        BuildTowerChoices();
        BuildDropList();
        btnReceive?.onClick.AddListener(OnReceive);
    }

    int CalcBaseReward()
    {
        var node = RunManager.Instance.CurrentMapGraph?
            .GetNode(RunManager.Instance.CurrentNodeId);
        return node?.type == NodeType.Boss ? 100 : 50;
    }

    // ── タワー3択 ──────────────────────────────
    void BuildTowerChoices()
    {
        if (towerChoiceRoot == null || towerChoicePrefab == null) return;

        var allTowers = Resources.LoadAll<TowerData>("Towers");
        if (allTowers.Length == 0) return;

        // ランダムに3種を抽選（重複なし）
        var pool = allTowers.OrderBy(_ => Random.value).Take(3).ToArray();

        if (towerChoiceLabel)
            towerChoiceLabel.text = "タワーを1つ選んで取得（スキップ可）";

        foreach (var td in pool)
        {
            var card = Instantiate(towerChoicePrefab, towerChoiceRoot);
            var captured = td;
            card.Setup(td, () => OnTowerSelected(card, captured));
        }
    }

    void OnTowerSelected(ResultTowerChoiceUI card, TowerData tower)
    {
        // 同じカードをもう一度押したら選択解除
        if (selectedTowerCard == card)
        {
            selectedTowerCard.SetSelected(false);
            selectedTowerCard = null;
            selectedTower = null;
            return;
        }
        selectedTowerCard?.SetSelected(false);
        selectedTowerCard = card;
        selectedTowerCard.SetSelected(true);
        selectedTower = tower;
    }

    // ── ドロップ品 ─────────────────────────────
    void BuildDropList()
    {
        if (dropListRoot == null || dropItemPrefab == null) return;
        foreach (Transform c in dropListRoot) Destroy(c.gameObject);
        drops.Clear();

        int pendingScrap = RunManager.Instance.PendingScrap;
        if (pendingScrap > 0)
        {
            var item = new ResultDropItem
                { type = DropType.Scrap, scrapAmount = pendingScrap, selected = true };
            drops.Add(item);
            var ui = Instantiate(dropItemPrefab, dropListRoot);
            ui.Setup(item, $"回収スクラップ: {pendingScrap} Sc", autoSelect: true);
        }
    }

    // ── 確定 ──────────────────────────────────
    void OnReceive()
    {
        // 選んだタワーを追加
        if (selectedTower != null)
            RunManager.Instance.AddTower(new TowerInstance(selectedTower));

        // ドロップ品を反映
        foreach (var d in drops)
        {
            if (!d.selected) continue;
            if (d.type == DropType.Scrap) RunManager.Instance.CommitPendingScrap();
        }

        SceneTransitionManager.Instance.TransitionTo("MapScene");
    }
}
