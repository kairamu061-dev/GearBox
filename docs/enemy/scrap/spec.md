# スクラップドロップ 仕様

## 機能一覧

| # | 機能名 | 説明 |
|---|--------|------|
| 1 | スクラップ散乱 | 敵死亡時に scrapDropMin〜Max のランダム個数の ScrapObject を散乱する |
| 2 | 散乱位置計算 | 死亡位置の周囲（半径 1.5 ユニット）にランダム方向・距離で配置する |
| 3 | 戦車回収 | 戦車の ScrapCollector コライダーと接触すると RunManager.AddScrap(1) を呼んで消滅する |
| 4 | 自動消滅 | 生成から 60 秒経過した ScrapObject は自動的に消滅する |

## ScrapDropper 動作仕様

1. `EnemyController.Die()` から `ScrapDropper.Drop(Vector2 deathPosition, EnemyData data)` が呼ばれる
2. `Random.Range(data.scrapDropMin, data.scrapDropMax + 1)` で散乱個数を決定する
3. 個数分ループして、`deathPosition + Random.insideUnitCircle * 1.5f` の位置に ScrapObject を配置する

## ScrapObject 動作仕様

1. `Drop()` 呼び出し後にプールから取得して指定位置に配置・有効化する
2. コルーチンで 60 秒タイマーを開始する
3. 戦車の `ScrapCollector`（CircleCollider2D, isTrigger）と接触すると `OnTriggerEnter2D` が発火する
4. `RunManager.AddScrap(1)` を呼び出してプールへ返却する
5. 60 秒タイマー到達時もプールへ返却する（`AddScrap` は呼ばない）

## エラー / 異常ケース

| 条件 | 挙動 |
|------|------|
| 散乱位置が障害物の内部になる | 位置補正なし（物理的に押し出される or 上に乗る） |
| 既に消滅タイマーが動いている ScrapObject に接触 | 消滅フラグで二重処理を防ぐ |

## 未対応ケース

- スクラップ1個の価値変動（現仕様は常に1固定）
- 磁石アイテムによる回収範囲拡大
