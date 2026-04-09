# マップ生成 概要

## 目的・背景

ランごとに一意な分岐付きノードマップを手続き生成する。MapSceneController がマップを表示・操作する前提として、グラフデータを構築する純粋ロジック層。

## スコープ

**含む:**
- `MapNode` / `MapGraph` データ構造
- `NodeType` / `NodeState` 列挙型
- `MapGenerator` 手続き生成アルゴリズム

**含まない:**
- マップの描画・UI（→ map/scene/）
- RunManager へのデータ保持（RunManager 側の責務）

## 制約

- Unity 非依存の純粋 C# クラスのみ。MonoBehaviour 不使用
- 生成後、すべてのノードがスタートから到達可能であることを保証する（不可なら再生成）
- Random.Range は Unity の System.Random ラッパーを使用（テスト時はシード固定可能）

## 完了条件

- `MapGenerator.Generate()` が仕様のパラメータ通りの `MapGraph` を返す
- 孤立ノードが存在しないことを単体テストで確認済み
- `MapGraph.GetReachableNodes()` が正しい隣接リストを返す
