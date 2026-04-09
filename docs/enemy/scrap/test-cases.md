# スクラップドロップ テストケース

## 単体テスト

| # | テスト名 | 前提条件 | 操作 | 期待結果 |
|---|----------|----------|------|----------|
| U1 | Drop で指定範囲内の個数だけ散乱する | ScrapDropper 初期化済み・data.scrapDropMin=2, Max=5 | `Drop()` を10回呼ぶ | 各回の生成個数が 2〜5 の範囲内 |
| U2 | 散乱位置が半径 1.5 ユニット以内 | ScrapDropper 初期化済み・deathPosition = (0, 0) | `Drop()` を呼ぶ | 全 ScrapObject の位置が origin から 1.5 ユニット以内 |
| U3 | 戦車接触で AddScrap が呼ばれる | ScrapObject 配置済み | ScrapCollector コライダーと接触 | `RunManager.AddScrap(1)` が呼ばれ ScrapObject が返却される |
| U4 | 60 秒タイマーで自動返却 | ScrapObject 配置済み | 60 秒経過 | ScrapObject が返却される（AddScrap は呼ばれない） |
| U5 | 消滅フラグで二重処理なし | ScrapObject 配置済み | 同フレームで2接触イベント | `AddScrap` が1回のみ呼ばれる |

## 統合テスト

| # | テスト名 | 前提条件 | 操作 | 期待結果 |
|---|----------|----------|------|----------|
| I1 | 敵死亡 → ScrapDropper → ScrapObject → 回収の一連の流れ | EnemyController + ScrapDropper 設定済み | `TakeDamage()` で HP を 0 にする | Die() → Drop() → ScrapObject 生成 → 戦車が回収 → Scrap 残高が増加 |

## E2Eテスト

| # | テスト名 | 前提条件 | 操作 | 期待結果 |
|---|----------|----------|------|----------|
| E1 | バトルでスクラップを全回収 | BattleScene 起動 | 全敵を倒して全 ScrapObject に接触する | HUD の Scrap 表示が合計ドロップ数と一致する |
