using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSceneController : MonoBehaviour
{
    [Header("在庫")]
    [SerializeField] Transform itemListRoot;
    [SerializeField] ShopItemUI itemPrefab;

    [Header("売却・合成タブ")]
    [SerializeField] GameObject sellPanel;
    [SerializeField] Transform sellListRoot;
    [SerializeField] ShopItemUI sellCardPrefab;

    [Header("HUD")]
    [SerializeField] TMP_Text scrapText;

    [Header("ボタン")]
    [SerializeField] Button btnClose;
    [SerializeField] Button btnTabBuy;
    [SerializeField] Button btnTabSell;

    [SerializeField] TowerData[] allTowers;

    List<ShopItem> stock = new();

    void Start()
    {
        SceneTransitionManager.Instance?.FadeIn(0.4f);
        GenerateStock();
        RefreshBuyList();
        RefreshSellList();
        UpdateHUD();
        RunManager.Instance.OnScrapChanged += _ => UpdateHUD();

        btnClose?.onClick.AddListener(() => SceneTransitionManager.Instance.TransitionTo("MapScene"));
        btnTabBuy?.onClick.AddListener(()  => sellPanel?.SetActive(false));
        btnTabSell?.onClick.AddListener(() => sellPanel?.SetActive(true));
    }

    void OnDestroy() => RunManager.Instance.OnScrapChanged -= _ => UpdateHUD();

    void GenerateStock()
    {
        stock.Clear();
        if (allTowers == null || allTowers.Length == 0) return;
        var pool = new List<TowerData>(allTowers);
        int count = Mathf.Min(Random.Range(3, 6), pool.Count);
        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, pool.Count);
            stock.Add(new ShopItem { tower = pool[idx], sold = false });
            pool.RemoveAt(idx);
        }
    }

    void RefreshBuyList()
    {
        foreach (Transform c in itemListRoot) Destroy(c.gameObject);
        foreach (var item in stock)
        {
            var ui = Instantiate(itemPrefab, itemListRoot);
            var captured = item;
            ui.Setup(item.tower.displayName, item.tower.basePrice, item.sold, () =>
            {
                if (captured.sold) return;
                if (!RunManager.Instance.SpendScrap(captured.tower.basePrice)) return;
                captured.sold = true;
                RunManager.Instance.AddTower(new TowerInstance(captured.tower));
                RefreshBuyList();
            });
        }
    }

    void RefreshSellList()
    {
        if (sellListRoot == null) return;
        foreach (Transform c in sellListRoot) Destroy(c.gameObject);
        foreach (var tower in RunManager.Instance.TowerInventory)
        {
            var t = tower;
            var ui = Instantiate(sellCardPrefab, sellListRoot);
            int sellPrice = t.data.basePrice / 2;
            ui.Setup(t.data.displayName, sellPrice, false, () =>
            {
                RunManager.Instance.RemoveTower(t);
                RunManager.Instance.AddScrap(sellPrice);
                RefreshSellList();
            });
        }
    }

    void UpdateHUD()
    {
        if (scrapText) scrapText.text = $"{RunManager.Instance.Scrap} ⚙";
    }

    class ShopItem { public TowerData tower; public bool sold; }
}
