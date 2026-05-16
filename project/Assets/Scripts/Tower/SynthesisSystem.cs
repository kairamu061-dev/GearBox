using System.Collections.Generic;
using UnityEngine;

public static class SynthesisSystem
{
    // 2つのタワーから合成結果を返す。レシピがなければ null
    public static SynthesisRecipe FindRecipe(TowerData a, TowerData b,
        IEnumerable<SynthesisRecipe> knownRecipes)
    {
        foreach (var r in knownRecipes)
        {
            if ((r.materialA == a && r.materialB == b) ||
                (r.materialA == b && r.materialB == a))
                return r;
        }
        return null;
    }

    // 合成を実行してグリッド・インベントリを更新する
    public static TowerInstance Execute(TowerInstance matA, TowerInstance matB,
        SynthesisRecipe recipe)
    {
        var rm = RunManager.Instance;

        // グリッドから除去
        if (matA.isOnGrid) RemoveFromGrid(matA, rm);
        else rm.RemoveTower(matA);

        if (matB.isOnGrid) RemoveFromGrid(matB, rm);
        else rm.RemoveTower(matB);

        var result = new TowerInstance(recipe.result);
        rm.AddTower(result);
        return result;
    }

    static void RemoveFromGrid(TowerInstance tower, RunManager rm)
    {
        var pos = tower.gridPosition.ToVector2Int();
        var layout = rm.GridLayout;
        if (pos.x < rm.GridSize.x && pos.y < rm.GridSize.y &&
            layout[pos.x, pos.y] == tower)
            layout[pos.x, pos.y] = null;
        tower.isOnGrid = false;
        rm.OnGridChanged?.Invoke(); // グリッドUI更新トリガー
    }
}
