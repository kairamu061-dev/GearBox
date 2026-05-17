using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public struct LogEntry
{
    public DateTime  timestamp;
    public string    message;
    public LogTag[]  tags;
    public string    level; // INFO / WARN / ERROR / UNITY_LOG / UNITY_WARN / UNITY_ERROR

    public string Format()
    {
        var time   = timestamp.ToString("HH:mm:ss.fff");
        var tagStr = tags?.Length > 0 ? "[" + string.Join("][", tags) + "]" : "";
        return $"[{time}][{level}]{tagStr} {message}";
    }
}

/// <summary>
/// ログのプール管理と一括ファイル出力を担う。
/// リリースビルドでは RuntimeInitializeOnLoadMethod が呼ばれないため実質無効。
/// </summary>
public static class GameLogger
{
    static readonly List<LogEntry> pool = new();
    static bool initialized;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        if (initialized) return;
        initialized = true;
        Application.logMessageReceived += OnUnityLog;
        pool.Clear();
    }

    static void OnUnityLog(string condition, string stackTrace, LogType type)
    {
        // Unity 標準の Debug.Log 等もプールに追加
        string level = type switch
        {
            LogType.Warning   => "UNITY_WARN",
            LogType.Error     => "UNITY_ERROR",
            LogType.Exception => "UNITY_ERROR",
            _                 => "UNITY_LOG",
        };
        pool.Add(new LogEntry
        {
            timestamp = DateTime.Now,
            message   = condition,
            tags      = null,
            level     = level,
        });
    }
#endif

    public static void AddEntry(string message, LogTag[] tags, string level)
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        pool.Add(new LogEntry
        {
            timestamp = DateTime.Now,
            message   = message,
            tags      = tags,
            level     = level,
        });
#endif
    }

    public static void DumpToFile()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        var sb = new StringBuilder();
        sb.AppendLine($"=== GearBox Debug Log  {DateTime.Now:yyyy-MM-dd HH:mm:ss} ===");
        sb.AppendLine($"Total entries: {pool.Count}");
        sb.AppendLine();
        foreach (var e in pool)
            sb.AppendLine(e.Format());

        // プロジェクトルートに出力
        var dir  = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        var path = Path.Combine(dir, $"debug_log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");
        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        Debug.Log($"[GameLogger] ログ出力完了: {path}");
#endif
    }

    public static int EntryCount => pool.Count;

    public static void Clear()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        pool.Clear();
        Debug.Log("[GameLogger] ログをクリアしました");
#endif
    }
}
