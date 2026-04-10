# グリッド拡張UI 設計

## Unityパッケージ / アセット

| パッケージ / アセット | 用途 | 選定理由 |
|----------------------|------|----------|
| TextMeshPro | グリッドサイズ・スクラップ数テキスト | Unity 標準の高品位テキスト |

## Prefab 構造

```
GridExpansionUI (MonoBehaviour)
  ├── GridSizeText (TMP_Text)       — "3 × 3" などを表示
  ├── ScrapText (TMP_Text)          — 現在スクラップ数
  ├── AddColumnButton (Button)      — 列を追加
  └── AddRowButton (Button)         — 行を追加
```

## クラス設計

| クラス名 | 種別 | 責務 |
|---------|------|------|
| `GridExpansionUI` | MonoBehaviour | ボタンイベント購読・RunManager 連携・UI 状態更新 |

## インターフェース / イベント

```csharp
public class GridExpansionUI : MonoBehaviour
{
    private void Start();    // RunManager.OnScrapChanged / OnGridChanged を購読して RefreshUI() を登録
    private void OnDestroy();// 購読解除

    public void OnAddColumnClicked();  // AddColumnButton の onClick
    public void OnAddRowClicked();     // AddRowButton の onClick

    private void RefreshUI(); // グリッドサイズテキスト・スクラップ表示・ボタン有効状態を更新
}
```

## 依存関係

| 参照先 | 用途 |
|--------|------|
| `RunManager`（core/） | GridSize 読み取り・SpendScrap・ExpandGrid 呼び出し・OnScrapChanged / OnGridChanged 購読 |
