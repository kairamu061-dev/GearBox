using System.Diagnostics;

/// <summary>
/// ゲーム全体のロガー。[Conditional] によりリリースビルドでは呼び出しが自動削除される。
/// 使用例: Log.Info("攻撃", LogTag.Battle, LogTag.Player);
/// </summary>
public static class Log
{
    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void Info(string message, params LogTag[] tags)
    {
        var formatted = Format(message, "INFO", tags);
        GameLogger.AddEntry(message, tags, "INFO");
        UnityEngine.Debug.Log(formatted);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void Warning(string message, params LogTag[] tags)
    {
        var formatted = Format(message, "WARN", tags);
        GameLogger.AddEntry(message, tags, "WARN");
        UnityEngine.Debug.LogWarning(formatted);
    }

    [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
    public static void Error(string message, params LogTag[] tags)
    {
        var formatted = Format(message, "ERROR", tags);
        GameLogger.AddEntry(message, tags, "ERROR");
        UnityEngine.Debug.LogError(formatted);
    }

    static string Format(string message, string level, LogTag[] tags)
    {
        var time   = System.DateTime.Now.ToString("HH:mm:ss.fff");
        var tagStr = tags?.Length > 0 ? "[" + string.Join("][", tags) + "]" : "";
        return $"[{time}][{level}]{tagStr} {message}";
    }
}
