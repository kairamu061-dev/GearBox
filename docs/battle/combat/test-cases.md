# 戦闘フェーズ テストケース

## ユニットテスト

| ID | テスト名 | 入力 / 条件 | 期待結果 | ステータス |
|----|---------|------------|---------|-----------|
| U-01 | RunManager.AddScrap | 初期スクラップ100に AddScrap(50) | CurrentScrap = 150 になる | [ ] |
| U-02 | RunManager.SpendScrap — 残高あり | スクラップ100で SpendScrap(60) | true を返し CurrentScrap = 40 になる | [ ] |
| U-03 | RunManager.SpendScrap — 残高不足 | スクラップ30で SpendScrap(60) | false を返し CurrentScrap が変わらない | [ ] |
| U-04 | RunManager.TakeDamage | HP=100 で TakeDamage(25) | CurrentHp = 75 になり OnHpChanged イベントが発火する | [ ] |
| U-05 | RunManager.TakeDamage — HP0以下 | HP=10 で TakeDamage(20) | CurrentHp = 0 になりゲームオーバーフラグが立つ | [ ] |
| U-06 | TankController — WASD 入力 | Input.GetAxis("Horizontal") = 1 | Rigidbody2D.velocity.x > 0 になる | [ ] |
| U-07 | ScrapCollector — trigger | ScrapObject が CircleCollider2D 内に入る | RunManager.AddScrap が呼ばれ ScrapObject が消える | [ ] |
| U-08 | FlagTrigger — 戦車接触 | FlagTrigger に TankController が衝突 | OnFlagReached イベントが発火する | [ ] |
| U-09 | BattleHUD — HP表示更新 | RunManager.OnHpChanged(75, 100) | HPバーが 75% で表示される | [ ] |
| U-10 | BattleHUD — スクラップ表示更新 | RunManager.OnScrapChanged(200) | スクラップ表示が「200」になる | [ ] |

## インテグレーションテスト

| ID | テスト名 | 前提条件 | 手順 | 期待結果 | ステータス |
|----|---------|---------|------|---------|-----------|
| I-01 | 旗到達 → クリア遷移 | BattleScene に旗と戦車を配置 | 戦車を旗の位置まで移動 | BattleSceneController がクリア演出を実行し MapScene へ遷移する | [ ] |
| I-02 | HP0 → ゲームオーバー遷移 | BattleScene で HP=10 の状態 | 敵弾を受けて HP を 0 にする | BattleSceneController がゲームオーバー演出を実行し GameOverScene へ遷移する | [ ] |
| I-03 | グリッドレイアウト反映 | RunManager に 3×3 グリッドと配置済みタワーを設定 | BattleScene を起動 | タワースロットが GridLayout 通りに生成されタワーが攻撃を開始する | [ ] |
| I-04 | スクラップ回収 → HUD 更新 | BattleScene に ScrapObject と戦車を配置 | 戦車で ScrapObject を回収する | RunManager.CurrentScrap が増加し BattleHUD の表示が更新される | [ ] |

## E2Eテスト

| ID | テスト名 | 前提条件 | 手順 | 期待結果 | ステータス |
|----|---------|---------|------|---------|-----------|
| E-01 | 準備 → 戦闘 → クリア | PreprationScene でタワーを配置して出撃 | WASD で旗へ移動しながらタワーで敵を倒す | 旗に到達するとクリア演出が流れ MapScene へ戻る | [ ] |
| E-02 | 準備 → 戦闘 → ゲームオーバー | HP1 の状態で BattleScene を開始 | 敵弾を受ける | HP0 になりゲームオーバー画面へ遷移する | [ ] |
| E-03 | ボスノードクリア → 次エリア | ボスノードで BattleScene を開始 | ボスを撃破する | RunManager.StartNewArea が呼ばれ次エリアの MapScene へ遷移する | [ ] |

## ステータス凡例

- `[ ]` 未実施
- `[~]` 実施中
- `[x]` 合格
- `[!]` 不合格
