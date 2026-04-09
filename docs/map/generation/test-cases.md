# マップ生成 テストケース

## 単体テスト

| # | テスト名 | 前提条件 | 操作 | 期待結果 |
|---|----------|----------|------|----------|
| U1 | 生成後のノード層数 | — | `MapGenerator.Generate()` を呼ぶ | ノード層数が6（スタート除く） |
| U2 | 最終層はボス固定 | — | `Generate()` を呼ぶ | 最終層の唯一のノードが `NodeType.Boss` |
| U3 | 到達可能性保証 | — | `Generate()` を10回呼ぶ | すべてでスタートから全ノードへ到達可能 |
| U4 | GetNode が正しいノードを返す | MapGraph 生成済み | `GetNode(id)` を各 id で呼ぶ | 各 id に対応するノードが返る |
| U5 | GetReachableNodes が隣接ノードを返す | currentNodeId = スタート | `GetReachableNodes()` | スタートの `nextNodeIds` に含まれるノードリスト |
| U6 | GetReachableNodes が非隣接ノードを除外 | currentNodeId = スタート | `GetReachableNodes()` | スタートの nextNodeIds 以外のノードは含まない |
| U7 | シード固定で同一マップを生成 | — | 同じシードで `Generate()` を2回呼ぶ | 2回のグラフが同一（ノード数・接続・種別） |
| U8 | ノード種別の出現率（通常層） | 1000回生成のサンプル | 通常層の種別集計 | Battle ≈ 50%、Mystery ≈ 20%、Shop ≈ 15%、Refit ≈ 15%（±5%許容） |

## 統合テスト

| # | テスト名 | 前提条件 | 操作 | 期待結果 |
|---|----------|----------|------|----------|
| I1 | RunManager へのグラフ保存 | RunManager 存在 | `Generate()` → RunManager.CurrentMapGraph にセット | RunManager から同一グラフが取得できる |
| I2 | StartNewArea で新グラフに更新 | RunManager 存在・グラフ設定済み | `RunManager.StartNewArea()` | CurrentMapGraph が新しい MapGraph に置き換わる |

## E2Eテスト

| # | テスト名 | 前提条件 | 操作 | 期待結果 |
|---|----------|----------|------|----------|
| E1 | ラン開始から MapScene 表示 | ゲーム起動 | ランを開始する | MapScene に生成されたノードグラフが表示される |
