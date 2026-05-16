using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultSceneController : MonoBehaviour
{
    [Header("基本報酬")]
    [SerializeField] TMP_Text baseScrapText;

    [Header("ドロップ選択")]
    [SerializeField] Transform dropListRoot;
    [SerializeField] ResultDropItemUI dropItemPrefab;
    [SerializeField] TMP_Text pendingScrapText;

    [Header("ボタン")]
    [SerializeField] Button btnReceive;

    readonly List<ResultDropItem> drops = new();

    void Start()
    {
        SceneTransitionManager.Instance?.FadeIn(0.4f);

        int baseScrap = CalcBaseReward();
        if (baseScrapText) baseScrapText.text = $"基本報酬: {baseScrap} Sc";
        RunManager.Instance.AddScrap(baseScrap);

        BuildDropList();
        UpdatePendingText();
        btnReceive?.onClick.AddListener(OnReceive);
    }

    int CalcBaseReward()
    {
        var node = RunManager.Instance.CurrentMapGraph?
            .GetNode(RunManager.Instance.CurrentNodeId);
        return node?.type == NodeType.Boss ? 100 : 50;
    }

    void BuildDropList()
    {
        if (dropListRoot == null || dropItemPrefab == null) return;
        foreach (Transform c in dropListRoot) Destroy(c.gameObject);
        drops.Clear();

        // バトル中の仮保持スクラップ
        int pendingScrap = RunManager.Instance.PendingScrap;
        if (pendingScrap > 0)
        {
            var item = new ResultDropItem
            {
                type = DropType.Scrap,
                scrapAmount = pendingScrap,
                selected = true,
            };
            drops.Add(item);
            var ui = Instantiate(dropItemPrefab, dropListRoot);
            ui.Setup(item, $"スクラップ {pendingScrap} Sc", autoSelect: true);
        }

        // TODO: タワー・強化パーツのドロップ（将来FieldGeneratorから受け取る）
    }

    void UpdatePendingText()
    {
        if (pendingScrapText)
            pendingScrapText.text = $"回収スクラップ: {RunManager.Instance.PendingScrap} Sc";
    }

    void OnReceive()
    {
        foreach (var d in drops)
        {
            if (!d.selected) continue;
            switch (d.type)
            {
                case DropType.Scrap:
                    RunManager.Instance.CommitPendingScrap();
                    break;
                case DropType.Tower when d.tower != null:
                    RunManager.Instance.AddTower(d.tower);
                    break;
                case DropType.Relic when d.relic != null:
                    RunManager.Instance.AddRelic(d.relic);
                    break;
            }
        }
        SceneTransitionManager.Instance.TransitionTo("MapScene");
    }
}

public enum DropType { Scrap, Tower, Relic }

public class ResultDropItem
{
    public DropType type;
    public int scrapAmount;
    public TowerInstance tower;
    public RelicData relic;
    public bool selected;
}
