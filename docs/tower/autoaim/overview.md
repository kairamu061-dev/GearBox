# 自動追尾型攻撃 概要

## 目的・背景

射程内の敵を自動検出し、プレイヤー操作なしに弾丸を発射する攻撃タイプ。機関銃・散弾銃などが使用する。

## スコープ

**含む:**
- `AutoAimAttack` MonoBehaviour（IAttackBehaviour 実装）

**含まない:**
- `Projectile`（→ tower/aimed/ と共用 Prefab）
- `AimProvider`（→ tower/base/、AutoAim は使用しない）
- 他の攻撃タイプ

## 制約

- `IAttackBehaviour` インターフェース（tower/base/）に準拠する
- `Physics2D.OverlapCircle` で射程内の敵を毎 CT 検出する
- ターゲット選択は「最も HP が低い敵」を優先する
- 射程内に敵がいない場合は CT をリセットせず次フレームで再チェックする

## 完了条件

- 射程内に敵が存在するとき、CT ごとに最低 HP の敵に向けて弾丸を発射する
- 射程外の敵には反応しない
- 単体テストでターゲット選択・発射ロジックが検証済み
