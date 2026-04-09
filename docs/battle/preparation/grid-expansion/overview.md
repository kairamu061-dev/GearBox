# グリッド拡張UI 概要

## 目的・背景

スクラップを消費してタワーグリッドの行または列を追加する UI コンポーネント。準備フェーズと改修ノードの両方で同一 Prefab として使用される。

## スコープ

**含む:**
- `GridExpansionUI` MonoBehaviour（ボタン制御・RunManager.ExpandGrid() 呼び出し）
- `GridExpansionUI` Prefab（shared Prefab）

**含まない:**
- グリッドデータの管理（→ core/RunManager）
- グリッドの描画（→ battle/preparation/ の GridUI）
- このUIを組み込むシーン側のコントローラ（→ battle/preparation/ / economy/refit/）

## 制約

- **Prefab のオーナーは battle/preparation/grid-expansion/** — RefitScene は同じ Prefab をそのまま使用する
- スクラップ不足・グリッド上限（5×5）のチェックは UI 側で実施する（RunManager の呼び出し前にガード）
- `RunManager.OnScrapChanged` を購読してボタン状態をリアルタイム更新する

## 完了条件

- 「列を追加」「行を追加」ボタンが残高・上限に応じて有効/無効になる
- ボタン押下で `RunManager.ExpandGrid()` が呼ばれてグリッドが正しく拡張される
- PreprationScene・RefitScene の両方で同一 Prefab が正しく機能する
