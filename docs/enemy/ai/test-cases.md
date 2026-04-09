# 敵 AI 実装 テストケース

## ユニットテスト

| ID | テスト名 | 入力 / 条件 | 期待結果 | ステータス |
|----|---------|------------|---------|-----------|
| U-01 | ChaserAI — 移動方向 | 戦車が北10mにいる状態で UpdateAI | velocity が北向き（y > 0）になる | [ ] |
| U-02 | TurretAI — 射程外 | 戦車が attackRange 外にいる状態で UpdateAI | 弾丸が発射されない | [ ] |
| U-03 | TurretAI — 射程内・CT中 | 戦車が range 内・CT未満の状態で UpdateAI | 弾丸が発射されない | [ ] |
| U-04 | TurretAI — 射程内・CT経過 | 戦車が range 内・CT以上の状態で UpdateAI | EnemyProjectile が1発生成される | [ ] |
| U-05 | RusherAI — 直線移動 | 初期方向（右）で UpdateAI | velocity が右方向（x > 0）で一定 | [ ] |
| U-06 | RusherAI — 壁反転 | 壁コライダーに当たった状態 | velocity の x 符号が反転する | [ ] |
| U-07 | FortressAI — HP閾値チェック | HP=100 から TakeDamage(51) で HP=49 | 子ユニットスポーンが1回呼ばれる | [ ] |
| U-08 | FortressAI — スポーン重複なし | HP 閾値を2回通過させる（バフ→デバフ） | 子ユニットスポーンは合計1回 | [ ] |
| U-09 | BossController — フェーズ2移行 | HP=100%→59% に減らす | フェーズ2の攻撃CT に変わる | [ ] |
| U-10 | BossController — フェーズ3移行 | HP=60%→29% に減らす | 全方位弾が有効になる | [ ] |

## インテグレーションテスト

| ID | テスト名 | 前提条件 | 手順 | 期待結果 | ステータス |
|----|---------|---------|------|---------|-----------|
| I-01 | TurretAI の弾丸が戦車にダメージ | TurretAI の敵と TankController を同シーンに配置（射程内） | CT 経過まで待機 | TankController の HP が attackDamage 分減少する | [ ] |
| I-02 | BossController — フェーズ3全方位弾 | ボス Prefab をバトルシーンに配置 | HP を 29% 以下にする | 全方位（8方向）に EnemyProjectile が発射される | [ ] |

## E2Eテスト

| ID | テスト名 | 前提条件 | 手順 | 期待結果 | ステータス |
|----|---------|---------|------|---------|-----------|
| E-01 | ボスノードでボス撃破 | ボスノードの BattleScene でボスをスポーン | ボスを3フェーズかけて撃破する | BattleSceneController.OnBossDefeated が呼ばれエリアクリア遷移する | [ ] |

## ステータス凡例

- `[ ]` 未実施
- `[~]` 実施中
- `[x]` 合格
- `[!]` 不合格
