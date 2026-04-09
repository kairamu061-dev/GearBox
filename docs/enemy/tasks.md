# 敵システム タスク

> サブ項目に分割済み。詳細なタスクは各サブ項目を参照。
>
> - [敵システム基盤 タスク](./base/tasks.md)
> - [敵 AI 実装 タスク](./ai/tasks.md)

## 実装タスク一覧（サマリー）

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

### データ定義
- [ ] `EnemyData` ScriptableObject の定義（`BossPhase` 含む）
- [ ] 敵5種の ScriptableObject アセット作成（スクラップウォーカー・蒸気砲台・装甲列車・要塞型・エリアボス）

### 共通基盤
- [ ] `IEnemyAI` インターフェースの定義
- [ ] `EnemyController` の実装（HP管理・`TakeDamage`・死亡処理・`IEnemyAI.UpdateAI()` 呼び出し）
- [ ] `EnemyProjectile` の実装（敵弾: 直進・戦車への `TakeDamage` 呼び出し）
- [ ] `ScrapDropper` の実装（死亡時にスクラップをランダム散乱）
- [ ] `ScrapObject` の実装（フィールドに残るスクラップ・60秒で自動消滅）

### AI 種別別実装
- [ ] `ChaserAI` の実装（戦車方向へ直進・`Rigidbody2D.velocity` 更新）
- [ ] `TurretAI` の実装（固定砲台・射程内の戦車へ CT ごとに発射）
- [ ] `RusherAI` の実装（直線移動・壁/障害物で反転）
- [ ] `FortressAI` の実装（`TurretAI` 相当の砲撃 + HP50% 以下で子ユニットスポーン）
- [ ] `BossController` の実装（HP 閾値でフェーズ移行・フェーズ別 AI 切り替え）

### Prefab / アセット
- [ ] 通常敵4種の Prefab 作成（`EnemyController` + 対応 AI コンポーネントをアタッチ）
- [ ] ボスの Prefab 作成（`BossController` をアタッチ）
- [ ] `EnemyProjectile` Prefab 作成
- [ ] `ScrapObject` Prefab 作成

### 動作確認
- [ ] 各 AI タイプが仕様通りに動作することを確認
- [ ] HP0 で `ScrapDropper` が起動してスクラップが散乱することを確認
- [ ] スタック時のリスポーン処理が機能することを確認
- [ ] ボスの3フェーズ移行を確認

## 依存関係

- `IDamageable`（tower/base）→ `EnemyController` の前提
- `IEnemyAI` 定義 → 各 AI 実装の前提
- `EnemyController` → AI 種別実装・`ScrapDropper` の前提
- `EnemyData` ScriptableObject → Prefab 作成の前提
