# 敵システム テストケース

## ユニットテスト

| ID | テスト名 | 入力 / 条件 | 期待結果 | ステータス |
|----|---------|------------|---------|-----------|
| U-01 | EnemyController.TakeDamage | HP=100 の敵に TakeDamage(30) | CurrentHp = 70 になる | [ ] |
| U-02 | EnemyController — 死亡 | HP=10 の敵に TakeDamage(10) | 死亡処理が呼ばれ ScrapDropper が起動する | [ ] |
| U-03 | ChaserAI — 移動方向 | 戦車が北10mにいる状態で UpdateAI | Rigidbody2D.velocity が北向き（y > 0）になる | [ ] |
| U-04 | TurretAI — 射程外 | 戦車が range 外にいる状態で UpdateAI | 弾丸が発射されない | [ ] |
| U-05 | TurretAI — 射程内・CT中 | 戦車が range 内・CT未満の状態で UpdateAI | 弾丸が発射されない | [ ] |
| U-06 | TurretAI — 射程内・CT経過 | 戦車が range 内・CT以上の状態で UpdateAI | EnemyProjectile が生成される | [ ] |
| U-07 | RusherAI — 壁反転 | 壁に当たった状態で UpdateAI | 移動方向が反転する | [ ] |
| U-08 | FortressAI — HP閾値チェック | HP=100 から TakeDamage(51) で HP=49 にする | 子ユニットスポーンが呼ばれる | [ ] |
| U-09 | ScrapDropper — スクラップ数 | EnemyData.scrapDrop = (2, 5) で死亡 | 2〜5個の ScrapObject が生成される | [ ] |
| U-10 | ScrapObject — 自動消滅 | ScrapObject を生成（lifetime=60s） | 60秒後に Destroy される | [ ] |

## インテグレーションテスト

| ID | テスト名 | 前提条件 | 手順 | 期待結果 | ステータス |
|----|---------|---------|------|---------|-----------|
| I-01 | 敵弾が戦車にダメージ | TurretAI の敵と TankController を同シーンに配置 | CT 経過まで待機 | TankController の HP が敵弾ダメージ分減少する | [ ] |
| I-02 | 死亡 → スクラップ散乱 → 自動回収 | スクラップウォーカーと ScrapCollector を持つ戦車を配置 | 敵を撃破する | ScrapObject が散乱し、戦車が近づくと自動回収されて RunManager に加算される | [ ] |
| I-03 | ボスフェーズ移行 | BossController の Prefab を配置（HP=300, フェーズ2=150） | HP を 151 → 149 に減らす | フェーズ2の AI に切り替わる | [ ] |

## E2Eテスト

| ID | テスト名 | 前提条件 | 手順 | 期待結果 | ステータス |
|----|---------|---------|------|---------|-----------|
| E-01 | 通常バトルで全敵撃破 | BattleScene に通常敵4種をスポーン | タワーで敵を全滅させる | 全敵が死亡しスクラップが散乱・回収される | [ ] |
| E-02 | ボスノードでボス撃破 | ボスノードの BattleScene でボスをスポーン | ボスを3フェーズで撃破する | エリアクリア処理が走り MapScene に遷移する | [ ] |

## ステータス凡例

- `[ ]` 未実施
- `[~]` 実施中
- `[x]` 合格
- `[!]` 不合格
