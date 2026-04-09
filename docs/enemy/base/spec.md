# 敵システム基盤 仕様

## 機能一覧

| # | 機能名 | 説明 |
|---|--------|------|
| 1 | EnemyData 定義 | ScriptableObject で敵のパラメータを定義する |
| 2 | IEnemyAI インターフェース | AI の共通契約を定義する |
| 3 | HP 管理 | ダメージを受けて HP を減少させ、0 で死亡処理を呼ぶ |
| 4 | 死亡処理 | ScrapDropper を起動してスクラップを散乱させ、自身を消滅させる |
| 5 | 敵弾 | 直進してプレイヤーに当たると TakeDamage を呼ぶ |
| 6 | スクラップドロップ | 死亡時にスクラップオブジェクトをランダム散乱する |
| 7 | ScrapObject | フィールドに落ちるスクラップ。60秒で自動消滅する |

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
| `projectilePrefab` | GameObject | 弾丸 Prefab（砲台系のみ使用） |

## EnemyController 動作仕様

1. `Initialize(EnemyData)` で `EnemyData` を受け取り、HP を初期化する
2. 毎フレーム `IEnemyAI.UpdateAI(tankTransform)` を呼び出す
3. `TakeDamage(float)` で HP を減算し、0 以下で `Die()` を呼ぶ
4. `Die()` で `ScrapDropper.Drop()` → `Destroy(gameObject)` の順で実行する

## スクラップドロップ仕様

- 死亡時に `scrapDropMin〜scrapDropMax` のランダム値個の `ScrapObject` を生成する
- `ScrapObject` は死亡位置の周囲（半径 1.5 ユニット）にランダム散乱する
- `ScrapObject` は 60 秒後に自動消滅する

## ScrapObject 回収時の RunManager 連携

- 戦車の `ScrapCollector`（`CircleCollider2D` trigger）が `ScrapObject` に接触すると `OnTriggerEnter2D` が発火する
- **`ScrapObject` 側が `RunManager.AddScrap(1)` を呼び出して加算する**（ScrapCollector 側では呼ばない）
- 加算後、`ScrapObject` は即座に自身を非活性化してプールへ返却する（詳細は enemy/scrap/ を参照）

## EnemyProjectile 動作仕様

- 生成時に方向を設定し、一定速度（velocity）で直進する
- `IDamageable` を持つ GameObject に当たると `TakeDamage` を呼ぶ
- 壁または障害物に当たると消滅する

## エラー / 異常ケース

| 条件 | 挙動 |
|------|------|
| 敵が障害物にスタック | 2秒経過で強制リスポーン（別位置に再出現） |
| 死亡処理中に二重 TakeDamage | 死亡フラグで二重 Die() を防ぐ |
