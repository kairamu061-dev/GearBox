using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class RunSaveData
{
    public int currentHp;
    public int maxHp;
    public int scrap;
    public string resumeSceneName;
    // TODO: GridLayout, TowerInventory, MapGraph などの直列化
}

[Serializable]
public class RunHistoryData
{
    public string runId;
    public string result;
    public int reachedArea;
    public string timestamp;
    // TODO: 詳細フィールド（通過ノード・取得ログ等）
}

[Serializable]
class HistoryFile
{
    public List<RunHistoryData> entries = new();
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    static string SavePath => Path.Combine(Application.persistentDataPath, "run_save.json");
    static string HistoryPath => Path.Combine(Application.persistentDataPath, "run_history.json");
    const int MaxHistoryCount = 20;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // --- 中断セーブ ---
    public bool HasSave() => File.Exists(SavePath);

    public void SaveRun(RunSaveData data)
    {
        try { File.WriteAllText(SavePath, JsonUtility.ToJson(data)); }
        catch (Exception e) { Debug.LogError($"SaveRun failed: {e}"); }
    }

    public RunSaveData LoadRun()
    {
        try
        {
            if (!HasSave()) return null;
            return JsonUtility.FromJson<RunSaveData>(File.ReadAllText(SavePath));
        }
        catch (Exception e) { Debug.LogError($"LoadRun failed: {e}"); return null; }
    }

    public void DeleteSave()
    {
        if (File.Exists(SavePath)) File.Delete(SavePath);
    }

    // --- ラン履歴 ---
    public void AppendHistory(RunHistoryData entry)
    {
        var file = LoadHistoryFile();
        file.entries.Insert(0, entry);
        if (file.entries.Count > MaxHistoryCount)
            file.entries.RemoveRange(MaxHistoryCount, file.entries.Count - MaxHistoryCount);
        try { File.WriteAllText(HistoryPath, JsonUtility.ToJson(file)); }
        catch (Exception e) { Debug.LogError($"AppendHistory failed: {e}"); }
    }

    public List<RunHistoryData> LoadHistory() => LoadHistoryFile().entries;

    HistoryFile LoadHistoryFile()
    {
        try
        {
            if (File.Exists(HistoryPath))
                return JsonUtility.FromJson<HistoryFile>(File.ReadAllText(HistoryPath));
        }
        catch (Exception e) { Debug.LogError($"LoadHistory failed: {e}"); }
        return new HistoryFile();
    }
}
