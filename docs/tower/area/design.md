# 範囲型攻撃 設計

## Unityパッケージ / アセット

| パッケージ / アセット | 用途 | 選定理由 |
|----------------------|------|----------|
| Unity Physics 2D | OverlapCircle による範囲内敵検索 | 標準機能で十分 |

## クラス設計

| クラス名 | 種別 | 責務 |
|---------|------|------|
| `AreaAttack` | MonoBehaviour | IAttackBehaviour 実装。OverlapCircle で範囲内の全敵を取得し、全員に TakeDamage() を呼ぶ |

## インターフェース / イベント

```csharp
public class AreaAttack : MonoBehaviour, IAttackBehaviour
{
    [SerializeField] private LayerMask   enemyLayer;
    [SerializeField] private GameObject  effectPrefab; // オプション（null 可）

    // TowerBehaviour から呼ばれる
    public void Execute(TowerData data, AimProvider aim);
}
```

## 依存関係

| 参照先 | 用途 |
|--------|------|
| `IAttackBehaviour`（tower/base/） | Execute() シグネチャの定義 |
| `IDamageable` | 各敵への TakeDamage() 呼び出し |
