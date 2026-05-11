using UnityEngine;

public class Projectile : MonoBehaviour
{
    float damage;
    float speed = 10f;
    Vector2 targetPoint;
    bool hasTarget;
    DamageType damageType;
    float areaRadius;

    public void Initialize(Vector2 target, float dmg, DamageType dtype, float areaR = 0f, float spd = 10f)
    {
        targetPoint = target;
        damage = dmg;
        damageType = dtype;
        areaRadius = areaR;
        speed = spd;
        hasTarget = true;
    }

    // 追尾型用：ターゲット位置を後から更新できる
    public void SetTargetPoint(Vector2 point) => targetPoint = point;

    void Update()
    {
        if (!hasTarget) return;
        var dir = (targetPoint - (Vector2)transform.position).normalized;
        transform.Translate(dir * speed * Time.deltaTime, Space.World);

        if (Vector2.Distance(transform.position, targetPoint) < 0.1f)
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
                if (h.TryGetComponent<EnemyController>(out var e))
                    e.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
