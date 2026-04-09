# 敵システム基盤 設計

> 独立実装可能な単位はサブ項目に分割済み。詳細は各サブ項目を参照。
>
> - [敵弾 設計](../projectile/design.md)
> - [スクラップドロップ 設計](../scrap/design.md)

## Unityパッケージ / アセット

| パッケージ / アセット | 用途 | 選定理由 |
|----------------------|------|----------|
| Unity Physics 2D | 移動・衝突・射程検索 | NavMesh 不使用のため Rigidbody2D + Steering で実装 |

## シーン・オブジェクト構成

```
BattleScene
  └── Enemies (GameObject)
        └── EnemyUnit (Prefab × n)
              ├── Rigidbody2D
              ├── CircleCollider2D（当たり判定）
              ├── EnemyController (MonoBehaviour)
              ├── [AI コンポーネント: enemy/ai で定義]
              ├── ScrapDropper (MonoBehaviour)
              └── HealthBar (子 GameObject・任意)

        └── BossUnit (Prefab × 1, ボスノード限定)
              ├── Rigidbody2D
              ├── PolygonCollider2D
              ├── BossController (MonoBehaviour)  ← enemy/ai で定義
              └── ScrapDropper (MonoBehaviour)
```

## クラス設計

| クラス名 | 種別 | 責務 |
|---------|------|------|
| `EnemyData` | ScriptableObject | 敵の全パラメータを保持 |
| `IEnemyAI` | interface | `UpdateAI(Transform tank)` を定義する |
| `EnemyController` | MonoBehaviour | HP管理・ダメージ受信・AI呼び出し・死亡処理 |
| `EnemyProjectile` | MonoBehaviour | 敵弾: 直進・戦車への TakeDamage 呼び出し |
| `ScrapDropper` | MonoBehaviour | 死亡時に ScrapObject を指定個数ランダム散乱 |
| `ScrapObject` | MonoBehaviour | フィールドに落ちているスクラップ・60秒で自動消滅 |

## データ構造

```csharp
public enum AIType { Chaser, Turret, Rusher, Fortress, Boss }

[CreateAssetMenu(menuName = "GearBox/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string     enemyName;
    public GameObject prefab;
    public AIType     aiType;
    public float      maxHp;
    public float      moveSpeed;
    public float      attackDamage;
    public float      attackCooldown;
    public float      attackRange;
    public int        scrapDropMin;
    public int        scrapDropMax;
    public GameObject projectilePrefab; // TurretAI / FortressAI / Boss 用
}
```

## インターフェース / イベント

```csharp
public interface IEnemyAI
{
    void UpdateAI(Transform tankTransform);
}

// EnemyController の公開 API（IDamageable 実装）
public class EnemyController : MonoBehaviour, IDamageable
{
    public EnemyData Data { get; private set; }
    public void Initialize(EnemyData data);
    public void TakeDamage(float amount);  // HP 減少、0 で Die()
    // Die(): ScrapDropper.Drop() → Destroy(gameObject)
}

// 全敵撃破イベント（BattleSceneController が購読）
public static event Action OnAllEnemiesDefeated;
```

## 依存関係

| 参照先 | 用途 |
|--------|------|
| `IDamageable`（tower/base） | `EnemyController` が実装 / `EnemyProjectile` が呼び出す |
| `RunManager`（core） | `ScrapObject` 回収時にスクラップを加算 |
| `BattleSceneController`（battle/combat） | 全滅検知 |
