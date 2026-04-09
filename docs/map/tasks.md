# マップ・ノードシステム タスク

> サブ項目に分割済み。詳細なタスクは各サブ項目を参照。
>
> - [マップ生成 タスク](./generation/tasks.md)
> - [MapScene タスク](./scene/tasks.md)

## 実装タスク一覧（サマリー）

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

### データ定義
- [ ] `MapNode` クラスの定義（`NodeType` / `NodeState` enum 含む）
- [ ] `MapGraph` クラスの定義（`GetNode` / `GetReachableNodes` 含む）
- [ ] `RunManager` への `MapGraph` / `CurrentArea` フィールド追加

### マップ生成
- [ ] `MapGenerator` の実装
  - [ ] 層ごとにノード数（1〜3）をランダム決定
  - [ ] ノード種別を出現率に従って抽選（最終層はボス固定）
  - [ ] 全ノードがスタートから到達可能かチェック・再生成
  - [ ] 各ノードの `position`（表示座標）を計算

### MapScene
- [ ] MapScene の作成（シーン構造・Canvas 設定）
- [ ] `MapGraphView` の実装
  - [ ] ノードを `NodeButton` Prefab として動的配置
  - [ ] 接続線を `LineRenderer` で描画
- [ ] `NodeButtonUI` の実装（状態に応じた見た目: Reachable・Visited・Current・Unvisited）
- [ ] `MapSceneController` の実装
  - [ ] `OnNodeSelected(nodeId)` — 到達可能チェック → `TransitionToNode`
  - [ ] `TransitionToNode(node)` — 種別に応じたシーン遷移
  - [ ] Mystery ノードのオーバーレイ表示

### RunManager 連携
- [ ] `RunManager.CompleteNode(nodeId)` の実装（ノードを Visited に更新）
- [ ] `RunManager.StartNewArea()` の実装（新エリアの `MapGraph` を生成・初期化）

### 動作確認
- [ ] ラン開始時にマップが生成され表示されることを確認
- [ ] 到達可能ノードのみクリックできることを確認
- [ ] ノード選択後に正しいシーンへ遷移し、戻ると訪問済みになることを確認
- [ ] ボスノード撃破後に次エリアのマップへ遷移することを確認

## 依存関係

- `RunManager`（battle/combat で実装）→ マップ状態保持の前提
- `MapNode` / `MapGraph` 定義 → `MapGenerator` の前提
- `MapGenerator` → `MapSceneController` の前提
- `MapGraphView` / `NodeButtonUI` → `MapSceneController` の前提
