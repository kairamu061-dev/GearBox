# グリッド拡張UI テストケース

## 単体テスト

| # | テスト名 | 前提条件 | 操作 | 期待結果 |
|---|----------|----------|------|----------|
| U1 | スクラップ不足でボタン無効 | Scrap = 50 | `RefreshUI()` を呼ぶ | AddColumn・AddRow ボタンが interactable = false |
| U2 | 列上限でボタン無効 | GridSize.x = 5・Scrap = 200 | `RefreshUI()` を呼ぶ | AddColumn ボタンが interactable = false |
| U3 | 行上限でボタン無効 | GridSize.y = 5・Scrap = 200 | `RefreshUI()` を呼ぶ | AddRow ボタンが interactable = false |
| U4 | 列追加ボタン押下で ExpandGrid 呼ばれる | Scrap = 200・GridSize = (3, 3) | `OnAddColumnClicked()` を呼ぶ | `RunManager.SpendScrap(80)` → `ExpandGrid(true)` が呼ばれる |
| U5 | OnScrapChanged で即時ボタン更新 | GridExpansionUI 初期化済み・Scrap = 200 | `RunManager.AddScrap(-190)` で Scrap を 10 に減らす | ボタンが即座に無効化される |

## 統合テスト

| # | テスト名 | 前提条件 | 操作 | 期待結果 |
|---|----------|----------|------|----------|
| I1 | PreparationScene での拡張 | PreparationScene 起動・Scrap = 200 | AddColumn ボタンを押す | GridSize が (4, 3) になり GridUI が更新される |
| I2 | RefitScene での拡張（共用 Prefab） | RefitScene 起動・Scrap = 200 | 拡張タブの AddRow ボタンを押す | GridSize が (3, 4) になる（PreparationScene と同じ動作） |

## E2Eテスト

| # | テスト名 | 前提条件 | 操作 | 期待結果 |
|---|----------|----------|------|----------|
| E1 | グリッド拡張 → バトルで反映 | PreparationScene でグリッドを (4, 4) に拡張 | 出撃ボタンを押す | BattleScene の戦車グリッドが 4×4 になる |
