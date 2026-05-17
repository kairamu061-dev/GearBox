using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Projectile : MonoBehaviour
{
    float damage;
    float speed = 10f;
    Vector2 targetPoint;
    bool hasTarget;
    DamageType damageType;
    float areaRadius;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.isKinematic = true;
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    public void Initialize(Vector2 target, float dmg, DamageType dtype, float areaR = 0f, float spd = 10f)
    {
        targetPoint = target;
        damage = dmg;
        damageType = dtype;
        areaRadius = areaR;
        speed = spd;
        hasTarget = true;

        var dir = (target - (Vector2)transform.position).normalized;
        rb.linearVelocity = dir * speed;
    }

    public void SetTargetPoint(Vector2 point)
    {
        targetPoint = point;
        var dir = (point - rb.position).normalized;
        rb.linearVelocity = dir * speed;
    }

    void Update()
    {
        if (!hasTarget) return;
        if (Vector2.Distance(rb.position, targetPoint) < 0.2f)
            HitAtPoint();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<EnemyController>(out var enemy))
        {
            if (damageType == DamageType.Single)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        else if (other.TryGetComponent<FortressController>(out var fortress))
        {
            if (damageType == DamageType.Single)
            {
                fortress.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        else if (other.TryGetComponent<BossController>(out var boss))
        {
            if (damageType == DamageType.Single)
            {
                boss.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        else if (other.TryGetComponent<DestructibleWall>(out var wall))
        {
            wall.TakeDamage(damage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }

    void HitAtPoint()
    {
        if (damageType == DamageType.Area)
        {
            var hits = Physics2D.OverlapCircleAll(targetPoint, areaRadius);
            foreach (var h in hits)
            {
                if (h.TryGetComponent<EnemyController>(out var e)) e.TakeDamage(damage);
                else if (h.TryGetComponent<FortressController>(out var f)) f.TakeDamage(damage);
                else if (h.TryGetComponent<BossController>(out var b)) b.TakeDamage(damage);
            }
        }
        Destroy(gameObject);
    }
}
