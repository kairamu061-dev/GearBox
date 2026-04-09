# 敵システム基盤 タスク

> 独立実装可能な単位はサブ項目に分割済み。詳細は各サブ項目を参照。
>
> - [敵弾 タスク](../projectile/tasks.md)
> - [スクラップドロップ タスク](../scrap/tasks.md)

## 実装タスク一覧（基盤のみ）

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

### データ定義
- [ ] `AIType` enum の定義
- [ ] `EnemyData` ScriptableObject の定義
- [ ] 敵5種の ScriptableObject アセット作成（スクラップウォーカー・蒸気砲台・装甲列車・要塞型・エリアボス）

### インターフェース
- [ ] `IEnemyAI` インターフェースの定義（`UpdateAI(Transform tank)` のみ）

### EnemyController
- [ ] `EnemyController` の実装（`IDamageable` 実装・`Initialize` / `TakeDamage` / `Die`）
- [ ] `Update` 内で `IEnemyAI.UpdateAI` を呼ぶ処理
- [ ] HP=0 検知で `Die` を呼ぶ処理（二重死亡フラグで防護）
- [ ] スタック検知（2秒無移動で強制リスポーン）

### Prefab 作成
- [ ] 通常敵4種の Prefab 作成（`EnemyController` + ScrapDropper + 対応コライダーをアタッチ）
- [ ] ボスの Prefab 作成（`BossController` + ScrapDropper）

### 動作確認
- [ ] EnemyController が TakeDamage を受けて HP を正しく管理することを確認
- [ ] HP=0 で `Die()` が1回だけ呼ばれることを確認（二重死亡フラグ動作確認）

## 依存関係

- `IDamageable`（tower/base）→ `EnemyController` の前提
- `IEnemyAI` → 各 AI 実装（enemy/ai）の前提
- `EnemyController` → Prefab 作成の前提
- `EnemyData` ScriptableObject → Prefab 設定の前提
- `EnemyProjectile`（enemy/projectile/）/ `ScrapDropper`（enemy/scrap/）→ Prefab アタッチの前提
