# 改修ノード タスク

## 実装タスク一覧

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

### RefitScene
- [ ] RefitScene の作成（タブ構成: 強化 / 合成 / 修理 / 拡張 / 売却）
- [ ] `RefitSceneController` の実装（初期化・タブ管理・出発処理）

### タワー強化タブ
- [ ] `TowerUpgradeUI` の実装（強化可能タワー一覧表示・Lv上限チェック・コスト表示・実行）

### 修理タブ
- [ ] `RepairUI` の実装（修理量ボタン3種・HP満タン時グレーアウト・コスト表示）

### 売却タブ
- [ ] `TowerSellUI` の実装（タワー一覧表示・売却価格計算・実行・RunManager 反映）

### 合成タブ・拡張タブ
- [ ] `SynthesisUI`（tower/synthesis）の共用 Prefab を SynthesisPanel に組み込む
- [ ] `GridExpansionUI`（battle/preparation）の共用 Prefab を ExpandPanel に組み込む

### 動作確認
- [ ] タワー強化がコスト消費・パラメータ変化を正しく反映することを確認
- [ ] 修理がHP回復・コスト消費を正しく反映することを確認
- [ ] タワー売却がタワー削除・スクラップ増加を正しく反映することを確認
- [ ] 合成タブ・拡張タブが準備フェーズと同等に動作することを確認

## 依存関係

- `RunManager`（core）→ すべての前提
- `SynthesisUI`（tower/synthesis）→ 合成タブの前提
- `GridExpansionUI`（battle/preparation）→ 拡張タブの前提
- `RefitSceneController` → 各 UI タブの前提
