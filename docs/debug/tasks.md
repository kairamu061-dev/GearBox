# デバッグシステム タスク

## 実装タスク一覧

<!-- ステータス: [ ] 未着手 / [~] 進行中 / [x] 完了 -->

- [x] `LogTag` enum を定義する
- [x] `Log` 静的クラスを実装する（Info / Warning / Error）
- [x] `GameLogger` を実装する（プール・Unityログフック・ファイル出力）
- [x] `DebugMenuController` を実装する（自動生成・開閉・ボタン）
- [x] リリースビルド除外の確認（`[Conditional]` / `#if` の適用）
- [x] 既存の `Debug.Log` を `Log.Info` に置き換え（TankController・TowerBehaviour）
- [ ] デバッグメニューに RunManager 状態表示を追加
- [ ] デバッグメニューにシーン直接遷移を追加
- [ ] デバッグメニューにタイムスケール変更を追加

## 依存関係

- `Log` → `GameLogger`（Log が GameLogger.AddEntry を呼ぶ）
- `DebugMenuController` → `GameLogger`（ファイル出力・クリア）
