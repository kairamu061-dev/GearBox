# 戦闘フェーズ 設計

## Unityパッケージ / アセット

| パッケージ / アセット | 用途 | 選定理由 |
|----------------------|------|----------|
| Unity Physics 2D | 戦車移動・衝突・スクラップ回収トリガー | 標準機能で十分 |
| Unity Tilemap | バトルフィールドの地形・障害物描画 | 軽量で柔軟なマップ構成が可能 |
| Cinemachine 2D | 戦車追従カメラ | 追従・デッドゾーン・クランプを設定で対応 |
| DOTween | クリア・ゲームオーバー演出 | 軽量Tween |

## シーン・オブジェクト構成

```
BattleScene
  ├── [System]
  │     ├── BattleSceneController (MonoBehaviour)
  │     ├── AimProvider (MonoBehaviour)
  │     └── Pools
  │           ├── ProjectilePool
  │           └── PlacementObjectPool
  │
  ├── [Field]
  │     ├── Ground Tilemap
  │     ├── Obstacle Tilemap (障害物)
  │     ├── WallCollider (不可視境界コライダー)
  │     ├── Flag (GameObject)
  │     │     ├── CircleCollider2D (isTrigger)
  │     │     └── FlagTrigger (MonoBehaviour)
  │     └── Enemies (GameObject, 敵を子に持つ)
  │
  ├── [Tank]
  │     ├── Rigidbody2D
  │     ├── CircleCollider2D
  │     ├── TankController (MonoBehaviour)
  │     ├── ScrapCollector (MonoBehaviour)
  │     └── TowerSlots (子 GameObject × グリッド数)
  │           └── TowerBehaviour + AttackBehaviour
  │
  ├── [HUD]
  │     ├── BattleHUD (MonoBehaviour)
  │     │     ├── HpBar (Slider)
  │     │     └── ScrapText (TMP_Text)
  │     └── MinimapController (MonoBehaviour)
  │
  └── [Camera]
        └── CinemachineVirtualCamera (Follow=Tank)
```

## クラス設計

> **RunManager は `core/` に独立。** 詳細は [core/design.md](../../core/design.md) を参照。

| クラス名 | 種別 | 責務 |
|---------|------|------|
| `BattleSceneController` | MonoBehaviour | バトル全体の状態管理（開始・クリア・ゲームオーバー）・敵スポーン |
| `TankController` | MonoBehaviour | WASD入力受取・Rigidbody2D 速度更新・`IDamageable` 実装 |
| `AimProvider` | MonoBehaviour | マウスのワールド座標を毎フレーム更新、他スクリプトに提供 |
| `ScrapCollector` | MonoBehaviour | `CircleCollider2D` (trigger) 内の `ScrapObject` を自動回収 |
| `FlagTrigger` | MonoBehaviour | 戦車が範囲内に入ると `BattleSceneController.OnFlagReached()` を呼ぶ |
| `BattleHUD` | MonoBehaviour | `RunManager` を購読して HP・スクラップ表示を更新 |
| `MinimapController` | MonoBehaviour | 自機・敵・旗の位置をミニマップ上のアイコンに反映 |

## データ構造

```csharp
// RunManager が保持するバトル関連の状態
public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    // 戦車
    public int   MaxHp         { get; private set; }
    public int   CurrentHp     { get; private set; }
    public int   Scrap         { get; private set; }

    // グリッド
    public Vector2Int          GridSize       { get; private set; } // 初期 (3,3)
    public TowerInstance[,]    GridLayout     { get; private set; } // null = 空き

    // 所持タワー（グリッド外の手持ち）
    public List<TowerInstance> TowerInventory { get; private set; }

    // 合成
    public List<SynthesisRecipe> KnownRecipes { get; private set; }

    // API
    public void AddScrap(int amount);
    public void SpendScrap(int amount);
    public void TakeDamage(int amount);
    public void Heal(int amount);
    public void ExpandGrid(bool addColumn); // true=列追加, false=行追加
}
```

## インターフェース / イベント

```csharp
// IDamageable（tower/base で定義）を TankController が実装
public class TankController : MonoBehaviour, IDamageable
{
    public void TakeDamage(float amount); // RunManager.TakeDamage を呼ぶ
}

// BattleSceneController の主要メソッド
public class BattleSceneController : MonoBehaviour
{
    public void OnFlagReached();   // FlagTrigger から呼ばれる
    public void OnBossDefeated();  // BossController から呼ばれる
    public void OnTankDestroyed(); // TankController HP=0 で呼ばれる
}

// RunManager のイベント（BattleHUD が購読）
public static event Action<int, int> OnHpChanged;    // (current, max)
public static event Action<int>      OnScrapChanged;  // (total)
```

## 依存関係

| 参照先 | 用途 |
|--------|------|
| `RunManager` | HP・スクラップ・グリッドデータの読み書き |
| `TowerBehaviour` / `IAttackBehaviour` | tower/base で実装済みのタワーをスロットに配置 |
| `EnemyController` / `IEnemyAI` | enemy で実装済みの敵をフィールドにスポーン |
| `IDamageable` | 敵弾が `TankController` を傷つける |
