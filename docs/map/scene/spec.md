# MapScene 仕様

## 機能一覧

| # | 機能名 | 説明 |
|---|--------|------|
| 1 | ノードグラフ描画 | MapGraph の全ノードをアイコン付きボタンとして動的配置し、接続線を描画する |
| 2 | ノード状態表示 | 各ノードの状態（未訪問・到達可能・現在地・訪問済み）を視覚的に区別する |
| 3 | ノード選択 | 到達可能ノードのみクリック受付。MapSceneController に通知する |
| 4 | シーン遷移 | 選択ノード種別に応じて適切なシーンへ遷移する |
| 5 | MapHUD 表示 | エリア番号・所持スクラップ数を常時表示する |

## ノード状態の表示仕様

| 状態 | 表示 |
|------|------|
| Unvisited（到達不可） | グレーアウト、クリック無効 |
| Reachable（到達可能） | ハイライト（点滅アニメーション）、クリック有効 |
| Current（現在地） | 戦車アイコンを重ねて表示 |
| Visited（訪問済み） | 暗くなったアイコン、クリック無効 |

## ノード種別と遷移先

| 種別 | 遷移先 |
|------|--------|
| Battle | PreparationScene（通常バトルフラグ） |
| Boss | PreparationScene（ボスフラグ） |
| Shop | ShopScene |
| Refit | RefitScene |
| Mystery | MapScene 上にハテナイベント UI をオーバーレイ表示 |

## シーン構造

```
MapScene
  ├── [System]
  │     └── MapSceneController (MonoBehaviour)
  │
  ├── [Map]
  │     └── MapGraphView (MonoBehaviour)
  │           ├── EdgeRenderer × n (LineRenderer)
  │           └── NodeButton × n (Prefab)
  │                 ├── NodeButtonUI (MonoBehaviour)
  │                 ├── Image (アイコン)
  │                 └── TMP_Text (ラベル)
  │
  └── [HUD]
        └── MapHUD
              ├── AreaLabel (TMP_Text)
              └── ScrapText (TMP_Text)
```

## エラー / 異常ケース

| 条件 | 挙動 |
|------|------|
| 到達不可ノードをクリック | イベントを無視する（ボタンは非インタラクティブ） |
| RunManager の MapGraph が null | エラーログを出力し、マップ生成を再試行する |

## 未対応ケース

- マップのパン移動・ズーム
- ノード間移動のスプラインアニメーション
