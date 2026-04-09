# 照準型攻撃 設計

## Unityパッケージ / アセット

| パッケージ / アセット | 用途 | 選定理由 |
|----------------------|------|----------|
| Unity Physics 2D | Projectile の衝突検出（OnTriggerEnter2D） | 標準機能で十分 |

## クラス設計

| クラス名 | 種別 | 責務 |
|---------|------|------|
| `AimedAttack` | MonoBehaviour | IAttackBehaviour 実装。AimProvider からマウス方向を受け取り Projectile を発射する |
| `Projectile` | MonoBehaviour | 直進移動・衝突検出・ダメージ処理・ObjectPool への返却 |

## インターフェース / イベント

```csharp
public class AimedAttack : MonoBehaviour, IAttackBehaviour
{
    [SerializeField] private ObjectPool<Projectile> pool;
    [SerializeField] private AimProvider            aim;

    // TowerBehaviour から呼ばれる
    public void Execute(TowerData data, AimProvider aim);
}

public class Projectile : MonoBehaviour
{
    // ObjectPool から取得後に呼ばれる
    public void Launch(Vector2 direction, float speed, float range, float damage);

    // 命中または射程切れで呼ばれる
    private void ReturnToPool();

    private void OnTriggerEnter2D(Collider2D other); // 命中判定
}
```

## 依存関係

| 参照先 | 用途 |
|--------|------|
| `IAttackBehaviour`（tower/base/） | Execute() シグネチャの定義 |
| `AimProvider`（tower/base/） | マウスのワールド座標取得 |
| `ObjectPool<T>`（tower/base/） | Projectile のプーリング |
| `IDamageable` | 敵への TakeDamage() 呼び出し |
