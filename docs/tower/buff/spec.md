# バフ型タワー 仕様

## 機能一覧

| # | 機能名 | 説明 |
|---|--------|------|
| 1 | バフ適用 | グリッドに配置された時点でパッシブバフを BuffManager に登録する |
| 2 | バフ解除 | グリッドから除去された時点でバフを BuffManager から削除し再計算する |
| 3 | 加算スタック | 同種バフのタワーを複数積むと効果値が加算される |
| 4 | 最大HP管理 | 最大HP バフ適用時に currentHp の上限を更新する。除去時はクランプして減少させる |
| 5 | CT倍率適用 | 潤滑装置のCT倍率は全 TowerBehaviour のクールダウン計算に乗算する |

## タワー一覧

| タワー名 | バフ種別 | 効果量 | 概要 |
|---------|---------|--------|------|
| 過給器 | 移動速度 | +30% | 戦車スピードアップ（加算スタック） |
| 磁気回収機 | スクラップ取得量・回収半径 | +50% / +1.5 units | スクラップ効率アップ（加算スタック） |
| 潤滑装置 | 全タワーCT倍率 | -15% | 全体攻撃速度アップ（加算スタック） |
| 装甲強化板 | 最大HP | +30 HP | タンク耐久力アップ（加算スタック） |

## BuffManager の管理仕様

| バフ種別 | 基準値 | 計算方法 | 適用先 |
|---------|--------|---------|--------|
| 移動速度 | TankStats.baseMoveSpeed | baseMoveSpeed × (1 + 合計%) | Rigidbody2D の velocity 係数 |
| スクラップ取得量 | 1.0（倍率） | 1.0 + 合計% | ScrapPickup.value への乗算 |
| スクラップ回収半径 | TankStats.basePickupRadius | basePickupRadius + 合計値 | CircleCollider2D の radius |
| 全タワーCT倍率 | 1.0（倍率） | 1.0 - 合計% | TowerBehaviour.cooldown への乗算 |
| 最大HP | TankStats.baseMaxHp | baseMaxHp + 合計値 | TankHealth.maxHp |

## 動作仕様（BuffBehaviour）

### 配置時（OnPlaced）

1. `buffManager.AddBuff(data.buffType, data.buffValue)` を呼ぶ
2. BuffManager がバフ種別の合計値を更新し、対応するコンポーネントへ反映する

### 除去時（OnRemoved）

1. `buffManager.RemoveBuff(data.buffType, data.buffValue)` を呼ぶ
2. BuffManager がバフ種別の合計値を更新し、対応するコンポーネントへ反映する
3. 最大HP バフの場合、currentHp が新しい maxHp を超えていればクランプする

## TowerData 追加フィールド（バフ型専用）

| フィールド | 型 | 説明 |
|-----------|---|------|
| buffType | BuffType | バフの種別（MoveSpeed / ScrapAmount / PickupRadius / TowerCooldown / MaxHp） |
| buffValue | float | バフの加算量（%指定は 0.30 = 30%、実数指定は 30.0 = +30） |

## エラー / 異常ケース

| 条件 | 挙動 |
|------|------|
| 装甲強化板を除去したとき currentHp が新しい maxHp を超えていた | currentHp を maxHp にクランプする（HP強制減少） |
| 潤滑装置を複数積んで CT倍率合計が 100% を超えた | CT の最小値を 0.1s にクランプする（0以下にしない） |
| バフ型タワーを BattleScene 開始後に除去しようとした | BattleScene ではグリッド変更不可のため発生しない |

## 未対応ケース

- 時限バフ（CTベースの一時的なバフ）
- 敵へのデバフ
- バフ上限の設定（現状は加算スタック無制限）
