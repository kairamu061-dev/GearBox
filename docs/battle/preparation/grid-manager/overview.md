# GridManager 概要

## 目的・背景

グリッドへのタワー配置・除去・占有判定を純粋な計算ロジックとして切り出す。
`GridUI` や `TowerDragHandler` が RunManager の GridLayout を直接書き換えるのを防ぎ、
配置ルール（サイズ・重複・上限）を一箇所で管理する。

## スコープ

**含む:**
- `GridManager` static class（占有判定・配置・除去・占有マス取得）

**含まない:**
- UI 描画（→ battle/preparation/）
- ドラッグ操作（→ battle/preparation/）
- グリッドサイズの拡張（→ battle/preparation/grid-expansion/）

## 制約

- MonoBehaviour に依存しない純粋 C# クラスとする（テスト容易性）
- `RunManager.GridLayout` のみを読み書きし、他の状態に触れない

## 完了条件

- `CanPlace`・`Place`・`Remove`・`GetOccupiedCells` が実装済み
- 単体テストで境界判定・重複判定・除去が検証済み
