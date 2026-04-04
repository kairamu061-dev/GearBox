# タグ定義

ドキュメント内のインラインタグの一覧と仕様を定義する。
タグはHTMLコメント形式で埋め込み、エージェントが検索・処理できるようにする。

## インライン記法

```
<!-- [タグ名] フィールド名="値" フィールド名="値" -->
```

## タグ一覧

| タグ名 | 用途 | 必須フィールド | 任意フィールド |
|--------|------|--------------|--------------|
| `求：画像` | 画像が必要な箇所に配置。画像生成エージェントへの依頼 | `prompt` | `ratio`, `output` |
| `求：コンセプトアート` | キャラクター・背景・世界観のコンセプトアートが必要な箇所 | `prompt` | `ratio`, `output` |
| `求：UI素材` | ボタン・アイコン・HUDなどのUI素材が必要な箇所 | `prompt` | `ratio`, `output` |
| `求：サウンド` | BGM・SEが必要な箇所 | `description` | `duration`, `mood` |

---

## タグ詳細

### `求：画像`

画像があるとわかりやすい箇所に配置する。画像生成エージェントがこのタグを検索し、`prompt` をもとに画像を生成して `output` に保存する。

**フィールド:**

| フィールド | 必須 | デフォルト | 説明 |
|-----------|------|-----------|------|
| `prompt` | Yes | — | 画像生成プロンプト（英語推奨） |
| `ratio` | No | `16:9` | アスペクト比: `1:1` / `4:3` / `3:4` / `16:9` / `9:16` |
| `output` | No | `docs/assets/{自動命名}.png` | 保存先パス |

**使用例:**

```
<!-- [求：画像] prompt="A clean minimal web login form with email and password fields, modern flat UI design" ratio="16:9" output="docs/assets/auth-login.png" -->
```

---

### `求：コンセプトアート`

キャラクター・背景・世界観のコンセプトアートが必要な箇所に配置する。

**フィールド:**

| フィールド | 必須 | デフォルト | 説明 |
|-----------|------|-----------|------|
| `prompt` | Yes | — | 生成プロンプト（英語推奨） |
| `ratio` | No | `16:9` | アスペクト比 |
| `output` | No | `docs/assets/{自動命名}.png` | 保存先パス |

**使用例:**

```
<!-- [求：コンセプトアート] prompt="A cute 2D pixel art character, small hero with a sword, bright colors, white background" ratio="1:1" output="docs/assets/character-hero.png" -->
```

---

### `求：UI素材`

ボタン・アイコン・HUDなどのUI素材が必要な箇所に配置する。

**フィールド:**

| フィールド | 必須 | デフォルト | 説明 |
|-----------|------|-----------|------|
| `prompt` | Yes | — | 生成プロンプト（英語推奨） |
| `ratio` | No | `1:1` | アスペクト比 |
| `output` | No | `docs/assets/{自動命名}.png` | 保存先パス |

**使用例:**

```
<!-- [求：UI素材] prompt="A simple flat icon for a pause button, minimal style, transparent background" ratio="1:1" output="docs/assets/ui-pause.png" -->
```

---

### `求：サウンド`

BGM・SEが必要な箇所に配置する。

**フィールド:**

| フィールド | 必須 | デフォルト | 説明 |
|-----------|------|-----------|------|
| `description` | Yes | — | サウンドの説明（日本語可） |
| `duration` | No | — | 目安の長さ（例: `30s`, `loop`） |
| `mood` | No | — | 雰囲気・テイスト（例: `upbeat`, `tense`, `calm`） |

**使用例:**

```
<!-- [求：サウンド] description="ステージクリア時のファンファーレ" duration="3s" mood="cheerful, triumphant" -->
```
