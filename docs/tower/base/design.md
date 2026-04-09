# タワー定義・攻撃挙動 設計

> 攻撃タイプ別の実装はそれぞれのサブ項目を参照。
>
> - [照準型攻撃 設計](../aimed/design.md)
> - [自動追尾型攻撃 設計](../autoaim/design.md)
> - [範囲型攻撃 設計](../area/design.md)
> - [設置型攻撃 設計](../placement/design.md)

## Unityパッケージ / アセット

| パッケージ / アセット | 用途 | 選定理由 |
|----------------------|------|----------|
| Unity Physics 2D | OverlapCircle による射程内敵検索 | 標準機能で十分。NavMesh 不要 |

## シーン・オブジェクト構成

```
BattleScene
  └── TankRoot
        └── TowerSlot_0_0 〜 TowerSlot_N_M  (グリッド数分)
              ├── TowerBehaviour
              └── [AimedAttack / AutoAimAttack / AreaAttack / PlacementAttack]

  └── Pools
        ├── ProjectilePool
        └── PlacementObjectPool
```

## クラス設計

| クラス名 | 種別 | 責務 |
|---------|------|------|
| `TowerData` | ScriptableObject | タワーの全パラメータを保持する |
| `TowerBehaviour` | MonoBehaviour | CTカウント・`IAttackBehaviour.Execute()` 呼び出し |
| `IAttackBehaviour` | interface | 攻撃実行の共通口を定義する |
| `AimedAttack` | MonoBehaviour | 照準型: `AimProvider` からマウス方向を受け取り弾丸を発射 |
| `AutoAimAttack` | MonoBehaviour | 自動追尾型: `OverlapCircle` で射程内の敵を探して発射 |
| `AreaAttack` | MonoBehaviour | 範囲型: 周囲の全敵に即時 `TakeDamage` を呼ぶ |
| `PlacementAttack` | MonoBehaviour | 設置型: 前方に `PlacementObject` を生成、上限管理 |
| `Projectile` | MonoBehaviour | 直進移動・当たり判定・ダメージ処理・プール返却 |
| `PlacementObject` | MonoBehaviour | 敵接触 or 寿命で起爆・ダメージ処理・プール返却 |
| `ObjectPool<T>` | plain C# (generic) | `GameObject` のプーリング。取得 / 返却 API を提供 |
| `AimProvider` | MonoBehaviour (singleton) | `Camera.ScreenToWorldPoint` でマウスのワールド座標を毎フレーム更新、各タワーに提供 |
| `IDamageable` | interface | `void TakeDamage(float)` を定義。敵・戦車が実装 |

## データ構造

```csharp
public enum AttackType { Aimed, AutoAim, Area, Placement }

[CreateAssetMenu(menuName = "GearBox/TowerData")]
public class TowerData : ScriptableObject
{
    public string     towerName;
    public string     description;
    public Sprite     icon;
    public GameObject prefab;            // TowerBehaviour を持つ Prefab
    public AttackType attackType;
    public float      damage;
    public float      cooldown;          // CT（秒）
    public float      range;             // 射程（ユニット）
    public Vector2Int size;              // グリッド占有 (x=列, y=行)
    public bool[]     shape;             // size.x * size.y、左上原点の占有マップ
    public GameObject projectilePrefab;  // Aimed / AutoAim のみ
    public int        poolSize = 20;
    public int        basePrice;

    // 強化レベルデータ（最大 Lv3 = index 0〜1）
    public TowerUpgradeLevel[] upgradeLevels;
}

[System.Serializable]
public class TowerUpgradeLevel
{
    public float damageBonus;
    public float cooldownReduction;
    public float rangeBonus;
    public int   cost;
}

// RunManager が保持するランタイム状態
[System.Serializable]
public class TowerInstance
{
    public TowerData data;
    public int       upgradeLevel; // 0 = 無強化
}
```

## インターフェース / イベント

```csharp
public interface IAttackBehaviour
{
    void Execute(TowerData data, AimProvider aim);
}

public interface IDamageable
{
    void TakeDamage(float amount);
}

// TowerBehaviour の公開 API
public class TowerBehaviour : MonoBehaviour
{
    public TowerData Data { get; private set; }
    // BattleSceneController が配置時に呼ぶ
    public void Initialize(TowerData data, int upgradeLevel);
}

// ObjectPool の公開 API
public class ObjectPool<T> where T : MonoBehaviour
{
    public T  Get();
    public void Return(T obj);
}
```

## 依存関係

| 参照先 | 用途 |
|--------|------|
| `AimProvider` | 照準型・追尾型が毎フレームのマウスワールド座標を取得 |
| `RunManager` | `TowerInstance` リストを参照してタワーを初期化 |
| `IDamageable` | `Projectile` / `PlacementObject` が敵・戦車を傷つける |
