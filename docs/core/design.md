# コア（RunManager） 設計

## Unityパッケージ / アセット

| パッケージ / アセット | 用途 | 選定理由 |
|----------------------|------|----------|
| なし（Unity 標準のみ） | DontDestroyOnLoad / static event | 外部依存ゼロで全機能から安全に参照できる |

## シーン・オブジェクト構成

```
[初回ロードシーン（例: BootScene）]
  └── RunManager (GameObject)
        └── RunManager (MonoBehaviour, DontDestroyOnLoad)
```

- RunManager は最初のシーンで1回だけ生成し、以後すべてのシーンで `RunManager.Instance` として参照する
- 複数インスタンスの生成を防ぐため Awake で重複チェックを行い、後から生成されたものを即 Destroy する

## クラス設計

| クラス名 | 種別 | 責務 |
|---------|------|------|
| `RunManager` | MonoBehaviour (DontDestroyOnLoad) | ゲーム全体の状態管理・API 提供・イベント発火 |
| `TowerInstance` | plain C# | グリッド配置・手持ちの個別タワー（TowerData + Level + 装備状態） |

## データ構造

```csharp
public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    // --- 状態フィールド ---
    public int        MaxHp         { get; private set; } = 100;
    public int        CurrentHp     { get; private set; } = 100;
    public int        Scrap         { get; private set; } = 0;

    public Vector2Int GridSize      { get; private set; } = new Vector2Int(3, 3);
    public TowerInstance[,] GridLayout { get; private set; }

    public List<TowerInstance>   TowerInventory { get; private set; }
    public List<SynthesisRecipe> KnownRecipes   { get; private set; }

    public MapGraph CurrentMapGraph { get; private set; }
    public int      CurrentNodeId   { get; private set; } = -1;

    // --- イベント ---
    public static event Action<int, int> OnHpChanged;    // (current, max)
    public static event Action<int>      OnScrapChanged;  // (total)

    // --- API ---
    public void AddScrap(int amount);
    public bool SpendScrap(int amount);   // 残高不足なら false
    public void TakeDamage(int amount);
    public void Heal(int amount);
    public void ExpandGrid(bool addColumn);
    public void AddTower(TowerInstance tower);
    public void RemoveTower(TowerInstance tower);
    public void CompleteNode(int nodeId);
    public void StartNewArea();
    public void UnlockRecipe(SynthesisRecipe recipe);
}

// グリッドに配置される（または手持ちの）個別タワー
[System.Serializable]
public class TowerInstance
{
    public TowerData Data;   // 元の ScriptableObject
    public int       Level;  // 1〜3
    // 将来: 一時バフ等を追加しやすい構造
}
```

## インターフェース / イベント

```csharp
// 使用側（例: BattleHUD）
RunManager.OnHpChanged    += (current, max) => UpdateHpBar(current, max);
RunManager.OnScrapChanged += (total)        => UpdateScrapText(total);

// 使用側（例: ShopSceneController）
bool ok = RunManager.Instance.SpendScrap(price);
if (ok) RunManager.Instance.AddTower(new TowerInstance { Data = tower, Level = 1 });
```

## 依存関係

| 参照先 | 用途 |
|--------|------|
| `TowerData`（tower/base） | TowerInstance.Data の型 |
| `SynthesisRecipe`（tower/synthesis） | KnownRecipes の型 |
| `MapGraph` / `MapNode`（map） | CurrentMapGraph の型 |
