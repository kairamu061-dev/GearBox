# マップ・ノードシステム テストケース

## ユニットテスト

| ID | テスト名 | 入力 / 条件 | 期待結果 | ステータス |
|----|---------|------------|---------|-----------|
| U-01 | MapGenerator — 全ノード到達可能 | シード値を固定して MapGraph を生成 | GetReachableNodes(startNode) が全ノードを返す | [ ] |
| U-02 | MapGenerator — 最終層はボス固定 | MapGraph を生成 | 最終層のノードの NodeType が Boss のみ | [ ] |
| U-03 | MapGenerator — 層ごとのノード数 | MapGraph を生成 | 各層のノード数が 1〜3 の範囲内 | [ ] |
| U-04 | MapGraph.GetReachableNodes | Current ノードを設定して呼び出す | 接続されている未訪問ノードのみが返る | [ ] |
| U-05 | MapNode — 状態遷移 | NodeState を Reachable → Current に変更 | GetNode で更新後の状態が取得できる | [ ] |
| U-06 | RunManager.CompleteNode | nodeId を渡して CompleteNode を呼ぶ | 対象ノードが Visited 状態になる | [ ] |

## インテグレーションテスト

| ID | テスト名 | 前提条件 | 手順 | 期待結果 | ステータス |
|----|---------|---------|------|---------|-----------|
| I-01 | MapGraphView の描画 | MapGraph をセットして MapGraphView.Build を呼ぶ | ノード数分の NodeButtonUI が生成され接続線が描画される | [ ] |
| I-02 | ノード選択 → シーン遷移 | MapScene でバトルノードを Reachable 状態に設定 | NodeButtonUI をクリック | MapSceneController が PreparationScene へ遷移する | [ ] |
| I-03 | 訪問済みノードの見た目 | ノードを Visited 状態に設定 | MapGraphView を描画 | NodeButtonUI が Visited 用のスプライト・色で表示される | [ ] |
| I-04 | 到達不能ノードのクリック無効 | Unvisited 状態のノードをクリック | OnNodeSelected を呼ぶ | 遷移が発生せず何も起きない | [ ] |

## E2Eテスト

| ID | テスト名 | 前提条件 | 手順 | 期待結果 | ステータス |
|----|---------|---------|------|---------|-----------|
| E-01 | ラン開始 → マップ表示 | ゲームを新規ラン開始 | MapScene に遷移 | 生成されたマップが表示され開始ノードが選択可能 | [ ] |
| E-02 | バトルクリア → 訪問済み更新 | MapScene でバトルノードを選択してバトルへ | バトルをクリアして MapScene へ戻る | 選択したノードが Visited になり次のノードが Reachable になる | [ ] |
| E-03 | ボス撃破 → 次エリア | ボスノードを含むマップで進行 | ボスを撃破する | RunManager.StartNewArea が呼ばれ新しいエリアのマップが生成・表示される | [ ] |

## ステータス凡例

- `[ ]` 未実施
- `[~]` 実施中
- `[x]` 合格
- `[!]` 不合格
