# 設置型攻撃 仕様

## 機能一覧

| # | 機能名 | 説明 |
|---|--------|------|
| 1 | 設置オブジェクト生成 | CT が 0 になった時点で自機前方に PlacementObject を設置する |
| 2 | 設置上限管理 | 同時設置数が上限（デフォルト: 3）に達している場合、最古の設置物を返却してから設置する |
| 3 | 接触起爆 | PlacementObject に敵が触れると IDamageable.TakeDamage() を呼んでプールへ返却する |
| 4 | 寿命消滅 | 設置から一定時間（デフォルト: 10秒）経過すると自動的にプールへ返却する |

## 動作仕様（PlacementAttack）

1. `TowerBehaviour` が CT を 0 にすると `IAttackBehaviour.Execute()` を呼ぶ
2. アクティブな PlacementObject 数が上限以上なら、最古のオブジェクトを `pool.Return()` する
3. `pool.Get()` で PlacementObject を取得し、自機前方 1.0 unit の位置に配置する
4. PlacementObject を `activeObjects` キューに追加する

## 動作仕様（PlacementObject）

| パラメータ | デフォルト値 | 説明 |
|-----------|------------|------|
| 寿命 | 10 秒 | 経過後に自動返却 |
| 設置上限 | 3 個 | PlacementAttack 側で管理 |
| 命中レイヤー | Enemy | 接触する敵コライダー |

## エラー / 異常ケース

| 条件 | 挙動 |
|------|------|
| ObjectPool が枯渇 | 新規 Instantiate でプールを拡張する |
| 設置位置が壁の中 | 障害物コライダーと重ならない位置に補正する（将来対応でもよい） |

## 未対応ケース

- 設置物の向きアニメーション
- 爆発範囲ダメージ（現仕様は接触点のみ）
