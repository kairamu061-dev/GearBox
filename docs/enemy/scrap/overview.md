# スクラップドロップ 概要

## 目的・背景

敵の死亡時にスクラップオブジェクトをフィールドに散乱させ、戦車が近づくと自動回収して RunManager のスクラップ残高に加算する。経済システム（economy/）との接続点となる。

## スコープ

**含む:**
- `ScrapDropper` MonoBehaviour（死亡時にスクラップを散乱）
- `ScrapObject` MonoBehaviour（フィールド上に落ちるスクラップ、60秒タイマー、戦車接触で回収）
- `ScrapObject` Prefab

**含まない:**
- 敵 HP 管理・死亡判定（→ enemy/base/）
- スクラップを消費する UI（→ economy/shop/, economy/refit/）

## 制約

- `ScrapObject` が回収時に自分自身で `RunManager.AddScrap(1)` を呼ぶ
- `ScrapCollector`（battle/combat/）のトリガーで `OnTriggerEnter2D` が発火する
- ObjectPool による再利用を推奨（多数の ScrapObject が同時に出現しうる）

## 完了条件

- 敵死亡時に ScrapDropper が指定個数の ScrapObject を散乱する
- 戦車が接触すると `RunManager.AddScrap()` が呼ばれて ScrapObject が非活性化する
- 60 秒タイマーで回収されなかった ScrapObject が自動消滅する
