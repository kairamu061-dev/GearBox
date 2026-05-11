using System;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType { Battle, Mystery, Shop, Refit, Boss }

public enum NodeState { Unvisited, Reachable, Current, Visited }

[Serializable]
public class MapNode
{
    public int id;
    public NodeType type;
    public Vector2 position;
    public List<int> nextNodeIds = new();
    public NodeState state;
}

[Serializable]
public class MapGraph
{
    public List<MapNode> nodes = new();
    public int currentNodeId = -1;

    public MapNode GetNode(int id) => nodes.Find(n => n.id == id);

    public List<MapNode> GetReachableNodes()
    {
        var result = new List<MapNode>();
        foreach (var node in nodes)
            if (node.state == NodeState.Reachable)
                result.Add(node);
        return result;
    }

    public void SetReachableFrom(int nodeId)
    {
        var current = GetNode(nodeId);
        if (current == null) return;
        foreach (var nextId in current.nextNodeIds)
        {
            var next = GetNode(nextId);
            if (next != null && next.state == NodeState.Unvisited)
                next.state = NodeState.Reachable;
        }
    }
}
