# マップ・ノードシステム 設計

> サブ項目に分割済み。詳細な設計は各サブ項目を参照。
>
> - [マップ生成 設計](./generation/design.md)
> - [MapScene 設計](./scene/design.md)

## Unityパッケージ / アセット

| パッケージ / アセット | 用途 | 選定理由 |
|----------------------|------|----------|
| DOTween | ノード選択時のアニメーション | 軽量 |
| TextMeshPro | ノードラベル・エリア番号表示 | 標準高品位テキスト |

## シーン・オブジェクト構成

```
MapScene
  ├── [System]
  │     └── MapSceneController (MonoBehaviour)
  │
  ├── [Map]
  │     └── MapGraphView (MonoBehaviour)
  │           ├── EdgeRenderer × n (LineRenderer)
  │           └── NodeButton × n (Prefab)
  │                 ├── NodeButtonUI (MonoBehaviour)
  │                 ├── Image (アイコン)
  │                 └── TMP_Text (ラベル)
  │
  └── [HUD]
        └── MapHUD
              ├── AreaLabel (TMP_Text)
              └── ScrapText (TMP_Text)
```

## クラス設計

| クラス名 | 種別 | 責務 |
|---------|------|------|
| `MapNode` | plain C# | ノード1件のデータ（ID・種別・座標・接続先・訪問フラグ） |
| `MapGraph` | plain C# | ノードリストと接続情報を保持するグラフ構造 |
| `MapGenerator` | plain C# (static) | `MapGraph` を手続き生成する。層ごとにノード数・種別を抽選 |
| `MapSceneController` | MonoBehaviour | シーン初期化・ノード選択受信・ノード遷移処理 |
| `MapGraphView` | MonoBehaviour | `MapGraph` をもとにノードボタンと接続線を配置・描画 |
| `NodeButtonUI` | MonoBehaviour | ノードの状態（訪問済み・到達可能・現在地）に応じた見た目を制御 |

## データ構造

```csharp
public enum NodeType { Battle, Mystery, Shop, Refit, Boss }

public enum NodeState { Unvisited, Reachable, Current, Visited }

[System.Serializable]
public class MapNode
{
    public int      id;
    public NodeType type;
    public Vector2  position;       // MapGraphView 上の表示座標（0〜1 正規化）
    public List<int> nextNodeIds;   // 接続先ノードの ID リスト
    public NodeState state;
}

[System.Serializable]
public class MapGraph
{
    public List<MapNode> nodes;
    public int           currentNodeId;

    public MapNode GetNode(int id);
    public List<MapNode> GetReachableNodes(); // currentNodeId の接続先を返す
}

// RunManager が保持するマップ状態
// MapGraph CurrentMapGraph { get; }
// int      CurrentArea     { get; }  // エリア番号（1〜）
```

## インターフェース / イベント

```csharp
// ノード選択時のフロー
public class MapSceneController : MonoBehaviour
{
    // NodeButtonUI から呼ばれる
    public void OnNodeSelected(int nodeId);

    // ノード種別に応じたシーン遷移
    private void TransitionToNode(MapNode node);
    // Battle / Boss → PreparationScene
    // Shop          → ShopScene
    // Refit         → RefitScene
    // Mystery       → MysteryEventUI（MapScene 上にオーバーレイ）
}

// バトルクリア後に BattleSceneController → RunManager → MapScene に戻る際
// RunManager.CompleteNode(int nodeId) でノードを訪問済みにする
public class RunManager : MonoBehaviour
{
    public void CompleteNode(int nodeId);
    public void StartNewArea();       // ボスクリア後に新エリアのマップを生成
}
```

## 依存関係

| 参照先 | 用途 |
|--------|------|
| `RunManager` | `MapGraph` の保持・ノード完了通知・エリア進行 |
| `MapGenerator` | ラン開始時・エリアクリア時に新 `MapGraph` を生成 |
