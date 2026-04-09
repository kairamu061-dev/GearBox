# タワーシステム タスク

> サブ項目に分割済み。詳細なタスクは各サブ項目を参照。
>
> - [タワー基盤 タスク](./base/tasks.md)
> - [合成システム タスク](./synthesis/tasks.md)

## 実装タスク一覧（サマリー）

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

### タワー基盤（tower/base）
- [ ] `IDamageable` / `IAttackBehaviour` インターフェース定義
- [ ] `TowerData` ScriptableObject 定義
- [ ] `ObjectPool<T>` / `AimProvider` の実装
- [ ] 攻撃タイプ4種の実装（照準・追尾・範囲・設置）
- [ ] `Projectile` / `PlacementObject` の実装
- [ ] 初期タワー7種の ScriptableObject アセット作成
- [ ] タワー Prefab 7種の作成

### 合成システム（tower/synthesis）
- [ ] `SynthesisRecipe` ScriptableObject 定義
- [ ] `SynthesisManager` の実装
- [ ] `SynthesisUI` の実装（再利用可能 Prefab）
- [ ] 合成レシピ6種のアセット作成

## 依存関係

- `IDamageable` → `EnemyController`（enemy）の前提でもある
- `TowerData` → 合成システムの前提
- `TowerBehaviour` → `SynthesisUI` / `GridManager` の前提
- タワー基盤 → 合成システムの前提（素材タワー定義が必要）
