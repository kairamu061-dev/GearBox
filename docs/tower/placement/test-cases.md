# 設置型攻撃 テストケース

## 単体テスト

| # | テスト名 | 前提条件 | 操作 | 期待結果 |
|---|----------|----------|------|----------|
| U1 | Execute で前方に PlacementObject が設置される | PlacementAttack 初期化済み | `Execute()` を呼ぶ | 自機前方 1.0 unit に PlacementObject が配置される |
| U2 | 設置上限超過で最古が削除される | maxCount = 3・設置済み3個 | `Execute()` を呼ぶ | 最古の PlacementObject が返却され、新しいものが設置される |
| U3 | 敵接触で TakeDamage と返却 | PlacementObject 設置済み | Enemy コライダーと接触 | `TakeDamage()` 呼び出し後 `ReturnToPool()` |
| U4 | 寿命 10 秒で自動返却 | PlacementObject 設置済み | 10 秒経過 | `ReturnToPool()` が呼ばれる |

## 統合テスト

| # | テスト名 | 前提条件 | 操作 | 期待結果 |
|---|----------|----------|------|----------|
| I1 | 上限管理とプール返却の一貫性 | maxCount = 3・CT が速い設定 | `Execute()` を5回連続呼ぶ | アクティブな PlacementObject が常に3個以下に保たれる |

## E2Eテスト

| # | テスト名 | 前提条件 | 操作 | 期待結果 |
|---|----------|----------|------|----------|
| E1 | 地雷が敵を撃破する | BattleScene 起動・設置型タワー配置済み | 敵が地雷に向かって進む | 地雷に接触した敵の HP が減少し、地雷が消える |
