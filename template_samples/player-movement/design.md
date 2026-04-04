# プレイヤー移動 設計

## Unityパッケージ / アセット

| パッケージ / アセット | 用途 | 選定理由 |
|----------------------|------|----------|
| Unity Input System | 入力管理 | キーボード・ゲームパッドを統一的に扱える |
| Unity Physics 2D | 物理演算 | Rigidbody2D で重力・衝突を管理 |
| DOTween | 重力切り替えアニメーション | スムーズなカメラ・UI演出に使用 |

## シーン・オブジェクト構成

```
GameScene
  └── Player (GameObject)
        ├── Rigidbody2D
        ├── CapsuleCollider2D
        ├── Animator
        ├── PlayerController (MonoBehaviour)
        ├── GravityController (MonoBehaviour)
        └── GroundChecker (MonoBehaviour)
              └── GroundCheckPoint (子オブジェクト)
```

## クラス設計

| クラス名 | 種別 | 責務 |
|---------|------|------|
| `PlayerController` | MonoBehaviour | 入力受取・移動速度の Rigidbody2D への反映・Animator 制御 |
| `GravityController` | MonoBehaviour | 重力方向の切り替え・クールタイム管理 |
| `GroundChecker` | MonoBehaviour | Physics2D.OverlapCircle で接地判定 |
| `PlayerState` | enum | `Idle` / `Running` / `Falling` |

## データ構造

```csharp
public enum PlayerState { Idle, Running, Falling }

[CreateAssetMenu]
public class PlayerConfig : ScriptableObject
{
    public float moveSpeed = 5f;
    public float gravitySwitchCooldown = 0.3f;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.1f;
}
```

## インターフェース / イベント

```csharp
// 重力切り替え時に他システムへ通知
public static event Action<float> OnGravityChanged; // gravityScale (-1 or 1)

// PlayerController の主要メソッド
void Move(float horizontalInput);
void SwitchGravity();
bool IsGrounded();
```

## 依存関係

| パッケージ / アセット | 用途 |
|-----------------------|------|
| Unity Input System | `PlayerInputActions`（自動生成クラス）経由で入力取得 |
| DOTween | 重力切り替え時のプレイヤーのフリップアニメーション |
