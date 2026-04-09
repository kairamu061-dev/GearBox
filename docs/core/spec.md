# コア（RunManager） 仕様

## 機能一覧

| # | 機能名 | 説明 |
|---|--------|------|
| 1 | シングルトン管理 | DontDestroyOnLoad で全シーンに跨って単一インスタンスを保持する |
| 2 | HP 管理 | 戦車の現在HP・最大HPを保持し、ダメージ・回復 API を提供する |
| 3 | スクラップ管理 | スクラップ数を保持し、加算・消費 API を提供する（マイナス禁止） |
| 4 | グリッド管理 | グリッドサイズ・各セルのタワー配置を保持し、拡張 API を提供する |
| 5 | タワー在庫管理 | 手持ちタワー一覧を保持し、追加・削除 API を提供する |
| 6 | 合成レシピ管理 | 既知の合成レシピ一覧を保持し、解放 API を提供する |
| 7 | マップ進行管理 | 現在エリアの MapGraph・現在ノード ID を保持する |
| 8 | 状態変更イベント | HP・スクラップが変化した際にイベントを発火して購読者に通知する |

## 状態フィールド

| フィールド | 型 | 初期値 | 説明 |
|-----------|---|--------|------|
| `MaxHp` | int | 100 | 戦車の最大HP |
| `CurrentHp` | int | 100 | 戦車の現在HP |
| `Scrap` | int | 0 | 所持スクラップ数 |
| `GridSize` | Vector2Int | (3, 3) | グリッドの列数・行数 |
| `GridLayout` | TowerInstance[,] | all null | グリッドの配置状態 |
| `TowerInventory` | List\<TowerInstance\> | 空 | 手持ちタワー（グリッド外） |
| `KnownRecipes` | List\<SynthesisRecipe\> | 空 | 解放済み合成レシピ |
| `CurrentMapGraph` | MapGraph | null | 現在エリアのマップグラフ |
| `CurrentNodeId` | int | -1 | 現在選択中のノード ID |

## API 仕様

| メソッド | 引数 | 戻り値 | 説明 |
|---------|------|--------|------|
| `AddScrap(int amount)` | amount ≥ 0 | void | スクラップを加算し `OnScrapChanged` を発火 |
| `SpendScrap(int amount)` | amount ≥ 0 | bool | 残高が足りれば消費して true / 不足なら false（消費しない） |
| `TakeDamage(int amount)` | amount ≥ 0 | void | HP を減算（0未満は0止め）し `OnHpChanged` を発火 |
| `Heal(int amount)` | amount ≥ 0 | void | HP を加算（MaxHp 上限）し `OnHpChanged` を発火 |
| `ExpandGrid(bool addColumn)` | true=列追加 | void | GridSize と GridLayout を拡張する |
| `AddTower(TowerInstance t)` | - | void | TowerInventory に追加 |
| `RemoveTower(TowerInstance t)` | - | void | TowerInventory から除去 |
| `CompleteNode(int nodeId)` | - | void | 指定ノードを Visited 状態に更新 |
| `StartNewArea()` | - | void | 新エリアの MapGraph を生成・CurrentMapGraph を更新 |
| `UnlockRecipe(SynthesisRecipe r)` | - | void | KnownRecipes に追加（重複無視） |

## イベント

| イベント名 | シグネチャ | 発火タイミング |
|-----------|-----------|--------------|
| `OnHpChanged` | `Action<int, int>` (current, max) | TakeDamage / Heal 後 |
| `OnScrapChanged` | `Action<int>` (total) | AddScrap / SpendScrap 後 |

## TowerInventory と GridLayout の関係

| フィールド | 意味 |
|-----------|------|
| `TowerInventory` | グリッドに配置されていない「手持ち」のタワー一覧 |
| `GridLayout` | グリッド各セルの配置状態（null = 空き） |

**配置時（PreparationScene でグリッドへドロップ）:**
1. `RemoveTower(t)` で TowerInventory から除去
2. GridLayout の対象セルに TowerInstance を書き込む

**除去時（グリッドからタワーを引き剥がす）:**
1. GridLayout の占有セルをすべて null に戻す
2. `AddTower(t)` で TowerInventory に再登録

**バトル開始時（BattleScene 遷移後）:**
- GridLayout の内容をもとにタワーを Instantiate する
- TowerInventory はその時点で変更しない（バトル中は在庫操作なし）

**ゲームオーバー時:**
- GridLayout・TowerInventory はリセットせず RunManager が保持し続ける（GameOverScene で選択肢を表示するため）

## グリッド拡張の上限

| 方向 | 初期値 | 最大値 |
|------|--------|--------|
| 列（横） | 3 | 5 |
| 行（縦） | 3 | 5 |

## エラー / 異常ケース

| 条件 | 挙動 |
|------|------|
| `SpendScrap` で残高不足 | false を返し状態変更なし・イベント非発火 |
| `TakeDamage` で HP が 0 以下 | CurrentHp = 0 で止める（マイナスにしない） |
| `ExpandGrid` で上限超え | 処理をスキップ（呼び出し側が事前チェックする） |
| シーン遷移時の二重 Instantiate | 後から生成されたインスタンスを即 Destroy |
