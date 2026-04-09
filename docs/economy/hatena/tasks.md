# ハテナノード タスク

## 実装タスク一覧

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

### データ定義
- [ ] `HatenaEventType` enum の定義
- [ ] `HatenaEventData` ScriptableObject の定義
- [ ] イベント7種の ScriptableObject アセット作成（タワー選択・スクラップ発見・設計図発見・廃材の罠・謎の部品・廃兵の遺産・何もない）

### HatenaEventController
- [ ] `HatenaEventController.TriggerEvent()` の実装（ウェイトに従ってランダム抽選）
- [ ] タワー選択イベントの処理実装（ランダム3タワーを提示して選択させる）
- [ ] スクラップ増減イベントの処理実装（ScrapGain / ScrapLoss）
- [ ] 設計図発見イベントの処理実装（UnlockRecipe）
- [ ] 謎の部品イベントの処理実装（ランダムタワーを1段階強化）
- [ ] HP全回復イベントの処理実装
- [ ] フォールバック処理（未知レシピなし・Lv3タワーのみの場合は Nothing にフォールバック）

### オーバーレイ UI
- [ ] MapScene に HatenaOverlay GameObject を追加
- [ ] フレーバーテキスト表示・閉じるボタンの実装
- [ ] タワー選択ボタン3種の動的生成と選択処理

### 動作確認
- [ ] 全7種のイベントが正しく発動することを確認
- [ ] フォールバック処理が正しく機能することを確認

## 依存関係

- `RunManager`（core）→ すべての前提
- `HatenaEventData` アセット → `HatenaEventController` の前提
- `TowerData`（tower/base）→ タワー選択肢生成の前提
- `MapSceneController`（map）→ `TriggerEvent` の呼び出し元
