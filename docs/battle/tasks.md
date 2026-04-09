# バトルシステム タスク

> サブ項目に分割済み。詳細なタスクは各サブ項目を参照。
>
> - [準備フェーズ タスク](./preparation/tasks.md)
> - [戦闘フェーズ タスク](./combat/tasks.md)

## 実装タスク一覧（サマリー）

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

### 戦闘フェーズ（battle/combat）
- [ ] `RunManager` の実装（最優先）
- [ ] BattleScene の作成・フィールド設定
- [ ] `TankController` / `AimProvider` / `ScrapCollector` の実装
- [ ] `BattleSceneController` の実装
- [ ] `BattleHUD` / `MinimapController` の実装
- [ ] Cinemachine 2D カメラ設定

### 準備フェーズ（battle/preparation）
- [ ] `GridManager` の実装
- [ ] `GridUI` / `GridCell` Prefab の実装
- [ ] `TowerCardUI` / `TowerDragHandler` の実装
- [ ] タブ管理（配置・合成・拡張）
- [ ] `PreparationSceneController` の実装

## 依存関係

- `RunManager` → すべての前提（最初に実装する）
- 戦闘フェーズ → 準備フェーズ（GridLayout の確定が先）
- `TowerData`（tower/base）→ 両フェーズの前提
- `EnemyController`（enemy）→ 戦闘フェーズの前提
