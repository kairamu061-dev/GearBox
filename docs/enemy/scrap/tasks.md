# スクラップドロップ タスク

## 実装タスク一覧

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

### ScrapDropper
- [ ] `ScrapDropper` MonoBehaviour の作成
  - [ ] `Drop(deathPosition, data)` — scrapDropMin〜Max の個数分 ScrapObject をプールから取得・散乱
- [ ] `EnemyUnit` Prefab と `BossUnit` Prefab に `ScrapDropper` をアタッチする（enemy/base/design.md 参照）

### ScrapObject
- [ ] `ScrapObject` MonoBehaviour の作成
  - [ ] `Activate(position, pool)` — 位置設定・有効化・60秒タイマー開始
  - [ ] `LifetimeCoroutine()` — 60秒後にプールへ返却
  - [ ] `OnTriggerEnter2D` — ScrapCollector レイヤーに接触で `RunManager.AddScrap(1)` → プール返却
  - [ ] 消滅フラグで二重処理を防ぐ
- [ ] `ScrapObject` Prefab の作成（CircleCollider2D isTrigger + ScrapObject）

### 動作確認
- [ ] 敵を倒すと scrapDropMin〜Max の個数の ScrapObject が散乱することを確認
- [ ] 戦車が ScrapObject に接触すると RunManager.Scrap が加算されることを確認
- [ ] 60 秒経過で ScrapObject が消滅することを確認

## 依存関係

- `RunManager`（core/）→ AddScrap 呼び出しの前提
- `EnemyData`（enemy/base/）→ scrapDrop パラメータの前提
- `ObjectPool<T>`（tower/base/）→ ScrapObject プーリングの前提
- `EnemyController.Die()`（enemy/base/）→ ScrapDropper.Drop() 呼び出し元（Prefab に ScrapDropper をアタッチ）
