# デバッグシステム 設計

## シーン・オブジェクト構成

デバッグシステムはシーンに依存せず、ランタイムで自動生成される。

```
[DebugMenu]  ← DontDestroyOnLoad・RuntimeInitializeOnLoadMethodで自動生成
  └── DebugCanvas (Canvas / sortingOrder=9999)
        ├── DebugTrigger (Image + Button / 右上 60×60px)
        └── DebugMenuRoot (オーバーレイパネル)
              ├── Blocker (Image + Button / 全画面・背景タップで閉じる)
              └── Panel (メニュー本体)
                    ├── Label（タイトル）
                    ├── StatusText（ログ件数表示）
                    ├── Separator
                    ├── Button「ログをファイルに出力」
                    ├── Button「ログをクリア」
                    └── Button「閉じる」
```

## クラス設計

| クラス名 | 種別 | 責務 |
|---------|------|------|
| `Log` | static class | ログ出力の呼び出し口。[Conditional]で非対応ビルドから除外 |
| `GameLogger` | static class | ログエントリのプール管理・ファイル出力・Unityログフック |
| `LogTag` | enum | ログに付与するタグの定義 |
| `LogEntry` | struct | 1件分のログデータ（タイムスタンプ・メッセージ・タグ・レベル） |
| `DebugMenuController` | MonoBehaviour | デバッグメニューUIの生成・開閉・ボタンイベント処理 |

## データ構造

```csharp
public struct LogEntry
{
    public DateTime  timestamp;
    public string    message;
    public LogTag[]  tags;
    public string    level;  // "INFO" / "WARN" / "ERROR" / "UNITY_LOG" 等

    public string Format();  // [HH:mm:ss.fff][LEVEL][Tag] message 形式で返す
}
```

## インターフェース / イベント

```csharp
// Unity ログフック（GameLogger 内）
Application.logMessageReceived += OnUnityLog;

// DebugMenuController のランタイム自動生成
[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
static void AutoSpawn();
```

## ビルド除外の仕組み

| 手法 | 対象 | 効果 |
|------|------|------|
| `[Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]` | `Log` クラスの全メソッド | 呼び出し箇所がコンパイル時に削除される |
| `#if UNITY_EDITOR \|\| DEVELOPMENT_BUILD` | `DebugMenuController` クラス全体 | クラス自体がリリースバイナリに含まれない |
| `#if UNITY_EDITOR \|\| DEVELOPMENT_BUILD` | `GameLogger` の初期化・フック処理 | リリース時はプールもフックも動作しない |

## 依存関係

| 依存先 | 用途 |
|--------|------|
| `UnityEngine.UI` | デバッグメニューのCanvas・Button |
| `TMPro` | メニュー内のテキスト表示 |
| `System.IO` | ログファイルの書き出し |
| `Application.logMessageReceived` | Unityログのフック |
