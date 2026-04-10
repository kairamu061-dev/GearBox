# 敵システム基盤 仕様

## 機能一覧

| # | 機能名 | 説明 |
|---|--------|------|
| 1 | EnemyData 定義 | ScriptableObject で敵のパラメータを定義する |
| 2 | IEnemyAI インターフェース | AI の共通契約を定義する |
| 3 | HP 管理 | ダメージを受けて HP を減少させ、0 で死亡処理を呼ぶ |
| 4 | 死亡処理 | ScrapDropper を起動してスクラップを散乱させ、自身を消滅させる |

> **敵弾・スクラップドロップはサブ項目を参照:**
> - 敵弾 → [enemy/projectile/spec.md](../projectile/spec.md)
> - スクラップドロップ / ScrapObject → [enemy/scrap/spec.md](../scrap/spec.md)

## EnemyData フィールド

| フィールド | 型 | 説明 |
|-----------|---|------|
| `enemyName` | string | 表示名 |
| `prefab` | GameObject | 敵 Prefab |
| `maxHp` | float | 最大HP |
| `moveSpeed` | float | 移動速度（固定型は 0） |
| `attackDamage` | float | 1回の攻撃ダメージ |
| `attackCooldown` | float | 攻撃CT（秒） |
| `attackRange` | float | 攻撃射程（ユニット） |
| `aiType` | AIType enum | AI 種別 |
| `scrapDropMin` | int | スクラップドロップ最小値 |
| `scrapDropMax` | int | スクラップドロップ最大値 |
| `projectileSpeed` | float | 弾丸の移動速度（TurretAI / FortressAI / Boss 用） |
| `projectilePrefab` | GameObject | 弾丸 Prefab（砲台系のみ使用） |

## EnemyController 動作仕様

1. `Initialize(EnemyData)` で `EnemyData` を受け取り、HP を初期化する
2. 毎フレーム `IEnemyAI.UpdateAI(tankTransform)` を呼び出す
3. `TakeDamage(float)` で HP を減算し、0 以下で `Die()` を呼ぶ
4. `Die()` で `ScrapDropper.Drop()` → `Destroy(gameObject)` の順で実行する

> **スクラップドロップ・敵弾の詳細はサブ項目を参照:**
> - スクラップドロップ / ScrapObject → [enemy/scrap/spec.md](../scrap/spec.md)
> - 敵弾（EnemyProjectile）→ [enemy/projectile/spec.md](../projectile/spec.md)

## エラー / 異常ケース

| 条件 | 挙動 |
|------|------|
| 敵が障害物にスタック | 2秒経過で強制リスポーン（別位置に再出現） |
| 死亡処理中に二重 TakeDamage | 死亡フラグで二重 Die() を防ぐ |
