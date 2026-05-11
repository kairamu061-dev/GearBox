using System.Collections.Generic;
using UnityEngine;

public static class MapGenerator
{
    static readonly int[] LayerNodeCounts = { 1, 2, 3 };
    const int MaxBranches = 2;
    const int MaxRetries = 10;

    static readonly float[] TypeWeights = { 50f, 20f, 15f, 15f }; // Battle, Mystery, Shop, Refit
    static readonly NodeType[] Types = { NodeType.Battle, NodeType.Mystery, NodeType.Shop, NodeType.Refit };

    public static MapGraph Generate(int layerCount = 6)
    {
        for (int retry = 0; retry < MaxRetries; retry++)
        {
            var graph = TryGenerate(layerCount);
            if (graph != null) return graph;
        }
        return GenerateFallback(layerCount);
    }

    static MapGraph TryGenerate(int layerCount)
    {
        var graph = new MapGraph();
        int idCounter = 0;

        // スタートノード
        var start = new MapNode { id = idCounter++, type = NodeType.Battle, state = NodeState.Current, position = new Vector2(0.5f, 0f) };
        graph.nodes.Add(start);
        graph.currentNodeId = start.id;

        var prevLayer = new List<MapNode> { start };

        for (int layer = 0; layer < layerCount; layer++)
        {
            bool isFinalLayer = (layer == layerCount - 1);
            int count = isFinalLayer ? 1 : LayerNodeCounts[Random.Range(0, LayerNodeCounts.Length)];
            float yPos = (float)(layer + 1) / (layerCount + 1);

            var currentLayer = new List<MapNode>();
            for (int i = 0; i < count; i++)
            {
                float xPos = count == 1 ? 0.5f : Mathf.Lerp(0.1f, 0.9f, (float)i / (count - 1));
                var type = isFinalLayer ? NodeType.Boss :
                           (layer == 0 ? NodeType.Battle : WeightedRandom(Types, TypeWeights));

                var node = new MapNode { id = idCounter++, type = type, state = NodeState.Unvisited, position = new Vector2(xPos, yPos) };
                graph.nodes.Add(node);
                currentLayer.Add(node);
            }

            // 接続：前層 → 現層（各ノードが最低1本、最大2本）
            foreach (var prev in prevLayer)
            {
                int branchCount = Mathf.Min(MaxBranches, currentLayer.Count);
                var targets = new List<MapNode>(currentLayer);
                Shuffle(targets);
                for (int b = 0; b < branchCount; b++)
                    if (!prev.nextNodeIds.Contains(targets[b].id))
                        prev.nextNodeIds.Add(targets[b].id);
            }

            // 到達不能ノードに接続を追加
            foreach (var cur in currentLayer)
            {
                bool reachable = false;
                foreach (var prev in prevLayer)
                    if (prev.nextNodeIds.Contains(cur.id)) { reachable = true; break; }
                if (!reachable)
                    prevLayer[0].nextNodeIds.Add(cur.id);
            }

            prevLayer = currentLayer;
        }

        // 到達可能性チェック
        if (!AllReachable(graph)) return null;

        // スタートの次層を Reachable に
        graph.SetReachableFrom(start.id);
        return graph;
    }

    static bool AllReachable(MapGraph graph)
    {
        var visited = new HashSet<int>();
        var queue = new Queue<int>();
        queue.Enqueue(0);
        visited.Add(0);
        while (queue.Count > 0)
        {
            var id = queue.Dequeue();
            var node = graph.GetNode(id);
            foreach (var next in node.nextNodeIds)
                if (!visited.Contains(next)) { visited.Add(next); queue.Enqueue(next); }
        }
        return visited.Count == graph.nodes.Count;
    }

    static MapGraph GenerateFallback(int layerCount)
    {
        var graph = new MapGraph();
        int id = 0;
        var prev = new MapNode { id = id++, type = NodeType.Battle, state = NodeState.Current, position = new Vector2(0.5f, 0f) };
        graph.nodes.Add(prev);
        graph.currentNodeId = 0;

        for (int i = 0; i < layerCount; i++)
        {
            var type = (i == layerCount - 1) ? NodeType.Boss : NodeType.Battle;
            float y = (float)(i + 1) / (layerCount + 1);
            var node = new MapNode { id = id++, type = type, state = NodeState.Unvisited, position = new Vector2(0.5f, y) };
            graph.nodes.Add(node);
            prev.nextNodeIds.Add(node.id);
            prev = node;
        }

        graph.SetReachableFrom(0);
        return graph;
    }

    static NodeType WeightedRandom(NodeType[] types, float[] weights)
    {
        float total = 0f;
        foreach (var w in weights) total += w;
        float r = Random.Range(0f, total);
        float acc = 0f;
        for (int i = 0; i < weights.Length; i++) { acc += weights[i]; if (r <= acc) return types[i]; }
        return types[0];
    }

    static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
