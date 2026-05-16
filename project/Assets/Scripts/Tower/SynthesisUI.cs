using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// 準備フェーズ・改修・ショップで共用できる合成UIパネル
public class SynthesisUI : MonoBehaviour
{
    [Header("素材選択")]
    [SerializeField] Transform materialListRoot;
    [SerializeField] SynthesisMaterialCard cardPrefab;

    [Header("プレビュー")]
    [SerializeField] TMP_Text resultNameText;
    [SerializeField] TMP_Text resultDescText;
    [SerializeField] Button btnSynthesize;

    [Header("レシピ一覧")]
    [SerializeField] Transform recipeListRoot;
    [SerializeField] SynthesisMaterialCard recipePrefab;

    TowerInstance selectedA;
    TowerInstance selectedB;
    readonly List<SynthesisMaterialCard> cards = new();

    public System.Action OnSynthesized;

    void OnEnable() => Refresh();

    public void Refresh()
    {
        selectedA = selectedB = null;
        BuildMaterialList();
        BuildRecipeList();
        UpdatePreview();
    }

    void BuildMaterialList()
    {
        foreach (Transform c in materialListRoot) Destroy(c.gameObject);
        cards.Clear();

        // 手持ち
        foreach (var t in RunManager.Instance.TowerInventory)
            AddCard(t);

        // グリッド配置済み
        var layout = RunManager.Instance.GridLayout;
        var size   = RunManager.Instance.GridSize;
        for (int x = 0; x < size.x; x++)
            for (int y = 0; y < size.y; y++)
                if (layout[x, y] != null)
                    AddCard(layout[x, y]);
    }

    void AddCard(TowerInstance tower)
    {
        var card = Instantiate(cardPrefab, materialListRoot);
        var t = tower;
        card.Setup(tower, false, () => OnCardClicked(t));
        cards.Add(card);
    }

    void OnCardClicked(TowerInstance tower)
    {
        if (selectedA == tower) { selectedA = null; }
        else if (selectedB == tower) { selectedB = null; }
        else if (selectedA == null) selectedA = tower;
        else if (selectedB == null) selectedB = tower;
        else { selectedA = selectedB; selectedB = tower; }

        foreach (var c in cards)
            c.SetSelected(c.Tower == selectedA || c.Tower == selectedB);

        UpdatePreview();
    }

    void UpdatePreview()
    {
        if (selectedA == null || selectedB == null)
        {
            if (resultNameText) resultNameText.text = "素材を2つ選択してください";
            if (resultDescText) resultDescText.text = "";
            if (btnSynthesize) btnSynthesize.interactable = false;
            return;
        }

        var recipe = SynthesisSystem.FindRecipe(
            selectedA.data, selectedB.data,
            RunManager.Instance.KnownRecipes);

        if (recipe != null)
        {
            if (resultNameText) resultNameText.text = recipe.result.displayName;
            if (resultDescText) resultDescText.text = recipe.result.description;
            if (btnSynthesize) btnSynthesize.interactable = true;
        }
        else
        {
            if (resultNameText) resultNameText.text = "合成できません";
            if (resultDescText) resultDescText.text = "";
            if (btnSynthesize) btnSynthesize.interactable = false;
        }
    }

    public void OnClickSynthesize()
    {
        if (selectedA == null || selectedB == null) return;
        var recipe = SynthesisSystem.FindRecipe(
            selectedA.data, selectedB.data,
            RunManager.Instance.KnownRecipes);
        if (recipe == null) return;

        SynthesisSystem.Execute(selectedA, selectedB, recipe);
        OnSynthesized?.Invoke();
        Refresh();
    }

    void BuildRecipeList()
    {
        if (recipeListRoot == null) return;
        foreach (Transform c in recipeListRoot) Destroy(c.gameObject);
        foreach (var r in RunManager.Instance.KnownRecipes)
        {
            var card = Instantiate(recipePrefab, recipeListRoot);
            card.Setup(null, false, null);
            card.SetLabel($"{r.materialA.displayName} + {r.materialB.displayName} → {r.result.displayName}");
        }
    }
}
