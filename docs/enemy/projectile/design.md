# 敵弾 設計

## Unityパッケージ / アセット

| パッケージ / アセット | 用途 | 選定理由 |
|----------------------|------|----------|
| Unity Physics 2D | OnTriggerEnter2D による衝突検出 | 標準機能で十分 |

## クラス設計

| クラス名 | 種別 | 責務 |
|---------|------|------|
| `EnemyProjectile` | MonoBehaviour | 直進移動・衝突判定・ダメージ処理・消滅 |

## インターフェース / イベント

```csharp
public class EnemyProjectile : MonoBehaviour
{
    // 敵 AI から呼ばれる初期化メソッド
    public void Launch(Vector2 direction, float speed, float damage);

    private void OnTriggerEnter2D(Collider2D other);
}
```

## 依存関係

| 参照先 | 用途 |
|--------|------|
| `IDamageable`（tower/base/） | 戦車への TakeDamage() 呼び出し |
