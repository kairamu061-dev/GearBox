# GridManager 設計

## クラス設計

| クラス名 | 種別 | 責務 |
|---------|------|------|
| `GridManager` | static class (pure C#) | 占有判定・配置・除去・占有マス計算 |

## データ構造

```csharp
public static class GridManager
{
    // タワーを origin に配置できるか判定
    public static bool CanPlace(TowerData tower, Vector2Int origin, TowerInstance[,] grid);

    // 配置を実行（RunManager.GridLayout を更新し OnGridChanged を発火）
    public static void Place(TowerInstance tower, Vector2Int origin);

    // 任意の占有マスを起点に、そのタワーの全占有マスを null に戻す
    public static void Remove(Vector2Int anyOccupiedCell);

    // タワーのサイズから占有マス一覧を計算
    public static List<Vector2Int> GetOccupiedCells(TowerInstance tower, Vector2Int origin);
}
```

## 依存関係

| 参照先 | 用途 |
|--------|------|
| `RunManager`（core/） | GridLayout の読み書き・OnGridChanged 発火 |
| `TowerData`（tower/base/） | タワーサイズ（occupiedSize）の参照 |
| `TowerInstance`（tower/base/） | グリッドセルの値として使用 |
