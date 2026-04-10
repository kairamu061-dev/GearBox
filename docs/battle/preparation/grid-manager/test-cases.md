# GridManager テストケース

## 単体テスト

| # | テスト名 | 前提条件 | 操作 | 期待結果 |
|---|----------|----------|------|----------|
| U1 | 空グリッドへの配置可能判定 | 3×3グリッド・全マス空 | `CanPlace(1×1タワー, (0,0), grid)` | true |
| U2 | 占有済みマスへの配置不可判定 | (1,1) に既存タワー | `CanPlace(1×1タワー, (1,1), grid)` | false |
| U3 | グリッド外への配置不可判定 | 3×3グリッド | `CanPlace(1×1タワー, (3,0), grid)` | false |
| U4 | 2×1タワーの範囲チェック | (2,0) に配置・グリッド幅3 | `CanPlace(2×1タワー, (2,0), grid)` | false（x=3が範囲外） |
| U5 | Place でGridLayout が更新される | 3×3グリッド・全マス空 | `Place(1×1タワーA, (1,1))` | GridLayout[1,1] == タワーA |
| U6 | Remove で占有マスが null になる | (0,0)〜(1,0) に2×1タワーA | `Remove((0,0))` | GridLayout[0,0] == null かつ GridLayout[1,0] == null |
| U7 | Remove で任意の占有マスから削除できる | (0,0)〜(1,0) に2×1タワーA | `Remove((1,0))` | GridLayout[0,0] == null かつ GridLayout[1,0] == null |
| U8 | Remove で null マスを指定しても例外なし | 全マス空 | `Remove((0,0))` | 何も起きない（return） |
