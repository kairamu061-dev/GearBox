# GridManager タスク

## 実装タスク一覧

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

### GridManager
- [ ] `GridManager` static class の作成
  - [ ] `GetOccupiedCells(tower, origin)` — TowerData.occupiedSize から占有マス一覧を生成
  - [ ] `CanPlace(tower, origin, grid)` — 範囲内チェックと null チェック
  - [ ] `Place(tower, origin)` — GridLayout への書き込みと OnGridChanged 発火
  - [ ] `Remove(anyOccupiedCell)` — GridLayout 走査・全占有マスの null 化と OnGridChanged 発火

### 動作確認
- [ ] 1×1 タワーのCanPlace・Place・Remove が正しく動くことを確認
- [ ] 2×1 / 1×2 / 2×2 タワーの占有判定が正しいことを確認
- [ ] グリッド端に配置しようとすると CanPlace が false になることを確認

## 依存関係

- `RunManager`（core/）→ GridLayout アクセスの前提
- `TowerData` / `TowerInstance`（tower/base/）→ サイズ・インスタンス参照の前提
- `GridManager` → `GridUI` / `TowerDragHandler`（battle/preparation/）の前提
