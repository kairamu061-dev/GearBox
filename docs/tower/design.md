# タワーシステム 設計

> サブ項目に分割済み。詳細な設計は各サブ項目を参照。
>
> - [タワー基盤設計](./base/design.md)
> - [合成システム設計](./synthesis/design.md)

## Unityパッケージ / アセット

| パッケージ / アセット | 用途 | 選定理由 |
|----------------------|------|----------|
| Unity Physics 2D | 弾丸の移動・当たり判定 | 標準パッケージ |
| TextMeshPro | タワー情報カード表示 | 標準採用 |

## 横断的な設計方針

### ScriptableObject によるデータ駆動設計
- タワーパラメータ・合成レシピはすべて ScriptableObject で定義
- コード変更なしに新タワー・新レシピを追加できる構成にする

### IAttackBehaviour インターフェース
- 攻撃タイプ（照準・追尾・範囲・設置）の切り替えを戦略パターンで実装
- `TowerBehaviour` は AttackBehaviour を差し替えるだけで対応可能

```csharp
public interface IAttackBehaviour
{
    void Attack(TowerBehaviour tower, AimProvider aim);
}
```

### ObjectPool\<T\> による弾丸管理
- 弾丸・設置オブジェクトはプールから取得/返却してGCを抑制する

### AimProvider シングルトン
- マウスカーソルのワールド座標変換を一元管理
- 照準型タワーと `TankController` が共用する

## 依存関係

| 依存先 | 理由 |
|--------|------|
| `IDamageable`（tower/base が定義） | 敵・戦車へのダメージ適用 |
| `RunManager`（battle/combat） | タワー在庫・既知レシピの保持 |
| `SynthesisUI`（tower/synthesis） | 準備フェーズ・改修ノードで共用 |
