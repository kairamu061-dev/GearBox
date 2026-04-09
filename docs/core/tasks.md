# コア（RunManager） タスク

## 実装タスク一覧

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

### シングルトン基盤
- [ ] `RunManager` の MonoBehaviour 作成（DontDestroyOnLoad・二重生成ガード）
- [ ] `TowerInstance` クラスの定義

### 状態フィールドの初期化
- [ ] HP フィールド（MaxHp=100, CurrentHp=100）の定義
- [ ] Scrap フィールドの定義
- [ ] GridSize / GridLayout フィールドの定義と初期化（3×3 の空グリッド）
- [ ] TowerInventory / KnownRecipes リストの初期化

### API 実装
- [ ] `AddScrap(int amount)` — スクラップ加算・イベント発火
- [ ] `SpendScrap(int amount)` — 残高チェック・消費・イベント発火・bool 返却
- [ ] `TakeDamage(int amount)` — HP 減算（0止め）・イベント発火
- [ ] `Heal(int amount)` — HP 加算（MaxHp 上限）・イベント発火
- [ ] `ExpandGrid(bool addColumn)` — GridSize 拡張・GridLayout 再構築
- [ ] `AddTower` / `RemoveTower` — TowerInventory の追加・削除
- [ ] `CompleteNode(int nodeId)` — MapGraph のノード状態を Visited に更新
- [ ] `StartNewArea()` — 新エリアの MapGraph を生成して CurrentMapGraph に設定
- [ ] `UnlockRecipe(SynthesisRecipe recipe)` — 重複チェックして KnownRecipes に追加

### イベント定義
- [ ] `OnHpChanged` static event の定義
- [ ] `OnScrapChanged` static event の定義

### 動作確認
- [ ] `SpendScrap` が残高不足で false を返すことを確認
- [ ] `TakeDamage` で HP が 0 未満にならないことを確認
- [ ] `ExpandGrid` で GridLayout が正しく再構築されることを確認
- [ ] シーン遷移後も `RunManager.Instance` が維持されることを確認

## 依存関係

- `TowerData`（tower/base）→ `TowerInstance` の定義の前提
- `SynthesisRecipe`（tower/synthesis）→ `KnownRecipes` の型の前提
- `MapGraph`（map）→ `CompleteNode` / `StartNewArea` の前提
- `RunManager` → すべての機能エリアの前提（最初に実装する）
