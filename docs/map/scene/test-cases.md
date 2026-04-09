# MapScene テストケース

## 単体テスト

| # | テスト名 | 前提条件 | 操作 | 期待結果 |
|---|----------|----------|------|----------|
| U1 | 到達不可ノードのクリックを無視 | NodeButtonUI 初期化済み（Unvisited） | `OnPointerClick()` を呼ぶ | `OnNodeSelected` が呼ばれない |
| U2 | 到達可能ノードのクリックを通知 | NodeButtonUI 初期化済み（Reachable） | `OnPointerClick()` を呼ぶ | `controller.OnNodeSelected(nodeId)` が呼ばれる |
| U3 | RefreshState で色が切り替わる | NodeButtonUI 初期化済み | `RefreshState(Visited)` を呼ぶ | ボタンがグレーアウト表示になる |
| U4 | BuildView でノード数が一致 | MapGraph（ノード10件）をモック | `BuildView(graph)` を呼ぶ | NodeButton が10個生成される |

## 統合テスト

| # | テスト名 | 前提条件 | 操作 | 期待結果 |
|---|----------|----------|------|----------|
| I1 | ノード選択 → RunManager.CompleteNode 呼び出し | MapScene 起動、RunManager 存在 | Reachable ノードをクリック | `RunManager.CompleteNode(nodeId)` が呼ばれ、ノードが Visited になる |
| I2 | Battle ノード選択 → PreparationScene 遷移 | MapScene 起動 | Battle ノードをクリック | PreparationScene がロードされる |
| I3 | Mystery ノード選択 → オーバーレイ表示 | MapScene 起動、HatenaEventController 存在 | Mystery ノードをクリック | ハテナイベント UI がオーバーレイ表示される |

## E2Eテスト

| # | テスト名 | 前提条件 | 操作 | 期待結果 |
|---|----------|----------|------|----------|
| E1 | ノード遷移ループ | ゲーム起動 | MapScene → BattleScene → 勝利 → MapScene | 訪問済みノードが更新され、次のノードが到達可能になる |
