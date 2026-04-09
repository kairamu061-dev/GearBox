# 設置型攻撃 タスク

## 実装タスク一覧

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

### PlacementAttack
- [ ] `PlacementAttack` MonoBehaviour の作成（`IAttackBehaviour` 実装）
  - [ ] `activeObjects` キューの初期化
  - [ ] `Execute()` — 上限チェック → 上限以上なら最古オブジェクトを `pool.Return()` → `pool.Get()` で新規設置

### PlacementObject
- [ ] `PlacementObject` MonoBehaviour の作成
  - [ ] `Initialize(damage, lifetime, pool)` — ダメージ・寿命・プール参照を設定して有効化
  - [ ] `LifetimeCoroutine()` — lifetime 秒後に `ReturnToPool()` を呼ぶ
  - [ ] `OnTriggerEnter2D` — Enemy レイヤーに接触で `TakeDamage()` → `ReturnToPool()`
- [ ] `PlacementObject` Prefab 作成（感知機雷の外観）

### 動作確認
- [ ] CT ごとに自機前方に設置物が生成されることを確認
- [ ] 設置数が上限に達すると最古のものが消えることを確認
- [ ] 敵が接触するとダメージが入り設置物が消えることを確認
- [ ] 寿命 10 秒で自動消滅することを確認

## 依存関係

- `IAttackBehaviour`（tower/base/）→ PlacementAttack の前提
- `ObjectPool<T>`（tower/base/）→ PlacementObject 管理の前提
- `IDamageable` → 接触時ダメージ処理の前提
