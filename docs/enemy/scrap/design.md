# スクラップドロップ 設計

## Unityパッケージ / アセット

| パッケージ / アセット | 用途 | 選定理由 |
|----------------------|------|----------|
| Unity Physics 2D | ScrapObject の回収判定（OnTriggerEnter2D） | 標準機能で十分 |

## クラス設計

| クラス名 | 種別 | 責務 |
|---------|------|------|
| `ScrapDropper` | MonoBehaviour | `Drop()` で ScrapObject をプールから取得して散乱させる |
| `ScrapObject` | MonoBehaviour | 回収トリガー・60秒タイマー・プール返却 |

## インターフェース / イベント

```csharp
public class ScrapDropper : MonoBehaviour
{
    [SerializeField] private ObjectPool<ScrapObject> pool;

    // EnemyController.Die() から呼ばれる
    public void Drop(Vector2 deathPosition, EnemyData data);
}

public class ScrapObject : MonoBehaviour
{
    // ScrapDropper から呼ばれる初期化メソッド
    public void Activate(Vector2 position, ObjectPool<ScrapObject> pool);

    private void OnTriggerEnter2D(Collider2D other); // 戦車の ScrapCollector との接触判定

    private IEnumerator LifetimeCoroutine(); // 60 秒タイマー
}
```

## 依存関係

| 参照先 | 用途 |
|--------|------|
| `RunManager`（core/） | 回収時の `AddScrap(1)` 呼び出し |
| `EnemyData`（enemy/base/） | `scrapDropMin` / `scrapDropMax` の参照 |
| `ObjectPool<T>`（tower/base/） | ScrapObject のプーリング |
