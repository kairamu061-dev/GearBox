# ショップ タスク

## 実装タスク一覧

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

### データ定義
- [ ] `ItemType` enum の定義
- [ ] `ItemData` ScriptableObject の定義
- [ ] `ShopInventory` / `ShopEntry` クラスの定義
- [ ] 消耗品アイテムの ScriptableObject アセット作成（修理キット・大型修理キット）

### ShopInventoryGenerator
- [ ] `ShopInventoryGenerator.Generate()` の実装（出現率に従ってタワー・アイテムを抽選）
- [ ] 在庫を RunManager に保存する処理（再訪問時に再生成しない）

### ShopScene と UI
- [ ] ShopScene の作成（Canvas 設定）
- [ ] `ShopItemUI` Prefab の作成（アイコン・名前・価格・購入ボタン）
- [ ] `ShopItemUI` MonoBehaviour の実装（表示・ボタン有効/無効・sold 表示）
- [ ] `ShopSceneController.Initialize(inventory)` の実装
- [ ] `ShopSceneController.OnBuyClicked(entry)` の実装（SpendScrap → AddTower / UnlockRecipe / Heal）
- [ ] 残高不足時のボタングレーアウト制御
- [ ] 購入後の sold フラグ管理

### 動作確認
- [ ] タワー購入でスクラップが消費され所持タワーに追加されることを確認
- [ ] 残高不足で購入ボタンが押せないことを確認
- [ ] 再訪問時に在庫が変化しないことを確認

## 依存関係

- `RunManager`（core）→ すべての前提
- `TowerData`（tower/base）→ 在庫生成の前提
- `SynthesisRecipe`（tower/synthesis）→ 設計図アイテムの前提
- `ShopInventoryGenerator` → `ShopSceneController` の前提
