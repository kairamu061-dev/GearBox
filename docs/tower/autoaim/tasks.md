# 自動追尾型攻撃 タスク

## 実装タスク一覧

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

### AutoAimAttack
- [ ] `AutoAimAttack` MonoBehaviour の作成（`IAttackBehaviour` 実装）
  - [ ] `FindTarget(range)` — Physics2D.OverlapCircle で射程内の敵を全取得し、HP 最低のものを返す
  - [ ] `Execute()` — ターゲットが存在すれば方向計算 → Projectile 発射・CT リセット、なければスキップ

### Prefab
- [ ] 各自動追尾型タワーの Prefab に `AutoAimAttack` をアタッチ
- [ ] Projectile Pool の参照を設定（tower/aimed/ の Projectile Prefab を使用）

### 動作確認
- [ ] 射程内に敵が入ると自動的に発射されることを確認
- [ ] 射程外の敵には反応しないことを確認
- [ ] 複数敵がいる場合に HP 最低の敵がターゲットになることを確認

## 依存関係

- `IAttackBehaviour`（tower/base/）→ AutoAimAttack の前提
- `ObjectPool<T>`（tower/base/）→ Projectile 管理の前提
- `Projectile`（tower/aimed/）→ 弾丸クラスの前提
- `IDamageable` → 命中処理の前提
