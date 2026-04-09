# 敵 AI 実装 タスク

## 実装タスク一覧

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

### ChaserAI
- [ ] `ChaserAI` の実装（戦車方向への velocity 更新・簡易障害物回避）
- [ ] ChaserAI を Prefab（スクラップウォーカー）にアタッチして動作確認

### TurretAI
- [ ] `TurretAI` の実装（CT タイマー・射程チェック・EnemyProjectile 発射）
- [ ] TurretAI を Prefab（蒸気砲台）にアタッチして動作確認

### RusherAI
- [ ] `RusherAI` の実装（直線方向決定・等速移動・壁/障害物で方向反転）
- [ ] RusherAI を Prefab（装甲列車）にアタッチして動作確認

### FortressAI
- [ ] `FortressAI` の実装（TurretAI 相当の砲撃 + HP50% 閾値でスポーン）
- [ ] 子ユニットスポーン位置の空き判定（壁内スポーン防止）
- [ ] FortressAI を Prefab（要塞型）にアタッチして動作確認

### BossController
- [ ] `BossPhase` 構造体の定義
- [ ] `BossController` の実装（HP 閾値検知・フェーズ切り替え）
- [ ] フェーズ1（低速移動 + 通常砲撃）の実装
- [ ] フェーズ2（中速 + 高速砲撃 + 子ユニットスポーン）の実装
- [ ] フェーズ3（高速移動 + 全方位弾 + 突進）の実装
- [ ] 撃破時に `BattleSceneController.OnBossDefeated()` を呼ぶ処理
- [ ] BossController を Prefab（エリアボス）にアタッチして動作確認

### 動作確認
- [ ] 各 AI タイプが仕様通りに動作することを確認
- [ ] ボスの3フェーズ移行を確認

## 依存関係

- `IEnemyAI`（enemy/base）→ すべての AI 実装の前提
- `EnemyController`（enemy/base）→ AI アタッチの前提
- `EnemyProjectile`（enemy/base）→ TurretAI / FortressAI / BossController の前提
