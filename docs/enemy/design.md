# 敵システム 設計

> サブ項目に分割済み。詳細な設計は各サブ項目を参照。
>
> - [敵システム基盤 設計](./base/design.md)
> - [敵 AI 実装 設計](./ai/design.md)

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
              ├── [ChaserAI / TurretAI / RusherAI / FortressAI] (MonoBehaviour)
              ├── ScrapDropper (MonoBehaviour)
              └── HealthBar (子 GameObject・任意)

        └── BossUnit (Prefab × 1, ボスノード限定)
              ├── Rigidbody2D
              ├── PolygonCollider2D
              ├── BossController (MonoBehaviour)
              └── ScrapDropper (MonoBehaviour)
```

## クラス設計

| クラス名 | 種別 | 責務 |
|---------|------|------|
| `EnemyData` | ScriptableObject | 敵の全パラメータを保持 |
| `EnemyController` | MonoBehaviour | HP管理・ダメージ受信・死亡処理・AI呼び出し |
| `IEnemyAI` | interface | `UpdateAI(Transform tank)` を定義 |
| `ChaserAI` | MonoBehaviour | 戦車方向へ Rigidbody2D.velocity を更新（障害物は接触で回避） |
| `TurretAI` | MonoBehaviour | 固定砲台: 射程内の戦車に向け CT ごとに弾丸を発射 |
| `RusherAI` | MonoBehaviour | バトル開始時に直線方向を決定し等速移動、壁で反転 |
| `FortressAI` | MonoBehaviour | `TurretAI` と同様の砲撃 + HP 50% で子ユニットをスポーン |
| `BossController` | MonoBehaviour | HP 閾値でフェーズを切り替え、フェーズ別 AI 処理を実行 |
| `ScrapDropper` | MonoBehaviour | 死亡時に ScrapObject を指定個数 scatter |
| `ScrapObject` | MonoBehaviour | フィールドに落ちているスクラップ。戦車の回収コライダーに触れると RunManager に加算 |
| `EnemyProjectile` | MonoBehaviour | 敵弾。戦車に当たると `IDamageable.TakeDamage` を呼ぶ |

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

// ボスのフェーズ定義
[System.Serializable]
public class BossPhase
{
    public float   hpThreshold;  // このHP割合を下回るとフェーズ移行
    public float   moveSpeed;
    public float   attackCooldown;
    public bool    spawnMinions;
    public bool    allDirectionShot;
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
    public void TakeDamage(float amount);    // HP 減少、0 で Die()
    // Die(): ScrapDropper 呼び出し → Destroy
}

// 敵全滅イベント（BattleSceneController が購読）
public static event Action OnAllEnemiesDefeated;
```

## 依存関係

| 参照先 | 用途 |
|--------|------|
| `IDamageable` | `EnemyProjectile` から戦車の `TankController` を傷つける |
| `RunManager` | `ScrapObject` 回収時にスクラップを加算 |
| `BattleSceneController` | 敵スポーン・全滅検知 |
