# マップ生成 タスク

## 実装タスク一覧

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

### データ定義
- [ ] `NodeType` enum の定義（Battle / Mystery / Shop / Refit / Boss）
- [ ] `NodeState` enum の定義（Unvisited / Reachable / Current / Visited）
- [ ] `MapNode` クラスの定義（id・type・position・nextNodeIds・state）
- [ ] `MapGraph` クラスの定義（nodes・currentNodeId）
  - [ ] `GetNode(int id)` の実装
  - [ ] `GetReachableNodes()` の実装

### MapGenerator
- [ ] `MapGenerator` static クラスの作成
  - [ ] 層ごとのノード数（1〜3）をランダム決定するロジック
  - [ ] ノード種別を出現率に従って抽選するロジック（最終層はボス固定）
  - [ ] 前層と次層のノード間に接続を生成するロジック（最大2本・最低1本保証）
  - [ ] `AssignPositions()` — 各ノードの正規化座標を計算
  - [ ] `ValidateConnectivity()` — BFS で孤立ノードの有無を検証
  - [ ] リトライループ（最大10回）とフォールバック（直線マップ）

### 動作確認
- [ ] `Generate()` が層数6・ボス固定の `MapGraph` を返すことを確認
- [ ] 孤立ノードが存在しないことを単体テストで確認
- [ ] `GetReachableNodes()` が currentNodeId の隣接ノードを正しく返すことを確認

## 依存関係

- 依存なし（他のサブ項目より先に実装可能）
- `MapGraph` 定義 → `MapGenerator` の前提
- `MapGenerator` → map/scene/ の前提
