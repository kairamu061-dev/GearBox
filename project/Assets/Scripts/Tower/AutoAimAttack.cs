using UnityEngine;

public class AutoAimAttack : IAttackBehaviour
{
    TowerBehaviour owner;

    public bool HasTarget { get; private set; }

    public void SetOwner(TowerBehaviour o) => owner = o;

    // CT到達時に呼ばれる
    public void Execute()
    {
        var target = FindTarget();
        HasTarget = target != null;
        if (target != null) Fire(target);
    }

    // CT未開始時に毎フレーム呼ばれる。発射できたら true を返す
    public bool TryInstantFire()
    {
        var target = FindTarget();
        if (target == null) return false;
        Fire(target);
        return true;
    }

    EnemyController FindTarget()
    {
        var hits = Physics2D.OverlapCircleAll(owner.transform.position, owner.Data.range);
        EnemyController best = null;
        float bestDist = float.MaxValue;
        foreach (var h in hits)
        {
            if (!h.TryGetComponent<EnemyController>(out var e)) continue;
            float dist = Vector2.Distance(owner.transform.position, e.transform.position);
            if (dist < bestDist) { bestDist = dist; best = e; }
        }
        return best;
    }

    void Fire(EnemyController target)
    {
        var go = new GameObject("AutoAimProjectile");
        go.transform.position = owner.transform.position;
        var proj = go.AddComponent<Projectile>();
        go.AddComponent<CircleCollider2D>().isTrigger = true;
        proj.Initialize(target.transform.position, owner.Data.damage, owner.Data.damageType);
    }
}
