# ショップ 設計

## Unityパッケージ / アセット

| パッケージ / アセット | 用途 | 選定理由 |
|----------------------|------|----------|
| DOTween | 購入成功アニメーション | 軽量 |
| TextMeshPro | 価格・スクラップ数 | 標準 |

## シーン・オブジェクト構成

```
ShopScene
  ├── [System]
  │     └── ShopSceneController (MonoBehaviour)
  ├── [Header]
  │     ├── ScrapText (TMP_Text)
  │     └── CloseButton (Button)
  └── [Inventory]
        └── ShopItemList (ScrollRect)
              └── ShopItemUI × n (Prefab)
                    ├── ItemIcon (Image)
                    ├── ItemNameText (TMP_Text)
                    ├── ItemDescText (TMP_Text)
                    ├── PriceText (TMP_Text)
                    └── BuyButton (Button)
```

## クラス設計

| クラス名 | 種別 | 責務 |
|---------|------|------|
| `ItemData` | ScriptableObject | ショップアイテム（消耗品・設計図）のデータ |
| `ShopInventoryGenerator` | plain C# (static) | ノード訪問時にショップ在庫を抽選生成する |
| `ShopSceneController` | MonoBehaviour | ショップ画面の初期化・購入処理 |
| `ShopItemUI` | MonoBehaviour | 在庫アイテム1件の表示・購入ボタン制御 |

## データ構造

```csharp
public enum ItemType { Tower, Blueprint, Consumable }

[CreateAssetMenu(menuName = "GearBox/ItemData")]
public class ItemData : ScriptableObject
{
    public string   itemName;
    public string   description;
    public Sprite   icon;
    public ItemType itemType;
    public int      basePrice;

    public TowerData       towerData;      // itemType == Tower
    public SynthesisRecipe recipeToUnlock; // itemType == Blueprint
    public int             hpRestore;      // itemType == Consumable
}

[System.Serializable]
public class ShopInventory
{
    public List<ShopEntry> entries;
}

[System.Serializable]
public class ShopEntry
{
    public ItemData  item;   // null なら towerData を直接販売
    public TowerData tower;
    public int       price;
    public bool      sold;
}
```

## インターフェース / イベント

```csharp
public class ShopSceneController : MonoBehaviour
{
    // ノード選択時に RunManager のショップ在庫を参照してリストを描画
    public void Initialize(ShopInventory inventory);
    // 購入ボタン押下時
    public void OnBuyClicked(ShopEntry entry);
}
```

## 依存関係

| 参照先 | 用途 |
|--------|------|
| `RunManager`（core） | スクラップ消費・タワー追加・レシピ解放 |
| `TowerData`（tower/base） | タワー在庫のデータ参照 |
| `SynthesisRecipe`（tower/synthesis） | 設計図購入時のレシピ解放 |
