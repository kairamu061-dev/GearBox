# 照準型攻撃 テストケース

## 単体テスト

| # | テスト名 | 前提条件 | 操作 | 期待結果 |
|---|----------|----------|------|----------|
| U1 | Execute でマウス方向に弾丸が発射される | AimedAttack 初期化済み・AimProvider をモック | `Execute()` を呼ぶ | Projectile が AimPosition 方向に Launch される |
| U2 | AimProvider null 時にフォールバック発射 | AimProvider = null | `Execute()` を呼ぶ | transform.up 方向に発射される（エラーなし） |
| U3 | 射程超過で Projectile が返却される | Projectile 発射済み・range = 5 | 累積距離 > 5 になるまで Update | `ReturnToPool()` が呼ばれる |
| U4 | 敵命中で TakeDamage と返却 | Projectile 飛翔中 | Enemy コライダーと接触 | `IDamageable.TakeDamage()` 呼び出し後 `ReturnToPool()` |
| U5 | 壁命中でダメージなし返却 | Projectile 飛翔中 | Wall コライダーと接触 | `TakeDamage()` を呼ばず `ReturnToPool()` |

## 統合テスト

| # | テスト名 | 前提条件 | 操作 | 期待結果 |
|---|----------|----------|------|----------|
| I1 | TowerBehaviour 経由で CT ごとに発射 | TowerBehaviour + AimedAttack 設定済み | Update を CT 秒分実行 | CT ごとに Projectile が1発発射される |
| I2 | プール枯渇時に拡張 | poolSize = 1 の ObjectPool、2発同時発射 | `Execute()` を連続2回呼ぶ | 2発目で Instantiate され、プールが拡張される |

## E2Eテスト

| # | テスト名 | 前提条件 | 操作 | 期待結果 |
|---|----------|----------|------|----------|
| E1 | マウス照準で敵撃破 | BattleScene 起動・照準型タワー配置済み | マウスを敵に合わせて CT を待つ | 弾丸が敵に命中し、HP が減少する |
