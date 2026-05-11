public static class AttackBehaviourFactory
{
    public static IAttackBehaviour Create(AttackType type)
    {
        // 各攻撃タイプの実装は順次追加
        return type switch
        {
            _ => null,
        };
    }
}
