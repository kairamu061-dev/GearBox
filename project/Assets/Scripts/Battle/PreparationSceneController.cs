using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PreparationSceneController : MonoBehaviour
{
    [Header("グリッド")]
    [SerializeField] GridUI gridUI;

    [Header("タワー一覧")]
    [SerializeField] Transform towerListRoot;
    [SerializeField] TowerCardUI towerCardPrefab;

    [Header("ゴーストUI")]
    [SerializeField] Image ghostImage;

    [Header("ボタン")]
    [SerializeField] Button btnSortie;

    [Header("HUD")]
    [SerializeField] TMP_Text scrapText;

    // 選択状態
    TowerInstance selectedTower;
    List<TowerCardUI> cards = new();

    void Start()
    {
        SceneTransitionManager.Instance?.FadeIn(0.4f);
        gridUI.Build(RunManager.Instance.GridSize);
        gridUI.OnCellClicked = OnCellClicked;
        gridUI.OnCellHovered = OnCellHovered;

        RefreshTowerList();
        UpdateHUD();

        if (ghostImage) ghostImage.gameObject.SetActive(false);
        btnSortie?.onClick.AddListener(OnSortie);
        RunManager.Instance.OnScrapChanged += _ => UpdateHUD();
    }

    void OnDestroy() => RunManager.Instance.OnScrapChanged -= _ => UpdateHUD();

    void Update()
    {
        if (selectedTower == null) return;

        // ゴーストをマウス位置に追従
        if (ghostImage)
        {
            ghostImage.transform.position = Input.mousePosition;
            var cell = gridUI.GetCellUnderMouse();
            gridUI.ClearHighlights();
            if (cell.HasValue)
                gridUI.HighlightCell(cell.Value, gridUI.IsCellEmpty(cell.Value));
        }

        // 右クリックでキャンセル
        if (Input.GetMouseButtonDown(1)) CancelSelection();
    }

    void RefreshTowerList()
    {
        foreach (Transform c in towerListRoot) Destroy(c.gameObject);
        cards.Clear();

        foreach (var tower in RunManager.Instance.TowerInventory)
        {
            var card = Instantiate(towerCardPrefab, towerListRoot);
            card.Setup(tower);
            card.OnSelected   += SelectTower;
            card.OnHoverEnter += OnCardHoverEnter;
            card.OnHoverExit  += OnCardHoverExit;
            cards.Add(card);
        }
    }

    void SelectTower(TowerInstance tower)
    {
        selectedTower = tower;
        cards.ForEach(c => c.SetSelected(c.Tower == tower));

        if (ghostImage && tower.data?.icon != null)
        {
            ghostImage.sprite = tower.data.icon;
            var c = ghostImage.color; c.a = 0.6f; ghostImage.color = c;
            ghostImage.gameObject.SetActive(true);
        }
    }

    void CancelSelection()
    {
        selectedTower = null;
        cards.ForEach(c => c.SetSelected(false));
        gridUI.ClearHighlights();
        if (ghostImage) ghostImage.gameObject.SetActive(false);
    }

    void OnCellClicked(Vector2Int pos)
    {
        if (selectedTower == null) return;
        if (!gridUI.IsCellEmpty(pos)) return;

        RunManager.Instance.PlaceTower(selectedTower, pos);
        gridUI.Refresh();
        RefreshTowerList();
        CancelSelection();
    }

    void OnCellHovered(Vector2Int pos)
    {
        if (selectedTower == null) return;
        gridUI.HighlightCell(pos, gridUI.IsCellEmpty(pos));
    }

    void OnCardHoverEnter(TowerInstance tower) =>
        TowerParamPreview.Instance?.Show(tower);

    void OnCardHoverExit(TowerInstance tower) =>
        TowerParamPreview.Instance?.Hide();

    void UpdateHUD()
    {
        if (scrapText) scrapText.text = $"{RunManager.Instance.Scrap} Sc";
    }

    void OnSortie() => SceneTransitionManager.Instance.TransitionTo("BattleScene");
}
