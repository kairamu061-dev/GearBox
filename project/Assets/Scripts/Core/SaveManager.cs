using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// ─── セーブデータ ───────────────────────────────────
[Serializable]
public class RunSaveData
{
    public int currentHp;
    public int maxHp;
    public int scrap;
    public string resumeSceneName;
    public int currentNodeId;
    public int gridSizeX;
    public int gridSizeY;
    public List<SavedTower> towerInventory = new();
    public List<SavedTower> gridLayout     = new();  // 配置済みタワーと座標
    public List<string> knownRecipeIds     = new();
    public List<string> relicIds           = new();
    public SerializedMapGraph mapGraph;
}

[Serializable]
public class SavedTower
{
    public string towerId;
    public int level;
    public int gridX;
    public int gridY;
    public bool isOnGrid;
}

[Serializable]
public class SerializedMapGraph
{
    public List<SerializedNode> nodes = new();
    public int currentNodeId;
}

[Serializable]
public class SerializedNode
{
    public int id;
    public NodeType type;
    public float posX;
    public float posY;
    public List<int> nextNodeIds = new();
    public NodeState state;
}

// ─── ラン履歴 ────────────────────────────────────────
[Serializable]
public class RunHistoryData
{
    public string runId;
    public string result;
    public int reachedArea;
    public string timestamp;
    public List<string> visitedNodeTypes = new();
}

[Serializable]
class HistoryFile { public List<RunHistoryData> entries = new(); }

// ─── SaveManager ─────────────────────────────────────
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    static string SavePath    => Path.Combine(Application.persistentDataPath, "run_save.json");
    static string HistoryPath => Path.Combine(Application.persistentDataPath, "run_history.json");
    const int MaxHistoryCount = 20;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ─── 中断セーブ ─────────────────────────────────
    public bool HasSave() => File.Exists(SavePath);

    public void SaveRun(string resumeScene)
    {
        var rm = RunManager.Instance;
        var data = new RunSaveData
        {
            currentHp      = rm.CurrentHp,
            maxHp          = rm.MaxHp,
            scrap          = rm.Scrap,
            resumeSceneName = resumeScene,
            currentNodeId  = rm.CurrentNodeId,
            gridSizeX      = rm.GridSize.x,
            gridSizeY      = rm.GridSize.y,
        };

        // 手持ちタワー
        foreach (var t in rm.TowerInventory)
            data.towerInventory.Add(new SavedTower
            {
                towerId = t.data.towerId, level = t.level, isOnGrid = false
            });

        // グリッド配置タワー
        for (int x = 0; x < rm.GridSize.x; x++)
            for (int y = 0; y < rm.GridSize.y; y++)
            {
                var t = rm.GridLayout[x, y];
                if (t == null) continue;
                data.gridLayout.Add(new SavedTower
                {
                    towerId = t.data.towerId, level = t.level,
                    gridX = x, gridY = y, isOnGrid = true
                });
            }

        // 合成レシピID
        foreach (var r in rm.KnownRecipes)
            data.knownRecipeIds.Add($"{r.materialA.towerId}+{r.materialB.towerId}");

        // レリックID
        foreach (var r in rm.RelicInventory)
            data.relicIds.Add(r.relicId);

        // マップグラフ
        if (rm.CurrentMapGraph != null)
        {
            data.mapGraph = new SerializedMapGraph
                { currentNodeId = rm.CurrentNodeId };
            foreach (var n in rm.CurrentMapGraph.nodes)
                data.mapGraph.nodes.Add(new SerializedNode
                {
                    id = n.id, type = n.type, posX = n.position.x,
                    posY = n.position.y, nextNodeIds = n.nextNodeIds, state = n.state
                });
        }

        Write(SavePath, data);
    }

    public RunSaveData LoadRun()
    {
        try
        {
            if (!HasSave()) return null;
            return JsonUtility.FromJson<RunSaveData>(File.ReadAllText(SavePath));
        }
        catch (Exception e) { Debug.LogError($"LoadRun: {e}"); return null; }
    }

    // RunManager にセーブデータを復元する
    public bool RestoreRun(RunSaveData data)
    {
        if (data == null) return false;
        var rm = RunManager.Instance;

        // 基本ステータス
        rm.InitNewRun(null);
        rm.Heal(data.maxHp);
        rm.TakeDamage(data.maxHp - data.currentHp);
        while (rm.Scrap < data.scrap) rm.AddScrap(1);
        while (rm.Scrap > data.scrap) rm.SpendScrap(1);

        // グリッドサイズ
        while (rm.GridSize.x < data.gridSizeX) rm.ExpandGrid(true);
        while (rm.GridSize.y < data.gridSizeY) rm.ExpandGrid(false);

        // タワー復元
        var allTowers = Resources.LoadAll<TowerData>("Towers");
        var towerMap  = new System.Collections.Generic.Dictionary<string, TowerData>();
        foreach (var t in allTowers) towerMap[t.towerId] = t;

        foreach (var st in data.towerInventory)
        {
            if (!towerMap.TryGetValue(st.towerId, out var td)) continue;
            rm.AddTower(new TowerInstance(td) { level = st.level });
        }
        foreach (var st in data.gridLayout)
        {
            if (!towerMap.TryGetValue(st.towerId, out var td)) continue;
            var inst = new TowerInstance(td) { level = st.level };
            rm.AddTower(inst);
            rm.PlaceTower(inst, new Vector2Int(st.gridX, st.gridY));
        }

        // マップグラフ復元
        if (data.mapGraph != null)
        {
            var graph = new MapGraph { currentNodeId = data.mapGraph.currentNodeId };
            foreach (var sn in data.mapGraph.nodes)
                graph.nodes.Add(new MapNode
                {
                    id = sn.id, type = sn.type,
                    position = new Vector2(sn.posX, sn.posY),
                    nextNodeIds = sn.nextNodeIds, state = sn.state
                });
            rm.StartNewArea(graph);
            rm.CompleteNode(data.currentNodeId); // currentNodeId を反映
        }

        return true;
    }

    public void DeleteSave()
    {
        if (File.Exists(SavePath)) File.Delete(SavePath);
    }

    // ─── ラン履歴 ───────────────────────────────────
    public void AppendHistory(RunHistoryData entry)
    {
        var file = LoadHistoryFile();
        file.entries.Insert(0, entry);
        if (file.entries.Count > MaxHistoryCount)
            file.entries.RemoveRange(MaxHistoryCount, file.entries.Count - MaxHistoryCount);
        Write(HistoryPath, file);
    }

    public List<RunHistoryData> LoadHistory() => LoadHistoryFile().entries;

    HistoryFile LoadHistoryFile()
    {
        try
        {
            if (File.Exists(HistoryPath))
                return JsonUtility.FromJson<HistoryFile>(File.ReadAllText(HistoryPath));
        }
        catch (Exception e) { Debug.LogError($"LoadHistory: {e}"); }
        return new HistoryFile();
    }

    void Write(string path, object obj)
    {
        try { File.WriteAllText(path, JsonUtility.ToJson(obj, true)); }
        catch (Exception e) { Debug.LogError($"Write {path}: {e}"); }
    }
}
