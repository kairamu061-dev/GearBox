# 照準型攻撃 タスク

## 実装タスク一覧

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

### AimedAttack
- [ ] `AimedAttack` MonoBehaviour の作成（`IAttackBehaviour` 実装）
  - [ ] `Execute()` — AimProvider からマウス方向を取得してプールから Projectile を取得・発射
  - [ ] AimProvider 未設定時のフォールバック（transform.up 方向）

### Projectile
- [ ] `Projectile` MonoBehaviour の作成
  - [ ] `Launch(direction, speed, range, damage)` — 初速・射程・ダメージを設定して有効化
  - [ ] 毎フレーム直進移動（`Rigidbody2D.velocity` または `transform.Translate`）
  - [ ] 累積移動距離が射程超過で `ReturnToPool()`
  - [ ] `OnTriggerEnter2D` — Enemy レイヤーに命中で `TakeDamage()` → `ReturnToPool()`
  - [ ] `OnTriggerEnter2D` — Obstacle/Wall レイヤーに命中でダメージなし → `ReturnToPool()`
- [ ] `Projectile` Prefab 作成（デフォルト外観、タワー種別で差し替え可能）

### 動作確認
- [ ] テストシーンで CT ごとにマウス方向へ弾丸が発射されることを確認
- [ ] 弾丸が敵に命中してダメージが入ることを確認
- [ ] 射程超過・壁衝突で弾丸が返却されることを確認

## 依存関係

- `IAttackBehaviour`（tower/base/）→ AimedAttack の前提
- `AimProvider`（tower/base/）→ AimedAttack の前提
- `ObjectPool<T>`（tower/base/）→ Projectile 管理の前提
- `IDamageable` → 命中処理の前提
