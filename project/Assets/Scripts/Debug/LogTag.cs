// リリースビルドでも型は必要（[Conditional]で呼び出しが消えるため中身は残しても問題なし）
public enum LogTag
{
    System,
    Battle,
    Player,
    Enemy,
    Tower,
    Map,
    Economy,
    UI,
    Save,
    Debug,
}
