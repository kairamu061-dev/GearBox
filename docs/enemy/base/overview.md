# 敵システム基盤 概要

## 目的・背景

敵ユニット全種に共通するデータ定義・HP管理・死亡処理・スクラップドロップの基盤を実装する。AIの実装（enemy/ai）はこの基盤の上に乗るため、先に完成させる必要がある。

## スコープ

### 作るもの

- `EnemyData` ScriptableObject（敵パラメータ定義）
- `IEnemyAI` インターフェース（AI の共通契約）
- `EnemyController`（HP管理・ダメージ受信・AI呼び出し・死亡処理）
- `EnemyProjectile`（敵弾：直進・戦車 TakeDamage 呼び出し）
- `ScrapDropper`（死亡時にスクラップオブジェクトを散乱）
- `ScrapObject`（フィールドに落ちるスクラップ・60秒で自動消滅）

### 作らないもの

- AI ごとの行動実装（enemy/ai で対応）
- スクラップの回収処理（battle/combat で対応）
- 敵のスポーンスケジュール（battle/combat で対応）

## 制約

- `EnemyData` は ScriptableObject で定義し、コード変更なしに新敵種を追加できるようにする
- `IEnemyAI` インターフェースで AI の差し替えを可能にする（戦略パターン）
- NavMesh 不使用。移動は Steering Behaviour（Rigidbody2D.velocity）で実装する

## 完了条件

- `EnemyData` が定義されている
- `IEnemyAI` インターフェースが定義されている
- `EnemyController` が HP 管理・ダメージ受信・死亡処理を実行できる
- 死亡時に `ScrapDropper` が ScrapObject を散乱させる
- `ScrapObject` が 60 秒後に自動消滅する
