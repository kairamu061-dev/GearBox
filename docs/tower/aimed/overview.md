# 照準型攻撃 概要

## 目的・背景

マウスカーソル方向へ弾丸を発射する攻撃タイプ。プレイヤーが狙いをつけて射撃する唯一の攻撃タイプであり、蒸気砲・電磁砲塔などが使用する。

## スコープ

**含む:**
- `AimedAttack` MonoBehaviour（IAttackBehaviour 実装）
- `Projectile` MonoBehaviour（直進弾丸・ダメージ・プール返却）

**含まない:**
- `AimProvider`（→ tower/base/）
- `ObjectPool<T>`（→ tower/base/）
- 他の攻撃タイプ（AutoAim: tower/autoaim/, Area: tower/area/, Placement: tower/placement/）

## 制約

- `IAttackBehaviour` インターフェース（tower/base/）に準拠する
- `AimProvider` からマウスのワールド座標を毎フレーム取得する
- `ObjectPool<Projectile>` を使用して弾丸を管理する（直接 Instantiate 禁止）

## 完了条件

- CT ごとにマウス方向へ弾丸が発射される
- 弾丸が敵・壁に命中、または射程超過で IDamageable.TakeDamage を呼んでプールへ返却される
- 単体テストで発射ロジックが検証済み
