# 敵弾 タスク

## 実装タスク一覧

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

### EnemyProjectile
- [ ] `EnemyProjectile` MonoBehaviour の作成
  - [ ] `Launch(direction, speed, damage)` — 方向・速度・ダメージを設定して Rigidbody2D.velocity を適用
  - [ ] `OnTriggerEnter2D` — Player レイヤー命中で `IDamageable.TakeDamage()` → Destroy
  - [ ] `OnTriggerEnter2D` — Obstacle / Wall レイヤー命中でダメージなし → Destroy
  - [ ] 消滅フラグ（bool）で二重処理を防ぐ

### Prefab
- [ ] `EnemyProjectile` Prefab の作成（Rigidbody2D + CircleCollider2D + EnemyProjectile）

### 動作確認
- [ ] 戦車に命中すると TakeDamage が呼ばれることを確認
- [ ] 壁に命中するとダメージなしで消滅することを確認
- [ ] フィールド境界外に出ると消滅することを確認

## 依存関係

- `IDamageable`（tower/base/）→ 命中処理の前提
- `EnemyController`（enemy/base/）→ 発射元として使用（設計上は任意、EnemyProjectile 自体は非依存）
