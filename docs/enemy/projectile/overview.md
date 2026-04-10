# 敵弾 概要

## 目的・背景

敵（砲台系・ボス）が発射する弾丸の挙動を実装する。戦車の `IDamageable` インターフェースを通じてダメージを与える。

## スコープ

**含む:**
- `EnemyProjectile` MonoBehaviour（直進・衝突・ダメージ・消滅）
- `EnemyProjectile` Prefab

**含まない:**
- 敵の AI・発射タイミング制御（→ enemy/ai/）
- プレイヤー側の Projectile（→ tower/aimed/）
- スクラップドロップ（→ enemy/scrap/）

## 制約

- `IDamageable`（tower/base/）に準拠してダメージを与える
- ObjectPool は使用しない（通常敵・ボスフェーズ1〜2は頻度が低く Instantiate/Destroy で十分）
  - ボスフェーズ3の8方向同時射撃など高頻度が必要になった場合は enemy/projectile/dev-notes.md に記録して再検討する
- 壁・障害物レイヤーに当たった場合はダメージなしで即消滅する

## 完了条件

- 指定方向に直進し、戦車コライダーに命中すると `TakeDamage()` を呼んで消滅する
- 壁に当たると消滅する（ダメージなし）
- 単体テストで衝突挙動が検証済み
