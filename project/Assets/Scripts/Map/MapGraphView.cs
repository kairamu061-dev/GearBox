using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGraphView : MonoBehaviour
{
    [SerializeField] NodeButtonUI nodeButtonPrefab;
    [SerializeField] RectTransform mapContainer;
    [SerializeField] GameObject edgePrefab;

    readonly List<NodeButtonUI> nodeButtons = new();
    readonly List<GameObject> edges = new();

    public System.Action<MapNode> OnNodeSelected;

    public void Build(MapGraph graph)
    {
        Clear();
        foreach (var node in graph.nodes)
        {
            var btn = Instantiate(nodeButtonPrefab, mapContainer);
            var rect = mapContainer.rect;
            btn.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(node.position.x * rect.width - rect.width * 0.5f,
                            node.position.y * rect.height - rect.height * 0.5f);
            btn.Setup(node);
            btn.OnSelected += n => OnNodeSelected?.Invoke(n);
            nodeButtons.Add(btn);
        }
        DrawEdges(graph);
    }

    public void Refresh(MapGraph graph)
    {
        foreach (var btn in nodeButtons)
            btn.Setup(graph.GetNode(btn.Node.id));
    }

    void DrawEdges(MapGraph graph)
    {
        foreach (var node in graph.nodes)
        {
            foreach (var nextId in node.nextNodeIds)
            {
                var next = graph.GetNode(nextId);
                if (next == null) continue;
                if (edgePrefab == null) continue;

                var edge = Instantiate(edgePrefab, mapContainer);
                var line = edge.GetComponent<LineRenderer>();
                if (line != null)
                {
                    var from = GetWorldPos(node.position);
                    var to = GetWorldPos(next.position);
                    line.SetPosition(0, from);
                    line.SetPosition(1, to);
                }
                edges.Add(edge);
            }
        }
    }

    Vector3 GetWorldPos(Vector2 normalizedPos)
    {
        var rect = mapContainer.rect;
        return mapContainer.TransformPoint(new Vector3(
            normalizedPos.x * rect.width - rect.width * 0.5f,
            normalizedPos.y * rect.height - rect.height * 0.5f, 0f));
    }

    void Clear()
    {
        foreach (var b in nodeButtons) Destroy(b.gameObject);
        nodeButtons.Clear();
        foreach (var e in edges) Destroy(e);
        edges.Clear();
    }
}
