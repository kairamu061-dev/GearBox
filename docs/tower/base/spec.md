# タワー定義・攻撃挙動 仕様

## 機能一覧

| # | 機能名 | 説明 |
|---|--------|------|
| 1 | TowerData 定義 | ScriptableObject でタワーのすべてのパラメータを定義する |
| 2 | CT管理 | TowerBehaviour がフレームごとにCTをデクリメントし、0で攻撃を発動する |
| 3 | 照準型攻撃 | マウスカーソルのワールド座標方向へ弾を発射する |
| 4 | 自動追尾型攻撃 | 射程内の敵を自動検出し、その方向へ弾を発射する |
| 5 | 範囲型攻撃 | 自機周囲の指定半径内の全敵にダメージを与える |
| 6 | 設置型攻撃 | 自機前方にオブジェクト（地雷等）を設置する |
| 7 | 弾丸管理 | ObjectPool で弾丸の生成・返却を管理する |

## 操作方法

（プレイヤー操作なし。タワーはすべて自動動作）

## TowerData ScriptableObject フィールド

| フィールド | 型 | 説明 |
|-----------|---|------|
| towerName | string | タワーの表示名 |
| description | string | 説明文 |
| icon | Sprite | タワーアイコン |
| prefab | GameObject | タワーのPrefab参照 |
| attackType | AttackType | 照準型/自動追尾型/範囲型/設置型 |
| damage | float | 1回の攻撃ダメージ |
| cooldown | float | 攻撃CT（秒） |
| range | float | 攻撃射程（ユニット）|
| size | Vector2Int | グリッド占有サイズ（例: 1,1 / 2,1） |
| shape | bool[] | サイズ×サイズのマス占有マップ（L字形など対応） |
| projectilePrefab | GameObject | 弾丸Prefab（照準・追尾型のみ）|
| poolSize | int | 弾丸プールの初期サイズ |

## 攻撃タイプ別動作仕様

各攻撃タイプの詳細はそれぞれのサブ項目を参照。

| 攻撃タイプ | サブ項目 | クラス |
|-----------|---------|--------|
| 照準型（Aimed） | [tower/aimed](../aimed/spec.md) | `AimedAttack` / `Projectile` |
| 自動追尾型（AutoAim） | [tower/autoaim](../autoaim/spec.md) | `AutoAimAttack` |
| 範囲型（Area） | [tower/area](../area/spec.md) | `AreaAttack` |
| 設置型（Placement） | [tower/placement](../placement/spec.md) | `PlacementAttack` / `PlacementObject` |

## エラー / 異常ケース

| 条件 | 挙動 |
|------|------|
| プールが枯渇した | 弾丸を新規生成する（プールを拡張）→ 各攻撃タイプサブ項目参照 |
| `TowerBehaviour.Initialize()` 呼び出し前に攻撃が発動 | `data` が null のため攻撃をスキップ |

## 未対応ケース

- タワーの向きアニメーション（照準方向への回転）は将来対応
- タワーの耐久値・破壊
- 貫通弾・跳弾
