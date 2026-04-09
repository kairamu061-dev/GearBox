# 敵弾 テストケース

## 単体テスト

| # | テスト名 | 前提条件 | 操作 | 期待結果 |
|---|----------|----------|------|----------|
| U1 | Launch で指定方向に速度が設定される | EnemyProjectile 非活性 | `Launch(Vector2.up, 5f, 10f)` を呼ぶ | Rigidbody2D.velocity が Vector2.up * 5 になる |
| U2 | Player レイヤーへの命中で TakeDamage | EnemyProjectile 飛翔中 | Player コライダーと接触 | `IDamageable.TakeDamage(damage)` が呼ばれて GameObject が破棄される |
| U3 | Obstacle レイヤー命中でダメージなし消滅 | EnemyProjectile 飛翔中 | Obstacle コライダーと接触 | `TakeDamage` は呼ばれず GameObject が破棄される |
| U4 | 二重接触で二重処理なし | EnemyProjectile 飛翔中 | 同フレームに2コライダーと接触 | `TakeDamage` は1回のみ呼ばれる |

## 統合テスト

| # | テスト名 | 前提条件 | 操作 | 期待結果 |
|---|----------|----------|------|----------|
| I1 | TurretAI が発射した弾が戦車にダメージ | BattleScene・TurretAI の射程内に戦車 | CT 経過を待つ | 弾が発射されて戦車 HP が減少する |

## E2Eテスト

| # | テスト名 | 前提条件 | 操作 | 期待結果 |
|---|----------|----------|------|----------|
| E1 | 敵弾によるゲームオーバー | BattleScene・HP = 1 の戦車・砲台敵が射程内 | CT を待つ | 弾が命中して HP = 0 → ゲームオーバー遷移 |
