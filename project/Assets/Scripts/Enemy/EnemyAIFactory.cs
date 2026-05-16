using UnityEngine;

public static class EnemyAIFactory
{
    public static IEnemyAI Create(AIType type, EnemyController controller, Rigidbody2D rb)
    {
        return type switch
        {
            AIType.Chaser   => new ChaserAI(rb),
            AIType.Turret   => new TurretAI(controller),
            AIType.Rusher   => new RusherAI(rb),
            _               => new ChaserAI(rb),
        };
    }
}

// ── 追跡型 ──────────────────────────────────────
public class ChaserAI : IEnemyAI
{
    readonly Rigidbody2D rb;
    float speed;

    public ChaserAI(Rigidbody2D rb) => this.rb = rb;

    public void Initialize(EnemyData data) => speed = data.moveSpeed;

    public void UpdateAI(Transform tank)
    {
        var dir = ((Vector2)tank.position - rb.position).normalized;
        rb.linearVelocity = dir * speed;
    }
}

// ── 固定砲撃型 ──────────────────────────────────
public class TurretAI : IEnemyAI
{
    readonly EnemyController ctrl;
    float damage, cooldown, range, timer;
    GameObject projectilePrefab;

    public TurretAI(EnemyController ctrl) => this.ctrl = ctrl;

    public void Initialize(EnemyData data)
    {
        damage = data.attackDamage;
        cooldown = data.attackCooldown;
        range = data.attackRange;
        projectilePrefab = data.projectilePrefab;
        timer = cooldown;
    }

    public void UpdateAI(Transform tank)
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;
        float dist = Vector2.Distance(ctrl.transform.position, tank.position);
        if (dist > range) return;
        timer = cooldown;
        if (projectilePrefab == null) return;
        var dir = ((Vector2)tank.position - (Vector2)ctrl.transform.position).normalized;
        var proj = Object.Instantiate(projectilePrefab, ctrl.transform.position, Quaternion.identity);
        if (proj.TryGetComponent<EnemyProjectile>(out var ep))
            ep.Initialize(dir, damage);
    }
}

// ── 直線突進型 ──────────────────────────────────
public class RusherAI : IEnemyAI
{
    readonly Rigidbody2D rb;
    Vector2 dir;
    float speed;

    public RusherAI(Rigidbody2D rb) => this.rb = rb;

    public void Initialize(EnemyData data)
    {
        speed = data.moveSpeed;
        dir = Random.insideUnitCircle.normalized;
        rb.linearVelocity = dir * speed;
    }

    public void UpdateAI(Transform _)
    {
        // 壁での反転は OnCollisionEnter2D 側で行う（EnemyController 拡張予定）
    }
}
