# 自動追尾型攻撃 設計

## Unityパッケージ / アセット

| パッケージ / アセット | 用途 | 選定理由 |
|----------------------|------|----------|
| Unity Physics 2D | OverlapCircle による射程内敵検索 | 標準機能で十分 |

## クラス設計

| クラス名 | 種別 | 責務 |
|---------|------|------|
| `AutoAimAttack` | MonoBehaviour | IAttackBehaviour 実装。OverlapCircle で敵検出 → HP 最低の敵に向けて Projectile を発射する |

## インターフェース / イベント

```csharp
public class AutoAimAttack : MonoBehaviour, IAttackBehaviour
{
    [SerializeField] private ObjectPool<Projectile> pool; // tower/aimed/ の Projectile Prefab を使用
    [SerializeField] private LayerMask              enemyLayer;

    // TowerBehaviour から呼ばれる
    public void Execute(TowerData data, AimProvider aim);

    // 射程内の敵から HP 最低のものを返す（いなければ null）
    private Transform FindTarget(float range);
}
```

## 依存関係

| 参照先 | 用途 |
|--------|------|
| `IAttackBehaviour`（tower/base/） | Execute() シグネチャの定義 |
| `ObjectPool<Projectile>`（tower/base/） | Projectile のプーリング |
| `Projectile`（tower/aimed/） | 弾丸クラスの共用 |
| `IDamageable` | 敵の HP 取得・TakeDamage() 呼び出し |
