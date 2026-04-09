# タワー定義・攻撃挙動 タスク

> 攻撃タイプ別のタスクはそれぞれのサブ項目を参照。
>
> - [照準型攻撃 タスク](../aimed/tasks.md)
> - [自動追尾型攻撃 タスク](../autoaim/tasks.md)
> - [範囲型攻撃 タスク](../area/tasks.md)
> - [設置型攻撃 タスク](../placement/tasks.md)

## 実装タスク一覧（基盤のみ）

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

### データ定義
- [ ] `IDamageable` インターフェースの定義
- [ ] `IAttackBehaviour` インターフェースの定義
- [ ] `TowerData` ScriptableObject の定義（フィールド・`TowerUpgradeLevel` 含む）
- [ ] `TowerInstance` クラスの定義
- [ ] 初期タワー7種の ScriptableObject アセット作成（蒸気砲・散弾銃・機関銃・迫撃砲・感知機雷・蒸気炎放器・電磁砲塔）

### 共通基盤
- [ ] `ObjectPool<T>` の実装
- [ ] `AimProvider` の実装（`Camera.ScreenToWorldPoint` でマウスワールド座標を毎フレーム更新）

### タワー本体
- [ ] `TowerBehaviour` の実装（CTカウント・`IAttackBehaviour.Execute()` 呼び出し）

### 動作確認
- [ ] `TowerBehaviour` が CT ごとに `IAttackBehaviour.Execute()` を呼ぶことをテストで確認
- [ ] `ObjectPool<T>` の Get/Return サイクルが正しく機能することを確認

## 依存関係

- `IDamageable` / `IAttackBehaviour` 定義 → タワー実装すべての前提
- `ObjectPool<T>` → `Projectile` / `PlacementObject` の前提
- `AimProvider` → `AimedAttack` / `AutoAimAttack` の前提
- `TowerBehaviour` → 攻撃タイプ別実装の前提
- タワー ScriptableObject アセット → Prefab 作成の前提
