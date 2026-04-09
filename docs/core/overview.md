# コア（RunManager） 概要

## 目的・背景

ランを通じて変化するゲーム状態（HP・スクラップ・タワー在庫・グリッド・マップ進行・合成レシピ）を一元管理するシングルトンを実装する。すべての機能エリアがこのコンポーネントを前提としているため、最初に実装する。

## スコープ

### 作るもの

- `RunManager` シングルトン（DontDestroyOnLoad）
- 状態フィールド: HP / スクラップ / グリッド / タワー在庫 / 既知レシピ / マップ進行
- 状態変更 API: `AddScrap` / `SpendScrap` / `TakeDamage` / `Heal` / `ExpandGrid` / `CompleteNode` / `StartNewArea`
- 状態変更イベント: `OnHpChanged` / `OnScrapChanged`

### 作らないもの

- ゲームオーバー・クリア演出（battle/combat で対応）
- ショップ購入・強化処理（economy で対応）
- マップ生成ロジック（map で対応）

## 制約

- シングルトンパターンで実装し、DontDestroyOnLoad で全シーンに跨って存在する
- 状態の直接書き換えは RunManager の API 経由のみとし、外部からの直接代入は禁止する
- スクラップはマイナスにならない（SpendScrap は残高チェックして bool を返す）

## 完了条件

- `RunManager.Instance` が全シーンで参照できる
- すべての API が仕様通りに動作する
- `OnHpChanged` / `OnScrapChanged` イベントが購読側に正しく届く
