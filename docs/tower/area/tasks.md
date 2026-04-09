# 範囲型攻撃 タスク

## 実装タスク一覧

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

### AreaAttack
- [ ] `AreaAttack` MonoBehaviour の作成（`IAttackBehaviour` 実装）
  - [ ] `Execute()` — Physics2D.OverlapCircle で射程内の全敵を取得し、全員に `TakeDamage()` を呼ぶ
  - [ ] エフェクト GameObject の有効化（effectPrefab が設定されている場合のみ）

### Prefab
- [ ] 各範囲型タワーの Prefab に `AreaAttack` をアタッチ
- [ ] Enemy レイヤーマスクを設定

### 動作確認
- [ ] 射程内の全敵に CT ごとにダメージが入ることを確認
- [ ] 射程外の敵にはダメージが入らないことを確認
- [ ] 障害物を無視してダメージが届くことを確認

## 依存関係

- `IAttackBehaviour`（tower/base/）→ AreaAttack の前提
- `IDamageable` → ダメージ処理の前提
