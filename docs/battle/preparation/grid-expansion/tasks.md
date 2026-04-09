# グリッド拡張UI タスク

## 実装タスク一覧

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

### GridExpansionUI
- [ ] `GridExpansionUI` MonoBehaviour の作成
  - [ ] `Start()` で `RunManager.OnScrapChanged` を購読して `RefreshUI()` を登録
  - [ ] `OnAddColumnClicked()` — 残高・上限チェック → `SpendScrap(80)` → `ExpandGrid(true)` → `RefreshUI()`
  - [ ] `OnAddRowClicked()` — 残高・上限チェック → `SpendScrap(80)` → `ExpandGrid(false)` → `RefreshUI()`
  - [ ] `RefreshUI()` — GridSizeText・ScrapText を更新、ボタン有効/無効を切り替える
  - [ ] `OnDestroy()` で購読解除

### Prefab
- [ ] `GridExpansionUI` Prefab の作成（GridSizeText + ScrapText + AddColumnButton + AddRowButton）
- [ ] PreparationScene の ExpandPanel に Prefab を配置
- [ ] RefitScene の 拡張タブに同 Prefab を配置（追加実装なし）

### 動作確認
- [ ] スクラップ 80 未満でボタンがグレーアウトされることを確認
- [ ] GridSize が (5, 5) でボタンがグレーアウトされることを確認
- [ ] ボタン押下でグリッドが拡張され表示が更新されることを確認
- [ ] RefitScene でも同 Prefab が正しく動作することを確認

## 依存関係

- `RunManager`（core/）→ SpendScrap・ExpandGrid・OnScrapChanged の前提
