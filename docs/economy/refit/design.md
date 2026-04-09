# 改修ノード 設計

## Unityパッケージ / アセット

| パッケージ / アセット | 用途 | 選定理由 |
|----------------------|------|----------|
| TextMeshPro | コスト・説明テキスト | 標準 |

## シーン・オブジェクト構成

```
RefitScene
  ├── [System]
  │     └── RefitSceneController (MonoBehaviour)
  ├── [Header]
  │     ├── ScrapText (TMP_Text)
  │     ├── TabGroup (MonoBehaviour)
  │     │     └── Tab_Upgrade / Tab_Synthesis / Tab_Repair / Tab_Expand / Tab_Sell (Button × 5)
  │     └── DepartButton (Button)
  └── [TabContents]
        ├── UpgradePanel
        │     └── TowerUpgradeUI (MonoBehaviour)
        ├── SynthesisPanel  ← SynthesisUI 共用 Prefab（tower/synthesis）
        ├── RepairPanel
        │     └── RepairUI (MonoBehaviour)
        ├── ExpandPanel     ← GridExpansionUI 共用 Prefab（battle/preparation）
        └── SellPanel
              └── TowerSellUI (MonoBehaviour)
```

## クラス設計

| クラス名 | 種別 | 責務 |
|---------|------|------|
| `RefitSceneController` | MonoBehaviour | 改修画面の初期化・タブ管理・出発処理 |
| `TowerUpgradeUI` | MonoBehaviour | 強化可能タワー一覧・Lv上限チェック・コスト表示・実行 |
| `RepairUI` | MonoBehaviour | 修理量選択・コスト表示・HP満タン時のグレーアウト |
| `TowerSellUI` | MonoBehaviour | タワー選択・売却価格表示・実行 |

## インターフェース / イベント

```csharp
public class RefitSceneController : MonoBehaviour
{
    public void OnUpgrade(TowerInstance tower);  // RunManager のタワーにアップグレード適用
    public void OnRepair(int amount);            // RunManager.Heal
    public void OnSell(TowerInstance tower);     // RunManager からタワー除去してスクラップ加算
}
```

## 依存関係

| 参照先 | 用途 |
|--------|------|
| `RunManager`（core） | スクラップ消費・HP回復・タワー強化・売却 |
| `SynthesisUI`（tower/synthesis） | 合成タブで共用 Prefab を使用 |
| `GridExpansionUI`（battle/preparation） | 拡張タブで共用 Prefab を使用 |
