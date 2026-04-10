# 準備フェーズ 設計

> サブ項目として独立した設計詳細:
> - `GridManager`（占有判定・配置ロジック）→ [grid-manager/design.md](./grid-manager/design.md)
> - `GridExpansionUI`（グリッド拡張UI）→ [grid-expansion/design.md](./grid-expansion/design.md)

## Unityパッケージ / アセット

| パッケージ / アセット | 用途 | 選定理由 |
|----------------------|------|----------|
| DOTween | タブ切り替えアニメーション・合成成功演出 | 軽量 |
| TextMeshPro | UI テキスト全般 | Unity 標準の高品位テキスト |

## シーン・オブジェクト構成

```
PreparationScene
  ├── [System]
  │     └── PreparationSceneController (MonoBehaviour)
  │
  ├── [Header]
  │     ├── TitleText
  │     ├── TabGroup (MonoBehaviour)
  │     │     ├── Tab_Placement (Button)
  │     │     ├── Tab_Synthesis (Button)
  │     │     └── Tab_Expand (Button)
  │     └── SortieButton (Button)
  │
  ├── [TabContents]
  │     ├── PlacementPanel
  │     │     ├── TowerInventoryPanel
  │     │     │     └── TowerCardUI × n (ScrollRect 内)
  │     │     └── GridPanel
  │     │           ├── GridUI (MonoBehaviour)
  │     │           └── GridCell × (W×H) (Prefab)
  │     │
  │     ├── SynthesisPanel
  │     │     └── SynthesisUI (MonoBehaviour) ← tower/synthesis と共用 Prefab
  │     │
  │     └── ExpandPanel
  │           └── GridExpansionUI (MonoBehaviour)
  │
  └── [Preview]
        └── TowerPreviewPanel (ホバー時に表示)
```

## クラス設計

| クラス名 | 種別 | 責務 |
|---------|------|------|
| `PreparationSceneController` | MonoBehaviour | 画面初期化・タブ切り替え・出撃ボタン処理 |
| `TabGroup` | MonoBehaviour | タブボタン管理、アクティブパネルの切り替え |
| `GridUI` | MonoBehaviour | `RunManager.GridLayout` をもとにグリッドを描画・更新 |
| `GridCell` | MonoBehaviour | 1マスの表示・ドロップ受け入れ処理 |
| `TowerCardUI` | MonoBehaviour | 所持タワー1件の表示（アイコン・名前・パラメータ） |
| `TowerDragHandler` | MonoBehaviour | タワーのドラッグ開始・移動・ドロップ処理、グリッドへの配置依頼 |
| `GridManager` | plain C# | `RunManager.GridLayout` の読み書き・占有判定ロジック |
| `SynthesisUI` | MonoBehaviour | tower/synthesis と共用。準備フェーズでもそのまま使用 |
| `GridExpansionUI` | MonoBehaviour | グリッド拡張ボタンの有効/無効制御・拡張実行 |
| `TowerPreviewPanel` | MonoBehaviour | ホバー中タワーのパラメータをポップアップ表示 |

## データ構造

```csharp
// GridManager の内部ロジック（RunManager の GridLayout を操作）
public static class GridManager
{
    // タワーをグリッドに配置できるか判定
    // origin: 左上マスの座標
    public static bool CanPlace(TowerData tower, Vector2Int origin, TowerInstance[,] grid);

    // 配置を実行（RunManager.GridLayout を更新）
    public static void Place(TowerInstance tower, Vector2Int origin);

    // 指定マスのタワーを除去（占有マスをすべてクリア）
    public static void Remove(Vector2Int anyOccupiedCell);

    // TowerInstance の占有マス一覧を返す
    public static List<Vector2Int> GetOccupiedCells(TowerInstance tower, Vector2Int origin);
}
```

## インターフェース / イベント

```csharp
// ドラッグ中のハイライト更新
// TowerDragHandler → GridUI へ通知
public interface IGridDropTarget
{
    bool CanAccept(TowerData tower, Vector2Int cell);
    void OnDrop(TowerDragHandler handler, Vector2Int cell);
}

// グリッド・所持リスト変更時の UI 再描画
// RunManager のデータ変更後に PreparationSceneController が RefreshUI() を呼ぶ
public class PreparationSceneController : MonoBehaviour
{
    public void RefreshUI();  // GridUI + TowerInventoryPanel を再描画
}
```

## 依存関係

| 参照先 | 用途 |
|--------|------|
| `RunManager` | GridLayout・TowerInventory・Scrap の読み書き |
| `GridManager` | 占有判定・配置・除去ロジック |
| `SynthesisManager` | 合成タブの合成実行 |
| `TowerData` | カード表示・ドラッグ対象データ |
