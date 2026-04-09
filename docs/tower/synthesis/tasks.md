# タワー合成システム タスク

## 実装タスク一覧

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

### データ定義
- [ ] `SynthesisRecipe` ScriptableObject の定義
- [ ] 初期合成レシピ6種の ScriptableObject アセット作成

### ロジック
- [ ] `SynthesisManager` の実装
  - [ ] `CanSynthesize(a, b, out recipe)` — レシピ照合
  - [ ] `Execute(recipe)` — 素材消費・結果タワー追加（`RunManager` 経由）
  - [ ] `UnlockRecipe(recipe)` — 設計図取得・ハテナイベントで呼ぶ
  - [ ] グリッド配置済み素材タワーの自動除去（`GridManager` 経由）

### UI
- [ ] `SynthesisUI` の実装
  - [ ] 所持タワーのドロップダウン生成（同一タワーを両方に選べないよう制御）
  - [ ] 既知レシピのプレビュー表示（未知は「???」）
  - [ ] 「合成する」ボタンの有効/無効制御
  - [ ] 合成成功・失敗のフィードバック表示（DOTween）
- [ ] 既知レシピ一覧の `RecipeListItemUI` 実装

### 動作確認
- [ ] 既知レシピで合成すると素材2つが消費され結果タワーが増えることを確認
- [ ] 未知の組み合わせで「組み合わせ不明」になることを確認
- [ ] グリッドに配置済みの素材タワーが合成後にグリッドから除去されることを確認
- [ ] 設計図取得でレシピが既知になることを確認

## 依存関係

- `TowerData` / `TowerInstance`（tower/base）→ すべての前提
- `SynthesisRecipe` ScriptableObject → `SynthesisManager` の前提
- `SynthesisManager` → `SynthesisUI` の前提
- `GridManager`（battle/preparation）→ `SynthesisManager` でグリッド除去する場合の前提
