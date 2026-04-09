# MapScene 設計

## Unityパッケージ / アセット

| パッケージ / アセット | 用途 | 選定理由 |
|----------------------|------|----------|
| DOTween | ノードの点滅アニメーション | 軽量 Tween |
| TextMeshPro | ノードラベル・HUD テキスト | 標準高品位テキスト |

## クラス設計

| クラス名 | 種別 | 責務 |
|---------|------|------|
| `MapSceneController` | MonoBehaviour | シーン初期化・`OnNodeSelected()` 受信・`TransitionToNode()` 実行 |
| `MapGraphView` | MonoBehaviour | `MapGraph` をもとにノードボタンと接続線を動的生成・配置 |
| `NodeButtonUI` | MonoBehaviour | ノードの `NodeState` に応じた見た目（色・アニメーション）制御・クリックイベント発行 |

## インターフェース / イベント

```csharp
public class MapSceneController : MonoBehaviour
{
    // NodeButtonUI から呼ばれる
    public void OnNodeSelected(int nodeId);

    // ノード種別に応じたシーン遷移
    private void TransitionToNode(MapNode node);
    // Battle/Boss → SceneManager.LoadScene("PreparationScene")
    // Shop         → SceneManager.LoadScene("ShopScene")
    // Refit        → SceneManager.LoadScene("RefitScene")
    // Mystery      → HatenaEventController.TriggerEvent()（オーバーレイ）
}

public class MapGraphView : MonoBehaviour
{
    // MapSceneController の Start() から呼ばれる
    public void BuildView(MapGraph graph);
}

public class NodeButtonUI : MonoBehaviour
{
    public void Initialize(MapNode node, MapSceneController controller);
    public void RefreshState(NodeState state); // 状態変化時に見た目を更新
}
```

## 依存関係

| 参照先 | 用途 |
|--------|------|
| `RunManager` | `CurrentMapGraph` の読み取り・`CompleteNode()` 呼び出し |
| `MapGraph` / `MapNode`（map/generation/） | 描画データの取得 |
| `HatenaEventController`（economy/hatena/） | Mystery ノード選択時のイベント起動 |
