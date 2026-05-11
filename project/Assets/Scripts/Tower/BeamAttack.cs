using UnityEngine;

public class BeamAttack : IAttackBehaviour
{
    TowerBehaviour owner;

    public void SetOwner(TowerBehaviour o) => owner = o;

    public void Execute()
    {
        var origin = (Vector2)owner.transform.position;
        var dir = AimProvider.Instance != null
            ? (AimProvider.Instance.AimPosition - origin).normalized
            : (Vector2)owner.transform.up;

        var hits = Physics2D.RaycastAll(origin, dir, owner.Data.range);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        if (owner.Data.damageType == DamageType.Single)
        {
            foreach (var h in hits)
            {
                if (h.collider.TryGetComponent<EnemyController>(out var e))
                {
                    e.TakeDamage(owner.Data.damage);
                    break; // 最近接1体のみ
                }
            }
        }
        else
        {
            foreach (var h in hits)
                if (h.collider.TryGetComponent<EnemyController>(out var e))
                    e.TakeDamage(owner.Data.damage);
        }
    }
}
