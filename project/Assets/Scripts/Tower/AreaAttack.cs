using UnityEngine;

public class AreaAttack : IAttackBehaviour
{
    TowerBehaviour owner;

    public void SetOwner(TowerBehaviour o) => owner = o;

    public void Execute()
    {
        var hits = Physics2D.OverlapCircleAll(owner.transform.position, owner.Data.range);
        foreach (var h in hits)
            if (h.TryGetComponent<EnemyController>(out var e))
                e.TakeDamage(owner.Data.damage);
    }
}
