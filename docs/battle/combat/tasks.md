# 戦闘フェーズ タスク

## 実装タスク一覧

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

### RunManager（ゲーム全体の基盤）

> **RunManager は `core/` に独立。** 詳細は [core/tasks.md](../../core/tasks.md) を参照。
> 戦闘フェーズの実装を開始する前に `core/` を完了させること。

### シーン・フィールド
- [ ] BattleScene の作成（シーン構造・Tilemap 設定）
- [ ] フィールド境界の壁コライダー設置
- [ ] 障害物 Tilemap の配置ロジック（ランダム配置）
- [ ] 旗 GameObject の作成（`CircleCollider2D` + `FlagTrigger`）
- [ ] `FlagTrigger` の実装

### 戦車
- [ ] `TankController` の実装（WASD 入力 → `Rigidbody2D.velocity`・`IDamageable` 実装）
- [ ] `AimProvider` の実装（`Camera.ScreenToWorldPoint`・tower/base と共用）
- [ ] `ScrapCollector` の実装（`CircleCollider2D` trigger で `ScrapObject` を自動回収）
- [ ] タワースロットの動的生成（`RunManager.GridLayout` をもとに `TowerBehaviour` をアタッチ）

### バトル管理
- [ ] `BattleSceneController` の実装
  - [ ] バトル開始処理（敵・旗のスポーン）
  - [ ] `OnFlagReached()` → クリア演出 → MapScene 遷移
  - [ ] `OnBossDefeated()` → エリアクリア処理 → MapScene 遷移
  - [ ] `OnTankDestroyed()` → ゲームオーバー演出 → GameOverScene 遷移

### HUD
- [ ] `BattleHUD` の実装（`OnHpChanged` / `OnScrapChanged` を購読して UI 更新）
- [ ] `MinimapController` の実装（自機・敵・旗をアイコンで表示・旗への矢印）

### カメラ
- [ ] Cinemachine 2D Virtual Camera の設定（戦車追従・フィールド境界クランプ）

### 動作確認
- [ ] WASD 移動・障害物衝突を確認
- [ ] タワーが CT 通りに自動攻撃することを確認（tower/base の統合確認）
- [ ] スクラップの回収が `RunManager` に反映されることを確認
- [ ] 旗到達でクリア遷移、HP0 でゲームオーバー遷移を確認
- [ ] ボスノードでボス撃破 → エリアクリアを確認

## 依存関係

- `RunManager` → すべての前提（最初に実装する）
- `IDamageable` / `TowerBehaviour`（tower/base）→ タワースロット生成の前提
- `EnemyController` / `ScrapObject`（enemy）→ 敵スポーン・スクラップ回収の前提
- `BattleSceneController` → `FlagTrigger` / `TankController` の前提
- `BattleHUD` → `RunManager` イベントの前提
