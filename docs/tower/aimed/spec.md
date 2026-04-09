# 照準型攻撃 仕様

## 機能一覧

| # | 機能名 | 説明 |
|---|--------|------|
| 1 | 発射方向計算 | CT が 0 になった時点でマウスのワールド座標を取得し、タワーからの単位ベクトルを計算する |
| 2 | 弾丸発射 | ObjectPool から Projectile を取得し、計算した方向で初速を与えて発射する |
| 3 | 弾丸直進 | Projectile は一定速度で直進し、毎フレームで移動する |
| 4 | 命中処理 | 弾丸が敵コライダーに接触すると IDamageable.TakeDamage() を呼ぶ |
| 5 | 射程切れ | 発射起点からの累積移動距離が射程（TowerData.range）を超えたら返却する |
| 6 | 壁衝突 | 壁・障害物コライダーに接触したら即返却する（ダメージなし） |

## 動作仕様（AimedAttack）

1. `TowerBehaviour` が CT をデクリメントし 0 になると `IAttackBehaviour.Execute()` を呼ぶ
2. `AimProvider.AimPosition` からマウスのワールド座標を取得する
3. `(AimPosition - transform.position).normalized` で発射方向を計算する
4. `pool.Get()` で `Projectile` を取得し、位置・方向・速度を設定して有効化する

## 動作仕様（Projectile）

| パラメータ | 値 | 説明 |
|-----------|---|------|
| 移動速度 | 10 units/s（固定） | |
| 最大射程 | TowerData.range | 超えたら返却 |
| 命中レイヤー | Enemy | 敵のみダメージ |
| 壁レイヤー | Obstacle / Wall | 命中でダメージなし返却 |

## エラー / 異常ケース

| 条件 | 挙動 |
|------|------|
| ObjectPool が枯渇 | 新規 Instantiate でプールを拡張する |
| AimProvider が null | 正面方向（transform.up）にフォールバックして発射 |

## 未対応ケース

- 貫通弾・跳弾
- タワー回転アニメーション（照準方向に砲口を向ける）
