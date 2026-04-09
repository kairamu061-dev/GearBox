# 設置型攻撃 設計

## Unityパッケージ / アセット

| パッケージ / アセット | 用途 | 選定理由 |
|----------------------|------|----------|
| Unity Physics 2D | PlacementObject の接触検出（OnTriggerEnter2D） | 標準機能で十分 |

## クラス設計

| クラス名 | 種別 | 責務 |
|---------|------|------|
| `PlacementAttack` | MonoBehaviour | IAttackBehaviour 実装。設置上限管理・PlacementObject をプールから取得して前方に配置する |
| `PlacementObject` | MonoBehaviour | 敵接触で起爆・ダメージ処理・寿命タイマー・ObjectPool への返却 |

## インターフェース / イベント

```csharp
public class PlacementAttack : MonoBehaviour, IAttackBehaviour
{
    [SerializeField] private ObjectPool<PlacementObject> pool;
    [SerializeField] private int maxCount = 3;

    private Queue<PlacementObject> activeObjects;

    // TowerBehaviour から呼ばれる
    public void Execute(TowerData data, AimProvider aim);
}

public class PlacementObject : MonoBehaviour
{
    // PlacementAttack から呼ばれる初期化メソッド
    public void Initialize(float damage, float lifetime, ObjectPool<PlacementObject> pool);

    private void OnTriggerEnter2D(Collider2D other); // 接触起爆
    private IEnumerator LifetimeCoroutine();          // 寿命タイマー
}
```

## 依存関係

| 参照先 | 用途 |
|--------|------|
| `IAttackBehaviour`（tower/base/） | Execute() シグネチャの定義 |
| `ObjectPool<T>`（tower/base/） | PlacementObject のプーリング |
| `IDamageable` | 接触した敵への TakeDamage() 呼び出し |
