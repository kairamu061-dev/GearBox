# デバッグシステム 仕様

---

## 1. Log（呼び出し口）

### 呼び出し方法

```csharp
Log.Info("攻撃した", LogTag.Battle, LogTag.Player);
Log.Warning("ターゲットが見つからない", LogTag.Tower);
Log.Error("GridLayout が null", LogTag.System);
```

### 出力フォーマット

```
[HH:mm:ss.fff][LEVEL][Tag1][Tag2] メッセージ
```

例：
```
[14:32:05.123][INFO][Battle][Player] 攻撃した
[14:32:05.200][WARN][Tower] ターゲットが見つからない
```

### API

| メソッド | 説明 |
|---------|------|
| `Log.Info(message, params LogTag[] tags)` | 情報ログ。Unity コンソールに `Debug.Log` で出力 |
| `Log.Warning(message, params LogTag[] tags)` | 警告ログ。`Debug.LogWarning` で出力 |
| `Log.Error(message, params LogTag[] tags)` | エラーログ。`Debug.LogError` で出力 |

### ビルド除外

`[Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]` を付与しているため、リリースビルドでは呼び出し箇所がコンパイル時に自動削除される。

---

## 2. LogTag（タグ定義）

| タグ | 用途 |
|------|------|
| `System` | ゲーム全体の起動・終了・シーン遷移 |
| `Battle` | バトルシーン全般 |
| `Player` | 戦車・プレイヤー操作 |
| `Enemy` | 敵AI・スポーン |
| `Tower` | タワー攻撃・初期化 |
| `Map` | マップ生成・ノード選択 |
| `Economy` | スクラップ・ショップ・改修 |
| `UI` | UI操作・表示切り替え |
| `Save` | セーブ・ロード |
| `Debug` | デバッグ専用の一時ログ |

タグは複数指定可能。

---

## 3. GameLogger（プール・ファイル出力）

### ログのプール

- `Log.Info` 等で出力したログをエントリとしてプールに追加する
- `Application.logMessageReceived` にフックして Unity の `Debug.Log` 等も自動的にプールに追加する
- プールはメモリ上に保持し、ゲーム終了まで累積する

### ファイル出力

`GameLogger.DumpToFile()` を呼ぶと、プール内の全エントリを時系列順でテキストファイルに書き出す。

| 項目 | 内容 |
|------|------|
| 出力先 | プロジェクトルート（`Application.dataPath/../`） |
| ファイル名 | `debug_log_YYYYMMDD_HHmmss.txt` |
| エンコード | UTF-8 |

出力例：
```
=== GearBox Debug Log  2026-05-17 14:32:00 ===
Total entries: 42

[14:32:00.001][UNITY_LOG] LogoScene loaded
[14:32:05.123][INFO][Battle][Player] タワースポーン完了: 1体
[14:32:07.200][INFO][Tower][Battle] 蒸気砲 攻撃実行
```

### API

| メソッド | 説明 |
|---------|------|
| `GameLogger.DumpToFile()` | プール内容をファイルに書き出す |
| `GameLogger.Clear()` | プールを空にする |
| `GameLogger.EntryCount` | 現在のプールエントリ数 |

---

## 4. DebugMenuController（デバッグメニュー）

### 起動

`RuntimeInitializeOnLoadMethod(AfterSceneLoad)` によりゲーム起動時に自動生成される。`DontDestroyOnLoad` で全シーンに存在し続ける。

### 操作

| 操作 | 動作 |
|------|------|
| 右上の薄いエリア（60×60px）をタップ | デバッグメニューを開く |
| メニュー外をタップ | デバッグメニューを閉じる |
| 「閉じる」ボタン | デバッグメニューを閉じる |
| 「ログをファイルに出力」ボタン | `GameLogger.DumpToFile()` を呼ぶ |
| 「ログをクリア」ボタン | `GameLogger.Clear()` を呼ぶ |

### 入力ブロック

メニュー展開中は全画面ブロッカー（半透明黒）が背面のタッチを遮断する。

### ビルド除外

クラス全体を `#if UNITY_EDITOR || DEVELOPMENT_BUILD` で囲んでいるためリリースビルドには含まれない。

### 将来追加予定のメニュー項目

- RunManager の状態表示（HP・スクラップ・グリッドなど）
- シーン直接遷移
- タイムスケール変更
- 敵・タワーの即時スポーン

---

## 5. バグチケット

### 場所

`Debug/tickets/BUG-XXX.md`

### フォーマット

```markdown
---
id: BUG-XXX
title: タイトル
tags: [tag1, tag2]
state: Open | In Progress | Fixed | Closed
---

## 不具合内容
## 原因
## 対処
```

### ステート

| ステート | 説明 |
|---------|------|
| Open | 未着手 |
| In Progress | 対応中 |
| Fixed | 修正済み（動作未検証） |
| Closed | 修正確認済み |
