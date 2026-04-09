# コア（RunManager） テストケース

## ユニットテスト

| ID | テスト名 | 入力 / 条件 | 期待結果 | ステータス |
|----|---------|------------|---------|-----------|
| U-01 | シングルトン保持 | 2つのシーンを跨いで Instance を参照 | 同一インスタンスが返る | [ ] |
| U-02 | AddScrap | Scrap=50 で AddScrap(30) | Scrap = 80 / OnScrapChanged(80) 発火 | [ ] |
| U-03 | SpendScrap — 残高あり | Scrap=100 で SpendScrap(60) | true / Scrap = 40 / OnScrapChanged(40) 発火 | [ ] |
| U-04 | SpendScrap — 残高不足 | Scrap=30 で SpendScrap(60) | false / Scrap 変化なし / イベント非発火 | [ ] |
| U-05 | TakeDamage — 通常 | HP=100 で TakeDamage(25) | CurrentHp=75 / OnHpChanged(75,100) 発火 | [ ] |
| U-06 | TakeDamage — 0止め | HP=10 で TakeDamage(50) | CurrentHp=0（マイナスにならない） | [ ] |
| U-07 | Heal — 通常 | HP=50 MaxHp=100 で Heal(30) | CurrentHp=80 | [ ] |
| U-08 | Heal — MaxHp 上限 | HP=90 MaxHp=100 で Heal(30) | CurrentHp=100（超えない） | [ ] |
| U-09 | ExpandGrid — 列追加 | GridSize=(3,3) で ExpandGrid(true) | GridSize=(4,3) / GridLayout が 4×3 に拡張 | [ ] |
| U-10 | ExpandGrid — 上限 | GridSize=(5,3) で ExpandGrid(true) | GridSize 変化なし | [ ] |
| U-11 | AddTower / RemoveTower | TowerInstance を追加後に除去 | TowerInventory が 0 件に戻る | [ ] |
| U-12 | UnlockRecipe — 重複無視 | 同一レシピを2回 UnlockRecipe | KnownRecipes に1件のみ存在 | [ ] |

## インテグレーションテスト

| ID | テスト名 | 前提条件 | 手順 | 期待結果 | ステータス |
|----|---------|---------|------|---------|-----------|
| I-01 | OnHpChanged を BattleHUD が受信 | BattleHUD が OnHpChanged を購読 | TakeDamage(20) | BattleHUD の HP バーが更新される | [ ] |
| I-02 | OnScrapChanged を BattleHUD が受信 | BattleHUD が OnScrapChanged を購読 | AddScrap(50) | BattleHUD のスクラップ表示が更新される | [ ] |
| I-03 | CompleteNode がマップに反映 | MapGraph が初期化されている | CompleteNode(nodeId) | MapGraph の対象ノードが Visited になる | [ ] |

## E2Eテスト

| ID | テスト名 | 前提条件 | 手順 | 期待結果 | ステータス |
|----|---------|---------|------|---------|-----------|
| E-01 | シーン遷移後の状態保持 | RunManager.Scrap=120 の状態で MapScene にいる | バトルシーンへ遷移し戻る | MapScene に戻った後も Scrap=120 が維持されている | [ ] |
| E-02 | ラン通じたスクラップ増減 | 新規ラン開始（Scrap=0） | バトルで100回収 → ショップで40消費 | Scrap=60 が全シーンで正しく反映されている | [ ] |

## ステータス凡例

- `[ ]` 未実施
- `[~]` 実施中
- `[x]` 合格
- `[!]` 不合格
