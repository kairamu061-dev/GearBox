# GridManager 開発メモ

## 実装上の決定事項

### Remove の設計：任意の占有マスを起点にする
`Remove(anyOccupiedCell)` は origin ではなく任意の占有マスを引数に取る。
呼び出し元（TowerDragHandler）がクリックされたマスだけを知っており、
origin（左上マス）を別途計算する必要がないようにするため。
実装側は GridLayout 全体を走査して同じ TowerInstance 参照のマスをすべて null に戻す。

### static class を選択した理由
状態を持たず RunManager の GridLayout を操作するだけのため MonoBehaviour は不要。
pure C# static class にすることで Unity TestRunner の EditMode テストから直接呼び出せる。
