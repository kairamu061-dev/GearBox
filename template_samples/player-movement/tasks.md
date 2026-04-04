# プレイヤー移動 タスク

## 実装タスク一覧

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

- [x] `PlayerConfig` ScriptableObject の作成
- [x] `GroundChecker` の実装（Physics2D.OverlapCircle）
- [x] `PlayerController` の実装（左右移動・Animator連携）
- [~] `GravityController` の実装（重力切り替え・クールタイム）
- [ ] Input System の InputActions 設定（キーボード・ゲームパッド）
- [ ] アニメーター設定（Idle / Running / Falling ステートマシン）
- [ ] 単体テスト・動作確認

## 依存関係

- `PlayerConfig` ScriptableObject → 全タスクの前提
- `GroundChecker` → `PlayerController` より先に実装
- Input System 設定 → `PlayerController` 実装と並行可
