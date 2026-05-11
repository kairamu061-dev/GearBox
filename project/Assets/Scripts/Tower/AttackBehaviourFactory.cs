public static class AttackBehaviourFactory
{
    public static IAttackBehaviour Create(AttackType type) => type switch
    {
        AttackType.Aimed    => new AimedAttack(),
        AttackType.AutoAim  => new AutoAimAttack(),
        AttackType.Area     => new AreaAttack(),
        AttackType.Beam     => new BeamAttack(),
        _                   => null,
    };
}
