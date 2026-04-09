# 準備フェーズ タスク

## 実装タスク一覧

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

### シーン作成
- [ ] PreparationScene の作成（シーン構造・Canvas 設定）

### グリッド管理
- [ ] `GridManager` の実装
  - [ ] `CanPlace(tower, origin, grid)` — 占有判定
  - [ ] `Place(tower, origin)` — RunManager.GridLayout を更新
  - [ ] `Remove(cell)` — 占有マスをすべてクリア
  - [ ] `GetOccupiedCells(tower, origin)` — 占有マス一覧取得

### グリッド UI
- [ ] `GridUI` の実装（`RunManager.GridLayout` をもとにセルを動的生成・再描画）
- [ ] `GridCell` Prefab と `GridCell` MonoBehaviour の実装（`IGridDropTarget`）

### タワー一覧 UI
- [ ] `TowerCardUI` Prefab と実装（アイコン・名前・CT・DMG 表示）
- [ ] `TowerDragHandler` の実装（ドラッグ開始・追従・ドロップ・配置/除去依頼）
- [ ] `TowerPreviewPanel` の実装（ホバー時にパラメータをポップアップ）
- [ ] タワー一覧の ScrollRect 内で `TowerCardUI` を動的生成

### タブ管理
- [ ] `TabGroup` の実装（タブボタンとパネルの切り替え）
- [ ] 配置タブ・合成タブ・拡張タブのパネル切り替え

### 合成タブ
- [ ] `SynthesisUI` を PreparationScene に組み込む（tower/synthesis の実装を再利用）
- [ ] 合成後にグリッドから素材タワーが除去され GridUI が再描画されることを確認

### 拡張タブ
- [ ] `GridExpansionUI` Prefab を ExpandPanel に配置する（実装は [grid-expansion/tasks.md](./grid-expansion/tasks.md) を参照）
- [ ] GridExpansionUI 拡張後に `GridUI` が再描画されることを確認

### 出撃ボタン
- [ ] `PreparationSceneController` の実装（初期化・RefreshUI・出撃処理）
- [ ] 出撃ボタン押下で `RunManager.GridLayout` を確定して BattleScene へ遷移

### 動作確認
- [ ] タワーをグリッドへドラッグ＆ドロップできることを確認
- [ ] サイズ2マス以上のタワーの占有判定が正しいことを確認
- [ ] 合成タブで合成するとグリッドの素材タワーが消えることを確認
- [ ] 拡張タブでグリッドが拡張され GridUI が更新されることを確認
- [ ] 出撃後に BattleScene でグリッドの配置が反映されることを確認

## 依存関係

- `RunManager`（core/）→ すべての前提
- `TowerData` / `TowerInstance`（tower/base）→ GridUI・CardUI の前提
- `GridManager` → `GridUI` / `TowerDragHandler` の前提
- `SynthesisUI` / `SynthesisManager`（tower/synthesis）→ 合成タブの前提
- `GridExpansionUI` → 拡張タブの前提
