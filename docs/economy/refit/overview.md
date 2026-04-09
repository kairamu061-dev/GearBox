# 改修ノード 概要

## 目的・背景

改修ノードで提供されるタワー管理システムを実装する。タワー強化・HP修理・タワー売却の3機能を提供し、ランの中盤以降のデッキ調整を担う。合成とグリッド拡張は準備フェーズと共用の Prefab を使用する。

## スコープ

### 作るもの

- `RefitSceneController`（タブ管理・各操作を RunManager へ反映）
- `TowerUpgradeUI`（強化可能タワー一覧・Lv上限・コスト表示・実行）
- `RepairUI`（修理量選択・コスト表示・満タン時グレーアウト）
- `TowerSellUI`（タワー一覧から選択・売却価格表示・実行）
- RefitScene の作成（タブ構成: 強化 / 合成 / 修理 / 拡張 / 売却）

### 作らないもの

- ショップ購入（economy/shop で対応）
- ハテナイベント（economy/hatena で対応）
- 合成 UI の実装本体（tower/synthesis の SynthesisUI を再利用）
- グリッド拡張 UI の実装本体（battle/preparation の GridExpansionUI を再利用）

## 制約

- 合成タブは `SynthesisUI`（tower/synthesis）の共用 Prefab を差し込む
- 拡張タブは `GridExpansionUI`（battle/preparation）の共用 Prefab を差し込む
- タワー強化は最大 Lv3 まで

## 完了条件

- 改修でタワー強化・修理・売却が実行できる
- 合成タブ・拡張タブが準備フェーズと同等に機能する
- 残高不足・Lv上限・HP満タンでボタンがグレーアウトされる
