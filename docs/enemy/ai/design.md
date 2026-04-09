# 敵 AI 実装 設計

## Unityパッケージ / アセット

| パッケージ / アセット | 用途 | 選定理由 |
|----------------------|------|----------|
| Unity Physics 2D | Rigidbody2D.velocity による移動 | Steering Behaviour の実装基盤 |

## シーン・オブジェクト構成

```
EnemyUnit (Prefab)
  ├── EnemyController        ← enemy/base で定義
  └── [いずれか1つをアタッチ]
        ├── ChaserAI
        ├── TurretAI
        ├── RusherAI
        └── FortressAI

BossUnit (Prefab)
  └── BossController         ← EnemyController を内包するラッパー
```

## クラス設計

| クラス名 | 種別 | 責務 |
|---------|------|------|
| `ChaserAI` | MonoBehaviour / IEnemyAI | 毎フレーム戦車方向へ velocity を更新 |
| `TurretAI` | MonoBehaviour / IEnemyAI | CT 経過ごとに射程内の戦車へ弾丸を発射 |
| `RusherAI` | MonoBehaviour / IEnemyAI | 直線移動・壁で速度を反転 |
| `FortressAI` | MonoBehaviour / IEnemyAI | TurretAI 相当の砲撃 + HP50% で子ユニットスポーン |
| `BossController` | MonoBehaviour | HP 閾値でフェーズ移行・フェーズ別処理を委譲 |

## データ構造

```csharp
// ボスのフェーズ定義（EnemyData 内に配列として保持）
[System.Serializable]
public class BossPhase
{
    public float hpThreshold;      // このHP割合を下回るとフェーズ移行
    public float moveSpeed;
    public float attackCooldown;
    public bool  spawnMinions;
    public bool  allDirectionShot; // 全方位弾を使用するか
}

// EnemyData に追加（boss 用）
// public BossPhase[] phases;
```

## インターフェース / イベント

```csharp
// ChaserAI 実装例
public class ChaserAI : MonoBehaviour, IEnemyAI
{
    private Rigidbody2D _rb;
    public void UpdateAI(Transform tankTransform)
    {
        Vector2 dir = (tankTransform.position - transform.position).normalized;
        _rb.velocity = dir * _data.moveSpeed;
    }
}

// BossController — フェーズ管理
public class BossController : MonoBehaviour
{
    private int _currentPhase = 0;
    // EnemyController.OnHpChanged を購読してフェーズチェック
    private void CheckPhase(float currentHp, float maxHp)
    {
        float ratio = currentHp / maxHp;
        // ratio が phases[_currentPhase].hpThreshold を下回ったらフェーズ移行
    }
}
```

## 依存関係

| 参照先 | 用途 |
|--------|------|
| `IEnemyAI`（enemy/base） | 各 AI が実装するインターフェース |
| `EnemyController`（enemy/base） | AI コンポーネントのホスト・EnemyData 参照 |
| `EnemyProjectile`（enemy/base） | TurretAI / FortressAI が発射する弾丸 |
| `BattleSceneController`（battle/combat） | BossController がボス撃破時に通知 |
