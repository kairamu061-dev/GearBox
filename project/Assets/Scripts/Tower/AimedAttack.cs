using UnityEngine;

public class AimedAttack : IAttackBehaviour
{
    TowerBehaviour owner;

    public void SetOwner(TowerBehaviour o) => owner = o;

    public void Execute()
    {
        var aim = AimProvider.Instance?.AimPosition ?? Vector2.zero;
        var origin = (Vector2)owner.transform.position;
        var toAim = aim - origin;
        float range = owner.Data.range;

        // 射程内ならマウス位置、射程外なら境界点
        var targetPoint = toAim.magnitude <= range
            ? aim
            : origin + toAim.normalized * range;

        var go = new GameObject("Projectile");
        go.transform.position = origin;
        var proj = go.AddComponent<Projectile>();
        go.AddComponent<CircleCollider2D>().isTrigger = true;
        proj.Initialize(targetPoint, owner.Data.damage, owner.Data.damageType, owner.Data.range * 0.5f);
    }
}
