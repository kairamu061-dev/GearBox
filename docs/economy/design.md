# スクラップ経済システム 設計

> サブ項目に分割済み。詳細な設計は各サブ項目を参照。
>
> - [ショップ設計](./shop/design.md)
> - [改修ノード設計](./refit/design.md)
> - [ハテナノード設計](./hatena/design.md)

## Unityパッケージ / アセット

| パッケージ / アセット | 用途 | 選定理由 |
|----------------------|------|----------|
| DOTween | 購入成功・ボタン押下アニメーション | 軽量 |
| TextMeshPro | 価格・スクラップ数・説明テキスト | 標準高品位テキスト |

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

RefitScene
  ├── [System]
  │     └── RefitSceneController (MonoBehaviour)
  ├── [Header]
  │     ├── ScrapText
  │     ├── TabGroup (MonoBehaviour)
  │     │     ├── Tab_Upgrade / Tab_Synthesis / Tab_Repair / Tab_Expand / Tab_Sell
  │     └── DepartButton
  └── [TabContents]
        ├── UpgradePanel
        │     └── TowerUpgradeUI (MonoBehaviour)
        ├── SynthesisPanel ← SynthesisUI と共用 Prefab
        ├── RepairPanel
        │     └── RepairUI (MonoBehaviour)
        ├── ExpandPanel
        │     └── GridExpansionUI と共用 Prefab
        └── SellPanel
              └── TowerSellUI (MonoBehaviour)
```

## クラス設計

| クラス名 | 種別 | 責務 |
|---------|------|------|
| `ItemData` | ScriptableObject | ショップアイテム（消耗品・設計図）のデータ |
| `ShopInventoryGenerator` | plain C# (static) | ノード訪問時にショップ在庫を抽選生成する |
| `ShopSceneController` | MonoBehaviour | ショップ画面の初期化・購入処理 |
| `ShopItemUI` | MonoBehaviour | 在庫アイテム1件の表示・購入ボタン制御 |
| `RefitSceneController` | MonoBehaviour | 改修画面の初期化・タブ管理 |
| `TowerUpgradeUI` | MonoBehaviour | タワー強化タブ: 所持タワーのレベルアップ処理 |
| `RepairUI` | MonoBehaviour | 修理タブ: HP 回復量の選択と実行 |
| `TowerSellUI` | MonoBehaviour | 売却タブ: タワーをスクラップに変換 |
| `HatenaEventData` | ScriptableObject | ハテナイベント1件のデータ（種別・テキスト・効果） |
| `HatenaEventController` | MonoBehaviour | ランダムイベントを抽選して実行、結果UIを表示 |

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

    // itemType == Tower なら towerData を参照
    public TowerData        towerData;
    // itemType == Blueprint なら recipe を参照
    public SynthesisRecipe  recipeToUnlock;
    // itemType == Consumable
    public int              hpRestore;
}

// ShopInventoryGenerator の出力
[System.Serializable]
public class ShopInventory
{
    public List<ShopEntry> entries;
}
[System.Serializable]
public class ShopEntry
{
    public ItemData item;    // null なら towerData を直接販売
    public TowerData tower;
    public int       price;
    public bool      sold;
}

public enum HatenaEventType
{
    TowerChoice, ScrapGain, BlueprintDiscover,
    ScrapLoss, RandomUpgrade, FullHeal, Nothing
}

[CreateAssetMenu(menuName = "GearBox/HatenaEventData")]
public class HatenaEventData : ScriptableObject
{
    public HatenaEventType eventType;
    [TextArea] public string flavorText;
    public int scrapMin, scrapMax;       // ScrapGain / ScrapLoss 用
    public int towerChoiceCount = 3;     // TowerChoice 用
}
```

## インターフェース / イベント

```csharp
// ShopSceneController の主要 API
public class ShopSceneController : MonoBehaviour
{
    // ノード選択時に RunManager のショップ在庫を参照してリストを描画
    public void Initialize(ShopInventory inventory);
    // 購入ボタン押下時
    public void OnBuyClicked(ShopEntry entry);
}

// RefitSceneController の主要 API
public class RefitSceneController : MonoBehaviour
{
    public void OnUpgrade(TowerInstance tower);  // RunManager にアップグレード適用
    public void OnRepair(int amount);            // RunManager.Heal
    public void OnSell(TowerInstance tower);     // RunManager から除去してスクラップ加算
}
```

## 依存関係

| 参照先 | 用途 |
|--------|------|
| `RunManager` | スクラップ増減・HP回復・タワー追加/除去・グリッド拡張 |
| `SynthesisUI` / `SynthesisManager` | 改修の合成タブで tower/synthesis の実装を再利用 |
| `GridExpansionUI` | 改修・準備フェーズの拡張タブで共用 |
| `TowerData` / `ItemData` | ショップ在庫のデータ参照 |
