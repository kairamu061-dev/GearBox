# ハテナノード 設計

## Unityパッケージ / アセット

| パッケージ / アセット | 用途 | 選定理由 |
|----------------------|------|----------|
| TextMeshPro | フレーバーテキスト・タワー名表示 | 標準 |
| DOTween | オーバーレイのフェードイン/アウト | 軽量 |

## シーン・オブジェクト構成

```
MapScene（既存）
  └── HatenaOverlay (GameObject, 通常は非アクティブ)
        ├── Background (半透明 Image)
        ├── EventText (TMP_Text)  ← フレーバーテキスト
        ├── TowerChoicePanel (タワー選択イベント用、3ボタン)
        │     ├── TowerButton_0
        │     ├── TowerButton_1
        │     └── TowerButton_2
        └── CloseButton (Button)  ← タワー選択以外のイベントで表示
```

## クラス設計

| クラス名 | 種別 | 責務 |
|---------|------|------|
| `HatenaEventData` | ScriptableObject | イベント1件のデータ（種別・フレーバーテキスト・効果パラメータ） |
| `HatenaEventController` | MonoBehaviour | イベント抽選・処理実行・オーバーレイ UI 表示 |

## データ構造

```csharp
public enum HatenaEventType
{
    TowerChoice, ScrapGain, BlueprintDiscover,
    ScrapLoss, RandomUpgrade, FullHeal, Nothing
}

[CreateAssetMenu(menuName = "GearBox/HatenaEventData")]
public class HatenaEventData : ScriptableObject
{
    public HatenaEventType eventType;
    [Range(0f, 1f)] public float weight; // 出現率ウェイト（合計で正規化して使用）
    [TextArea] public string    flavorText;
    public int scrapMin, scrapMax;        // ScrapGain / ScrapLoss 用
    public int towerChoiceCount = 3;      // TowerChoice 用
}
```

## インターフェース / イベント

```csharp
public class HatenaEventController : MonoBehaviour
{
    [SerializeField] HatenaEventData[] allEvents;

    // ハテナノード選択時に MapSceneController から呼ばれる
    public void TriggerEvent();

    // イベント種別ごとの処理
    private void ProcessTowerChoice(HatenaEventData e);
    private void ProcessScrapGain(HatenaEventData e);
    private void ProcessBlueprintDiscover(HatenaEventData e);
    private void ProcessScrapLoss(HatenaEventData e);
    private void ProcessRandomUpgrade(HatenaEventData e);
    private void ProcessFullHeal();
}
```

## 依存関係

| 参照先 | 用途 |
|--------|------|
| `RunManager`（core） | スクラップ増減・HP全回復・タワー追加・レシピ解放 |
| `TowerData`（tower/base） | タワー選択肢の生成 |
| `MapSceneController`（map） | ハテナノード選択時に TriggerEvent を呼び出す |
