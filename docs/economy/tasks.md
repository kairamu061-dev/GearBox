# スクラップ経済システム タスク

> サブ項目に分割済み。詳細なタスクは各サブ項目を参照。
>
> - [ショップ タスク](./shop/tasks.md)
> - [改修ノード タスク](./refit/tasks.md)
> - [ハテナノード タスク](./hatena/tasks.md)

## 実装タスク一覧（サマリー）

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

### データ定義
- [ ] `ItemData` ScriptableObject の定義（`ItemType` enum 含む）
- [ ] 消耗品アイテムの ScriptableObject アセット作成（修理キット大小）
- [ ] `HatenaEventData` ScriptableObject の定義
- [ ] ハテナイベント7種の ScriptableObject アセット作成
- [ ] `ShopInventory` / `ShopEntry` クラスの定義

### ショップ
- [ ] `ShopInventoryGenerator` の実装（出現率に従ってタワー・アイテムを抽選）
- [ ] ShopScene の作成（シーン構造・Canvas 設定）
- [ ] `ShopSceneController` の実装（初期化・購入処理・`RunManager` 反映）
- [ ] `ShopItemUI` Prefab と実装（アイコン・名前・価格・「購入」ボタン）
- [ ] 残高不足時のボタングレーアウト制御
- [ ] 購入後に sold フラグを立てて在庫から除外

### 改修ノード
- [ ] RefitScene の作成（シーン構造・タブ構成）
- [ ] `RefitSceneController` の実装（タブ管理・各操作を `RunManager` へ反映）
- [ ] `TowerUpgradeUI` の実装（強化可能タワー一覧表示・Lv上限・コスト表示・実行）
- [ ] `RepairUI` の実装（修理量選択・コスト表示・満タン時グレーアウト）
- [ ] `TowerSellUI` の実装（タワー一覧から選択・売却価格表示・実行）
- [ ] `GridExpansionUI` を RefitScene にも組み込む（preparation と共用 Prefab）
- [ ] `SynthesisUI` を RefitScene にも組み込む（tower/synthesis と共用 Prefab）

### ハテナノード
- [ ] `HatenaEventController` の実装
  - [ ] ハテナイベントを出現率に従ってランダム抽選
  - [ ] 各イベント種別の処理実装（タワー選択肢 / スクラップ増減 / 設計図解放 / 強化 / 全回復 / 何もない）
  - [ ] MapScene 上にオーバーレイ形式でイベント UI を表示

### 動作確認
- [ ] ショップでタワーを購入するとスクラップが消費され所持タワーに追加されることを確認
- [ ] 残高不足で購入ボタンが押せないことを確認
- [ ] 改修でタワー強化・修理・売却・グリッド拡張が正しく機能することを確認
- [ ] ハテナイベントの全種別が正しく発動することを確認

## 依存関係

- `RunManager`（battle/combat で実装）→ すべての前提
- `TowerData`（tower/base）→ ショップ・改修の前提
- `SynthesisUI` / `SynthesisManager`（tower/synthesis）→ 改修の合成タブの前提
- `GridExpansionUI`（battle/preparation）→ 改修の拡張タブの前提（共用 Prefab）
- `ShopInventoryGenerator` → `ShopSceneController` の前提
- `HatenaEventData` アセット → `HatenaEventController` の前提
