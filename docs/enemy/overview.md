# 敵システム 概要

## 目的・背景

バトルフィールドに出現する敵ユニットを実装する。敵の種類・AI・ドロップがバトルの難易度とリプレイ性に直結する。初期は種類を絞ってシンプルなAIから始め、拡張しやすい設計にする。

## スコープ

### 作るもの

- EnemyData（ScriptableObject）: 敵のパラメータ定義
- EnemyAI: 自律移動・攻撃挙動
- 敵の種別（初期5種）: スクラップウォーカー・蒸気砲台・装甲列車・要塞型・ボス
- ダメージ処理・HP管理・死亡処理
- スクラップドロップ（死亡時にスクラップオブジェクトを生成）
- ボス専用の特殊行動

### 作らないもの

- スクラップの回収処理（battle/combat で対応）
- 敵のスポーンスケジュール（battle/combat のフィールド生成で対応）

## 制約

- 敵AIは NavMesh 不使用。Steering Behaviour（移動ベクトル計算）で実装する
- EnemyData は ScriptableObject で定義し、Assets/Data/Enemies/ 以下に配置する
- ボスの特殊行動はボス専用コンポーネントで実装し、通常敵と分離する

## 完了条件

- 5種の敵が EnemyData として定義されている
- 各敵が戦車を認識して追跡・攻撃する
- HPが0になるとスクラップをドロップして消滅する
- ボスが固有の特殊行動を行う

## 画面イメージ

```
フィールド上での敵の挙動例:

  スクラップウォーカー: → 戦車方向へ直進して体当たり
  蒸気砲台:            固定位置で射程内の戦車を砲撃
  装甲列車:            直線移動で突進、当たるとノックバック
  要塞型:              固定位置で砲台を展開しつつ砲撃
  ボス:                複数フェーズで行動パターンが変化
```

<!-- [求：コンセプトアート] prompt="Steampunk enemy vehicles: small scrap walker tank, fixed steam cannon turret, armored locomotive, heavy fortress tank, all in dark industrial style, top-down 2D sprites" ratio="16:9" output="docs/assets/enemy-lineup.png" -->
