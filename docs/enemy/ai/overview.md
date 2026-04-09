# 敵 AI 実装 概要

## 目的・背景

`IEnemyAI` インターフェースを実装する5種の AI コンポーネントを作成する。各 AI は独立して実装・テストでき、`EnemyController` にアタッチするだけで切り替えられる。

## スコープ

### 作るもの

- `ChaserAI`（追跡型: 戦車方向へ直進・簡易障害物回避）
- `TurretAI`（固定砲撃型: 射程内の戦車へ CT ごとに弾丸を発射）
- `RusherAI`（直線突進型: 直線移動・壁/障害物で反転）
- `FortressAI`（固定要塞型: 砲撃 + HP50% 以下で子ユニットスポーン）
- `BossController`（ボス専用: HP 閾値でフェーズを切り替え）

### 作らないもの

- AI の共通基盤（enemy/base の `IEnemyAI` / `EnemyController` で対応）
- ボス以外の Prefab 組み立て（enemy/base で対応）

## 制約

- すべての AI は `IEnemyAI.UpdateAI(Transform tankTransform)` を実装する
- `BossController` のみ `EnemyController` の代わりに直接使用するラッパーとする（HP 閾値処理が必要なため）
- AI ごとに MonoBehaviour を分離し、同一 GameObject に複数 AI をアタッチしない

## 完了条件

- 5種の AI が仕様通りの行動を行う
- ボスが3フェーズで行動パターンを変化させる
- 各 AI を `EnemyController` にアタッチして動作確認できる
