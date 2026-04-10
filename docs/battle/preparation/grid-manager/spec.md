# GridManager 仕様

## 機能一覧

| # | 機能名 | 説明 |
|---|--------|------|
| 1 | 占有判定 | タワーを指定 origin に配置できるか判定する |
| 2 | 配置実行 | RunManager.GridLayout の対象マスに TowerInstance を書き込む |
| 3 | 除去実行 | 指定マスのタワーが占有するすべてのマスを null に戻す |
| 4 | 占有マス取得 | TowerInstance の占有するマス一覧を返す |

## API 仕様

| メソッド | 引数 | 戻り値 | 説明 |
|---------|------|--------|------|
| `CanPlace(tower, origin, grid)` | TowerData, Vector2Int, TowerInstance[,] | bool | グリッド範囲内かつすべてのマスが空なら true |
| `Place(tower, origin)` | TowerInstance, Vector2Int | void | GridLayout の占有マスに tower を書き込む |
| `Remove(anyOccupiedCell)` | Vector2Int | void | そのマスを占有する TowerInstance の全占有マスを null に戻す |
| `GetOccupiedCells(tower, origin)` | TowerInstance, Vector2Int | List\<Vector2Int\> | タワーのサイズをもとに占有マス一覧を計算して返す |

## 動作仕様

### CanPlace
1. `GetOccupiedCells(tower, origin)` で占有マス一覧を取得
2. 各マスがグリッド範囲内（0 ≤ x < GridSize.x、0 ≤ y < GridSize.y）であることを確認
3. 各マスの `GridLayout[x, y]` が null であることを確認
4. すべて満たせば true、1つでも外れれば false

### Place
1. `GetOccupiedCells` で占有マスを取得
2. 各マスに `RunManager.GridLayout[x, y] = tower` を書き込む
3. `RunManager.OnGridChanged` を発火する（RunManager 側で実装）

### Remove
1. `GridLayout[anyOccupiedCell]` から TowerInstance を取得
2. その TowerInstance の origin を探索して `GetOccupiedCells` を取得
3. 各マスを null に戻す
4. `RunManager.OnGridChanged` を発火する

> **Note:** Remove は origin を引数に取らず、任意の占有マスから探索する。
> GridLayout 全体を走査して該当 TowerInstance の最左上マスを origin として確定する。

## エラー / 異常ケース

| 条件 | 挙動 |
|------|------|
| `CanPlace` が false のまま `Place` を呼んだ | 呼び出し元（TowerDragHandler）が事前に `CanPlace` でチェックする責務を持つ |
| `Remove` で null マスを指定 | 即 return（何もしない） |
| `Place` で origin がグリッド外 | `CanPlace` が false を返すため到達しない |
