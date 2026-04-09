# MapScene タスク

## 実装タスク一覧

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

### シーン構築
- [ ] MapScene の作成（Canvas・カメラ・シーン構造のセットアップ）
- [ ] NodeButton Prefab の作成（Image・TMP_Text・NodeButtonUI）
- [ ] MapHUD の作成（AreaLabel・ScrapText）

### MapGraphView
- [ ] `MapGraphView` の実装
  - [ ] `BuildView(MapGraph)` — ノードボタンを動的生成・正規化座標に配置
  - [ ] `LineRenderer` で接続線を描画

### NodeButtonUI
- [ ] `NodeButtonUI` の実装
  - [ ] `Initialize(MapNode, controller)` — ノードデータとコントローラを受け取り初期化
  - [ ] `RefreshState(NodeState)` — 状態に応じた色・アニメーションを適用
  - [ ] Reachable 状態で DOTween 点滅アニメーション

### MapSceneController
- [ ] `MapSceneController` の実装
  - [ ] Start() で `RunManager.CurrentMapGraph` を取得 → `MapGraphView.BuildView()` 呼び出し
  - [ ] `OnNodeSelected(nodeId)` — 到達可能チェック → `TransitionToNode()` 呼び出し
  - [ ] `TransitionToNode(node)` — 種別ごとのシーン遷移
  - [ ] Mystery ノード選択時に `HatenaEventController.TriggerEvent()` を呼ぶ

### 動作確認
- [ ] ラン開始時にマップが正しく描画されることを確認
- [ ] 到達可能なノードのみクリックできることを確認
- [ ] ノード選択後に正しいシーンへ遷移し、戻ると訪問済みになることを確認
- [ ] ボスノード撃破後に次エリアのマップへ遷移することを確認

## 依存関係

- `MapGraph` / `MapGenerator`（map/generation/）→ 描画データの前提
- `RunManager`（core/）→ マップ状態読み取りの前提
- `HatenaEventController`（economy/hatena/）→ Mystery 遷移の前提
