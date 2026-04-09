# 敵システム基盤 テストケース

## ユニットテスト

| ID | テスト名 | 入力 / 条件 | 期待結果 | ステータス |
|----|---------|------------|---------|-----------|
| U-01 | EnemyController.TakeDamage — 通常 | HP=100 で TakeDamage(30) | CurrentHp = 70 | [ ] |
| U-02 | EnemyController.TakeDamage — 死亡 | HP=10 で TakeDamage(10) | Die() が呼ばれる | [ ] |
| U-03 | EnemyController — 二重死亡防止 | Die() 呼び出し中に TakeDamage | 2回目の Die() が呼ばれない | [ ] |
| U-04 | ScrapDropper — 散乱個数 | scrapDropMin=5, scrapDropMax=10 で Drop() | 5〜10 個の ScrapObject が生成される | [ ] |
| U-05 | ScrapObject — 自動消滅 | ScrapObject を生成（lifetime=60s） | 60 秒後に消滅する | [ ] |
| U-06 | EnemyProjectile — 直進 | 右方向（velocity.x > 0）で生成 | 右方向に一定速度で移動する | [ ] |
| U-07 | EnemyProjectile — 戦車ダメージ | 戦車の IDamageable に接触 | TakeDamage が呼ばれ Projectile が消滅する | [ ] |

## インテグレーションテスト

| ID | テスト名 | 前提条件 | 手順 | 期待結果 | ステータス |
|----|---------|---------|------|---------|-----------|
| I-01 | 死亡 → スクラップ散乱 | EnemyController と ScrapDropper を持つ敵を配置 | TakeDamage で HP=0 にする | ScrapObject が散乱し敵 GameObject が消滅する | [ ] |
| I-02 | スタック検知 → リスポーン | 敵を障害物に挟んだ状態で2秒放置 | 2秒経過 | 敵が別位置にリスポーンされる | [ ] |

## E2Eテスト

| ID | テスト名 | 前提条件 | 手順 | 期待結果 | ステータス |
|----|---------|---------|------|---------|-----------|
| E-01 | 全敵撃破 → OnAllEnemiesDefeated | BattleScene に敵3体をスポーン | タワーで全敵を撃破する | OnAllEnemiesDefeated イベントが発火する | [ ] |

## ステータス凡例

- `[ ]` 未実施
- `[~]` 実施中
- `[x]` 合格
- `[!]` 不合格
