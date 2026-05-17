# デバッグシステム 仕様

## 機能一覧

| # | 機能名 | 説明 |
|---|--------|------|
| 1 | 構造化ログ出力 | タグ・タイムスタンプ付きのログを Unity コンソールに出力する |
| 2 | ログプール | カスタムログと Unity の標準ログを時系列順にメモリへ蓄積する |
| 3 | ログファイル出力 | プールしたログをテキストファイルとしてプロジェクトルートに書き出す |
| 4 | デバッグメニュー | 全シーン共通の開発用オーバーレイメニューを提供する |
| 5 | リリース除外 | エディタ・Development Build 以外のビルドからすべての機能を除外する |

---

## Log クラス

### 呼び出し形式

```csharp
Log.Info("攻撃した", LogTag.Battle, LogTag.Player);
Log.Warning("ターゲット未検出", LogTag.Tower);
Log.Error("GridLayout が null", LogTag.System);
```

### 出力フォーマット

```
[HH:mm:ss.fff][LEVEL][Tag1][Tag2] メッセージ
```

### API

| メソッド | レベル | Unity出力 |
|---------|--------|---------|
| `Log.Info(msg, params LogTag[])` | INFO | `Debug.Log` |
| `Log.Warning(msg, params LogTag[])` | WARN | `Debug.LogWarning` |
| `Log.Error(msg, params LogTag[])` | ERROR | `Debug.LogError` |

### ビルド除外

`[Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]` を全メソッドに付与。リリースビルドでは呼び出し箇所がコンパイル時に自動削除されるため実行時オーバーヘッドなし。

---

## LogTag 定義

| タグ | 用途 |
|------|------|
| `System` | 起動・終了・シーン遷移 |
| `Battle` | バトルシーン全般 |
| `Player` | 戦車・プレイヤー操作 |
| `Enemy` | 敵AI・スポーン |
| `Tower` | タワー攻撃・初期化 |
| `Map` | マップ生成・ノード選択 |
| `Economy` | スクラップ・ショップ・改修 |
| `UI` | UI操作・表示切り替え |
| `Save` | セーブ・ロード |
| `Debug` | デバッグ専用の一時ログ |

---

## GameLogger クラス

### ログプール

- `Log.Info` 等の出力を内部リストに蓄積する
- `Application.logMessageReceived` にフックして Unity の `Debug.Log` 等も自動収集する
- ゲーム起動時（`RuntimeInitializeOnLoadMethod`）にフックを登録する

### ファイル出力仕様

| 項目 | 内容 |
|------|------|
| 出力先 | プロジェクトルート（`Application.dataPath/../`） |
| ファイル名 | `debug_log_YYYYMMDD_HHmmss.txt` |
| エンコード | UTF-8 |
| 内容 | ヘッダ + 全エントリを時系列で改行区切り |

出力例：
```
=== GearBox Debug Log  2026-05-17 14:32:00 ===
Total entries: 42

[14:32:00.001][UNITY_LOG] LogoScene loaded
[14:32:05.123][INFO][Battle][Player] タワースポーン完了: 1体
[14:32:07.200][INFO][Tower][Battle] 蒸気砲 攻撃実行
```

### API

| メソッド / プロパティ | 説明 |
|---------------------|------|
| `GameLogger.DumpToFile()` | プール内容をファイルに書き出す |
| `GameLogger.Clear()` | プールを空にする |
| `GameLogger.EntryCount` | 現在のプールエントリ数を返す |

---

## DebugMenuController

### 起動

`[RuntimeInitializeOnLoadMethod(AfterSceneLoad)]` により自動生成。`DontDestroyOnLoad` で全シーンに存在し続ける。

### 操作方法

| 入力 | アクション |
|------|-----------|
| 右上 60×60px エリアをタップ | デバッグメニューを開く |
| メニュー外（ブロッカー）をタップ | デバッグメニューを閉じる |
| 「閉じる」ボタン | デバッグメニューを閉じる |
| 「ログをファイルに出力」ボタン | `GameLogger.DumpToFile()` を実行 |
| 「ログをクリア」ボタン | `GameLogger.Clear()` を実行 |

### 入力ブロック

メニュー展開中は全画面ブロッカーが背面のタッチを遮断する。

### ビルド除外

クラス全体を `#if UNITY_EDITOR || DEVELOPMENT_BUILD` で囲む。

## 未対応ケース

- ネットワーク越しのリモートロギング
- ログの検索・フィルタリング UI
- タイムスケール変更・シーン直接遷移などの追加デバッグ操作（将来対応）
