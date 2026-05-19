using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RefitSceneController : MonoBehaviour
{
    [Header("タブ")]
    [SerializeField] Button btnTabUpgrade;
    [SerializeField] Button btnTabRepair;
    [SerializeField] Button btnTabExpand;
    [SerializeField] GameObject upgradePanel;
    [SerializeField] GameObject repairPanel;
    [SerializeField] GameObject expandPanel;

    [Header("強化")]
    [SerializeField] Transform upgradeListRoot;
    [SerializeField] ShopItemUI upgradeItemPrefab;

    [Header("修理")]
    [SerializeField] Button btnRepair30;
    [SerializeField] Button btnRepair70;
    [SerializeField] Button btnRepairFull;

    [Header("拡張")]
    [SerializeField] Button btnExpandCol;
    [SerializeField] Button btnExpandRow;
    [SerializeField] TMP_Text gridSizeText;

    [Header("HUD")]
    [SerializeField] TMP_Text scrapText;
    [SerializeField] Button btnClose;

    System.Action<int>      onScrapChanged;
    System.Action<int, int> onHpChanged;

    void Start()
    {
        SceneTransitionManager.Instance?.FadeIn(0.4f);
        UpdateHUD();
        onScrapChanged = _ => { UpdateHUD(); RefreshExpand(); };
        onHpChanged    = (_, __) => RefreshRepair();
        RunManager.Instance.OnScrapChanged += onScrapChanged;
        RunManager.Instance.OnHpChanged    += onHpChanged;

        btnTabUpgrade?.onClick.AddListener(() => ShowPanel(upgradePanel));
        btnTabRepair?.onClick.AddListener(()  => ShowPanel(repairPanel));
        btnTabExpand?.onClick.AddListener(()  => ShowPanel(expandPanel));
        btnClose?.onClick.AddListener(() => SceneTransitionManager.Instance.TransitionTo("MapScene"));

        BindRepair();
        BindExpand();
        RefreshUpgradeList();
        RefreshRepair();
        RefreshExpand();
        ShowPanel(upgradePanel);
    }

    void OnDestroy()
    {
        if (RunManager.Instance == null) return;
        RunManager.Instance.OnScrapChanged -= onScrapChanged;
        RunManager.Instance.OnHpChanged    -= onHpChanged;
    }

    void ShowPanel(GameObject target)
    {
        upgradePanel?.SetActive(upgradePanel == target);
        repairPanel?.SetActive(repairPanel   == target);
        expandPanel?.SetActive(expandPanel   == target);
    }

    // ── 強化 ──────────────────────────────────
    void RefreshUpgradeList()
    {
        if (upgradeListRoot == null) return;
        foreach (Transform c in upgradeListRoot) Destroy(c.gameObject);
        foreach (var tower in RunManager.Instance.TowerInventory)
        {
            if (upgradeItemPrefab == null) continue;
            var t = tower;
            int cost = t.level == 1 ? 30 : t.level == 2 ? 50 : 0;
            bool maxed = t.level >= 3;
            var ui = Instantiate(upgradeItemPrefab, upgradeListRoot);
            ui.Setup($"{t.data.displayName} Lv{t.level}", cost, maxed, () =>
            {
                if (maxed || !RunManager.Instance.SpendScrap(cost)) return;
                t.level++;
                RefreshUpgradeList();
            });
        }
    }

    // ── 修理 ──────────────────────────────────
    void BindRepair()
    {
        btnRepair30?.onClick.AddListener(()   => TryRepair(0.3f, 30));
        btnRepair70?.onClick.AddListener(()   => TryRepair(0.7f, 60));
        btnRepairFull?.onClick.AddListener(() => TryRepair(1.0f, 100));
    }

    void TryRepair(float ratio, int cost)
    {
        if (!RunManager.Instance.SpendScrap(cost)) return;
        int heal = Mathf.RoundToInt(RunManager.Instance.MaxHp * ratio);
        RunManager.Instance.Heal(heal);
        RefreshRepair();
    }

    void RefreshRepair()
    {
        bool full = RunManager.Instance.CurrentHp >= RunManager.Instance.MaxHp;
        btnRepair30?.GetComponent<Button>().SetInteractable(!full && RunManager.Instance.Scrap >= 30);
        btnRepair70?.GetComponent<Button>().SetInteractable(!full && RunManager.Instance.Scrap >= 60);
        btnRepairFull?.GetComponent<Button>().SetInteractable(!full && RunManager.Instance.Scrap >= 100);
    }

    // ── 拡張 ──────────────────────────────────
    void BindExpand()
    {
        btnExpandCol?.onClick.AddListener(() => TryExpand(true));
        btnExpandRow?.onClick.AddListener(() => TryExpand(false));
    }

    void TryExpand(bool col)
    {
        if (!RunManager.Instance.SpendScrap(80)) return;
        RunManager.Instance.ExpandGrid(col);
        RefreshExpand();
    }

    void RefreshExpand()
    {
        var size = RunManager.Instance.GridSize;
        if (gridSizeText) gridSizeText.text = $"現在のグリッド: {size.x} × {size.y}";
        btnExpandCol?.GetComponent<Button>().SetInteractable(size.x < 5 && RunManager.Instance.Scrap >= 80);
        btnExpandRow?.GetComponent<Button>().SetInteractable(size.y < 5 && RunManager.Instance.Scrap >= 80);
    }

    void UpdateHUD()
    {
        if (scrapText) scrapText.text = $"{RunManager.Instance.Scrap} Sc";
    }
}

// Button 拡張
public static class ButtonExt
{
    public static void SetInteractable(this Button btn, bool v) { if (btn) btn.interactable = v; }
}
