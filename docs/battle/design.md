# バトルシステム 設計

> サブ項目に分割済み。詳細な設計は各サブ項目を参照。
>
> - [準備フェーズ設計](./preparation/design.md)
> - [戦闘フェーズ設計](./combat/design.md)

## Unityパッケージ / アセット

| パッケージ / アセット | 用途 | 選定理由 |
|----------------------|------|----------|
| Unity Physics 2D | 戦車・弾丸の物理演算 | 標準パッケージ、トップダウン2Dに適合 |
| Cinemachine | 戦車追従カメラ | フィールドクランプ・スムージングが標準機能で揃う |
| Unity Tilemap | 戦闘フィールドの地形描画 | 障害物配置・衝突判定と組み合わせやすい |
| DOTween | UI演出（クリア・ゲームオーバー） | 軽量・直感的なTween API |
| TextMeshPro | HUD・スコア表示 | 標準採用 |

## シーン構成（全体）

```
PreparationScene  → (出撃) → BattleScene → (クリア/ゲームオーバー) → MapScene / GameOverScene
```

- 準備フェーズと戦闘フェーズはシーンを分けて独立させる
- RunManager（DontDestroyOnLoad）が両シーン間で状態を引き継ぐ

## 横断的な設計方針

### RunManager 中心設計
- グリッドレイアウト・HP・スクラップ・タワー在庫はすべて RunManager が保持
- シーンを跨ぐデータ受け渡しは RunManager 経由に統一する

### IDamageable インターフェース
- 戦車・敵の両方に適用し、ダメージ処理を統一する

```csharp
public interface IDamageable
{
    void TakeDamage(int amount);
    int CurrentHp { get; }
}
```

### シーン遷移フロー

```
MapScene
  → (バトルノード選択)
  → PreparationScene
  → (出撃)
  → BattleScene
  → (旗到達 / ボス撃破 / HP0)
  → MapScene / GameOverScene
```

## 依存関係

| 依存先 | 理由 |
|--------|------|
| RunManager（battle/combat） | グリッド・HP・スクラップの状態保持 |
| TowerData / TowerBehaviour（tower/base） | 配置・攻撃の前提 |
| EnemyController（enemy） | 戦闘フェーズの敵スポーン |
