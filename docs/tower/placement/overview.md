# 設置型攻撃 概要

## 目的・背景

自機前方にオブジェクト（地雷等）を設置し、敵が接触したときに起爆する攻撃タイプ。感知機雷タワーが使用する。フィールドに罠を仕掛けるプレイスタイルを実現する。

## スコープ

**含む:**
- `PlacementAttack` MonoBehaviour（IAttackBehaviour 実装・設置上限管理）
- `PlacementObject` MonoBehaviour（寿命・接触起爆・ダメージ・プール返却）

**含まない:**
- `ObjectPool<T>`（→ tower/base/）
- 他の攻撃タイプ

## 制約

- `IAttackBehaviour` インターフェース（tower/base/）に準拠する
- `ObjectPool<PlacementObject>` を使用して設置物を管理する
- 設置数上限（デフォルト: 3）を超える場合、最も古い設置物を破壊してから新たに設置する
- 設置物の寿命はデフォルト 10 秒

## 完了条件

- CT ごとに自機前方に `PlacementObject` が設置される
- 設置物に敵が接触すると `IDamageable.TakeDamage()` を呼んでプールへ返却される
- 寿命切れでも同様に返却される
- 設置数上限が正しく機能する
