# タワー合成システム 設計

## Unityパッケージ / アセット

| パッケージ / アセット | 用途 | 選定理由 |
|----------------------|------|----------|
| DOTween | 合成成功時のアニメーション演出 | 軽量・直感的 |

## シーン・オブジェクト構成

```
PreparationScene / RefitScene（共用 Prefab）
  └── SynthesisPanel (GameObject)
        ├── SynthesisUI (MonoBehaviour)
        ├── IngredientDropdownA (TMP_Dropdown)
        ├── IngredientDropdownB (TMP_Dropdown)
        ├── ResultPreview (Image + Text)
        ├── SynthesizeButton (Button)
        └── KnownRecipeList (ScrollRect)
              └── RecipeListItem × n  (Prefab)
```

## クラス設計

| クラス名 | 種別 | 責務 |
|---------|------|------|
| `SynthesisRecipe` | ScriptableObject | 素材 A・B と結果タワーのデータを保持 |
| `SynthesisManager` | plain C# | レシピ照合・合成実行ロジック。`RunManager` 経由で所持リストを更新 |
| `SynthesisUI` | MonoBehaviour | ドロップダウン生成・プレビュー更新・合成ボタン制御 |
| `RecipeListItemUI` | MonoBehaviour | 既知レシピ一覧の1行分の表示 |

## データ構造

```csharp
[CreateAssetMenu(menuName = "GearBox/SynthesisRecipe")]
public class SynthesisRecipe : ScriptableObject
{
    public TowerData ingredientA;
    public TowerData ingredientB;
    public TowerData result;
    public bool      isKnownByDefault; // true = ラン開始時から既知
}

// RunManager が保持する合成関連の状態
// List<SynthesisRecipe> KnownRecipes;
// (初期化時に isKnownByDefault == true のものを全追加)
```

## インターフェース / イベント

```csharp
public static class SynthesisManager
{
    // レシピが既知かつ所持タワーで合成可能か
    public static bool CanSynthesize(TowerData a, TowerData b, out SynthesisRecipe recipe);

    // 合成実行: RunManager の所持リストから a・b を消費し result を追加
    // グリッドに a または b が配置済みの場合は配置を解除してから消費
    public static TowerData Execute(SynthesisRecipe recipe);
}

// レシピ解放（設計図取得・ハテナイベント時）
public static class SynthesisManager
{
    public static void UnlockRecipe(SynthesisRecipe recipe);
}
```

## 依存関係

| 参照先 | 用途 |
|--------|------|
| `RunManager` | 所持タワーリスト・既知レシピリストの読み書き |
| `TowerData` | 素材・結果タワーのデータ参照 |
| `GridManager` | 合成素材がグリッドに配置済みの場合に除去を依頼 |
