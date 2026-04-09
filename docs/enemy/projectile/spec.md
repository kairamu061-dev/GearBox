# 敵弾 仕様

## 機能一覧

| # | 機能名 | 説明 |
|---|--------|------|
| 1 | 発射初期化 | 生成時に方向・速度・ダメージを設定する |
| 2 | 直進移動 | 設定した速度で毎フレーム直進する |
| 3 | 戦車命中 | IDamageable を持つ GameObject に接触すると TakeDamage() を呼んで消滅する |
| 4 | 壁・障害物命中 | Obstacle / Wall レイヤーに接触するとダメージなしで消滅する |

## 動作仕様

1. 敵 AI（TurretAI / FortressAI / BossController）が `Instantiate(projectilePrefab)` で生成する
2. `Launch(direction, speed, damage)` を呼んで方向・速度・ダメージを設定する
3. `Rigidbody2D.velocity` で毎フレーム直進する
4. `OnTriggerEnter2D` で命中判定：
   - `IDamageable` → `TakeDamage(damage)` → `Destroy(gameObject)`
   - Wall / Obstacle → `Destroy(gameObject)`

## パラメータ

| パラメータ | 値 | 説明 |
|-----------|---|------|
| 移動速度 | EnemyData.projectileSpeed（Launch() 引数で渡す） | |
| 命中レイヤー | Player（戦車のみ） | |
| 無効レイヤー | Obstacle / Wall | ダメージなし消滅 |

## エラー / 異常ケース

| 条件 | 挙動 |
|------|------|
| フィールド境界外に出た | 境界コライダーに当たり消滅（壁命中と同じ） |
| 同フレームに複数コライダーと接触 | 最初の接触のみ処理。消滅フラグで二重処理を防ぐ |

## 未対応ケース

- 貫通弾（複数の IDamageable を連続ヒット）
- 弾の爆発範囲ダメージ
