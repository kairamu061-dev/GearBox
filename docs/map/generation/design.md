# マップ生成 設計

## Unityパッケージ / アセット

なし（純粋 C# クラスのみ）

## クラス設計

| クラス名 | 種別 | 責務 |
|---------|------|------|
| `NodeType` | enum | ノード種別の列挙（Battle / Mystery / Shop / Refit / Boss） |
| `NodeState` | enum | ノード状態の列挙（Unvisited / Reachable / Current / Visited） |
| `MapNode` | plain C# | ノード1件のデータ。id・type・position・nextNodeIds・state を保持 |
| `MapGraph` | plain C# | ノードリストと currentNodeId を保持。`GetNode` / `GetReachableNodes` API |
| `MapGenerator` | plain C# (static) | `Generate(seed?)` で `MapGraph` を返す。生成・検証・リトライをカプセル化 |

## データ構造

```csharp
public enum NodeType  { Battle, Mystery, Shop, Refit, Boss }
public enum NodeState { Unvisited, Reachable, Current, Visited }

[System.Serializable]
public class MapNode
{
    public int       id;
    public NodeType  type;
    public Vector2   position;      // 正規化座標 (0–1)
    public List<int> nextNodeIds;
    public NodeState state;
}

[System.Serializable]
public class MapGraph
{
    public List<MapNode> nodes;
    public int           currentNodeId;

    public MapNode       GetNode(int id);
    public List<MapNode> GetReachableNodes();
}

public static class MapGenerator
{
    // seed=null でランダム、指定でリプレイ可能
    public static MapGraph Generate(int? seed = null);

    private static bool ValidateConnectivity(MapGraph graph); // BFS で孤立チェック
    private static void AssignPositions(MapGraph graph);      // 正規化座標を付与
}
```

## 生成戦略

- 層ごとに独立して処理することで、前層の接続が後層に影響しない設計
- 接続は「前層ノード → 次層ノードの一部」をランダムに選択。最低1本は保証
- `ValidateConnectivity` は BFS で全ノードを探索し、未到達ノードがあれば `false` を返す

## 依存関係

なし（RunManager や MonoBehaviour に依存しない）

> RunManager が `MapGraph` を保持するが、生成クラス自体は RunManager を参照しない。
> 生成された `MapGraph` を呼び出し側（RunManager / MapSceneController）が保存する。
