using System;
using System.Collections.Generic;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    // --- 状態フィールド ---
    public int MaxHp { get; private set; } = 100;
    public int CurrentHp { get; private set; } = 100;
    public int Scrap { get; private set; } = 100;
    public Vector2Int GridSize { get; private set; } = new(3, 3);
    public TowerInstance[,] GridLayout { get; private set; }
    public List<TowerInstance> TowerInventory { get; private set; } = new();
    public List<SynthesisRecipe> KnownRecipes { get; private set; } = new();
    public List<RelicData> RelicInventory { get; private set; } = new();
    public MapGraph CurrentMapGraph { get; private set; }
    public int CurrentNodeId { get; private set; } = -1;
    public int CurrentArea { get; private set; } = 1;

    // バトル中の仮保持スクラップ
    public int PendingScrap { get; private set; }

    // --- イベント ---
    public event Action<int, int> OnHpChanged;
    public event Action<int> OnScrapChanged;
    public Action OnGridChanged;

    const int GridMax = 5;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        GridLayout = new TowerInstance[GridSize.x, GridSize.y];
    }

    public void InitNewRun(TowerData startTower)
    {
        MaxHp = 100;
        CurrentHp = 100;
        Scrap = 100;
        GridSize = new Vector2Int(3, 3);
        GridLayout = new TowerInstance[3, 3];
        TowerInventory = new List<TowerInstance>();
        KnownRecipes = new List<SynthesisRecipe>();
        RelicInventory = new List<RelicData>();
        CurrentMapGraph = null;
        CurrentNodeId = -1;
        PendingScrap = 0;

        if (startTower != null)
            AddTower(new TowerInstance(startTower));
    }

    // --- HP ---
    public void TakeDamage(int amount)
    {
        CurrentHp = Mathf.Max(0, CurrentHp - amount);
        OnHpChanged?.Invoke(CurrentHp, MaxHp);
    }

    public void Heal(int amount)
    {
        CurrentHp = Mathf.Min(MaxHp, CurrentHp + amount);
        OnHpChanged?.Invoke(CurrentHp, MaxHp);
    }

    // --- スクラップ ---
    public void AddScrap(int amount)
    {
        Scrap += amount;
        OnScrapChanged?.Invoke(Scrap);
    }

    public bool SpendScrap(int amount)
    {
        if (Scrap < amount) return false;
        Scrap -= amount;
        OnScrapChanged?.Invoke(Scrap);
        return true;
    }

    // バトル中の仮保持
    public void AddPendingScrap(int amount) => PendingScrap += amount;

    public void CommitPendingScrap()
    {
        AddScrap(PendingScrap);
        PendingScrap = 0;
    }

    public void ClearPendingScrap() => PendingScrap = 0;

    // --- グリッド ---
    public void PlaceTower(TowerInstance tower, Vector2Int pos)
    {
        RemoveTower(tower);
        GridLayout[pos.x, pos.y] = tower;
        tower.isOnGrid = true;
        tower.gridPosition = new Vector2IntSerializable(pos.x, pos.y);
    }

    public void UnplaceTower(TowerInstance tower)
    {
        var pos = tower.gridPosition.ToVector2Int();
        if (GridLayout[pos.x, pos.y] == tower)
            GridLayout[pos.x, pos.y] = null;
        tower.isOnGrid = false;
        AddTower(tower);
    }

    public void AddTower(TowerInstance tower) => TowerInventory.Add(tower);

    public void RemoveTower(TowerInstance tower) => TowerInventory.Remove(tower);

    public void ExpandGrid(bool addColumn)
    {
        int newW = GridSize.x + (addColumn ? 1 : 0);
        int newH = GridSize.y + (addColumn ? 0 : 1);
        if (newW > GridMax || newH > GridMax) return;

        var newLayout = new TowerInstance[newW, newH];
        for (int x = 0; x < GridSize.x; x++)
            for (int y = 0; y < GridSize.y; y++)
                newLayout[x, y] = GridLayout[x, y];

        GridLayout = newLayout;
        GridSize = new Vector2Int(newW, newH);
        OnGridChanged?.Invoke();
    }

    // --- レシピ ---
    public void UnlockRecipe(SynthesisRecipe recipe)
    {
        if (!KnownRecipes.Contains(recipe))
            KnownRecipes.Add(recipe);
    }

    // --- レリック ---
    public void AddRelic(RelicData relic)
    {
        RelicInventory.Add(relic);
        RelicEffectApplier.Apply(relic, this);
    }

    // --- マップ進行 ---
    public void StartNewArea(MapGraph graph)
    {
        if (CurrentMapGraph != null) CurrentArea++;
        CurrentMapGraph = graph;
        CurrentNodeId = 0;
    }

    public void CompleteNode(int nodeId)
    {
        var node = CurrentMapGraph?.GetNode(nodeId);
        if (node == null) return;

        // 選ばなかった Reachable ノードを Unvisited に戻す
        foreach (var n in CurrentMapGraph.nodes)
            if (n.state == NodeState.Reachable)
                n.state = NodeState.Unvisited;

        node.state = NodeState.Visited;
        CurrentNodeId = nodeId;
        CurrentMapGraph.SetReachableFrom(nodeId);
    }

    public void ResetRun()
    {
        InitNewRun(null);
    }
}
