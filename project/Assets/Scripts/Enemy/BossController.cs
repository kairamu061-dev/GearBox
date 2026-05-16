using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BossController : MonoBehaviour, IDamageable
{
    [Header("パラメータ")]
    [SerializeField] float maxHp           = 500f;
    [SerializeField] float phase2Threshold = 0.6f;  // HP60%でフェーズ2
    [SerializeField] float phase3Threshold = 0.3f;  // HP30%でフェーズ3

    [Header("フェーズ1")]
    [SerializeField] float p1MoveSpeed  = 2f;
    [SerializeField] float p1AttackCT   = 2.0f;
    [SerializeField] float p1Damage     = 20f;

    [Header("フェーズ2")]
    [SerializeField] float p2MoveSpeed  = 3.5f;
    [SerializeField] float p2AttackCT   = 1.0f;
    [SerializeField] float p2Damage     = 25f;
    [SerializeField] EnemyData spawnData;

    [Header("フェーズ3")]
    [SerializeField] float p3MoveSpeed  = 5f;
    [SerializeField] float p3AttackCT   = 0.6f;
    [SerializeField] float p3Damage     = 30f;

    [Header("参照")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] ScrapObject scrapPrefab;
    [SerializeField] int scrapDropMin = 150;
    [SerializeField] int scrapDropMax = 200;

    Rigidbody2D rb;
    float hp;
    int phase = 1;
    bool isDead;
    float attackTimer;
    bool phase2Spawned;

    void Awake() => rb = GetComponent<Rigidbody2D>();
    void Start()
    {
        hp = maxHp;
        StartCoroutine(BossLoop());
    }

    IEnumerator BossLoop()
    {
        while (!isDead)
        {
            yield return null;
            UpdatePhase();
            MoveTowardPlayer();
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f) Attack();
        }
    }

    void UpdatePhase()
    {
        float ratio = hp / maxHp;
        int newPhase = ratio > phase2Threshold ? 1 : ratio > phase3Threshold ? 2 : 3;
        if (newPhase == phase) return;
        phase = newPhase;

        if (phase == 2 && !phase2Spawned)
        {
            phase2Spawned = true;
            SpawnMinions(2);
        }
        if (phase == 3)
        {
            SpawnMinions(3);
            StartCoroutine(BurstAttack());
        }
    }

    void MoveTowardPlayer()
    {
        if (TankController.Instance == null) return;
        float speed = phase == 1 ? p1MoveSpeed : phase == 2 ? p2MoveSpeed : p3MoveSpeed;
        var dir = ((Vector2)TankController.Instance.transform.position - rb.position).normalized;
        rb.linearVelocity = dir * speed;
    }

    void Attack()
    {
        float ct     = phase == 1 ? p1AttackCT : phase == 2 ? p2AttackCT : p3AttackCT;
        float damage = phase == 1 ? p1Damage   : phase == 2 ? p2Damage   : p3Damage;
        attackTimer = ct;
        if (TankController.Instance == null || projectilePrefab == null) return;

        var dir = ((Vector2)TankController.Instance.transform.position
                   - (Vector2)transform.position).normalized;

        if (phase < 3)
        {
            FireProjectile(dir, damage);
        }
        else
        {
            // フェーズ3：全方位8発
            for (int i = 0; i < 8; i++)
            {
                float angle = i * 45f;
                var d = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad),
                                    Mathf.Sin(angle * Mathf.Deg2Rad));
                FireProjectile(d, damage);
            }
        }
    }

    void FireProjectile(Vector2 dir, float damage)
    {
        var proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        if (proj.TryGetComponent<EnemyProjectile>(out var ep))
            ep.Initialize(dir, damage);
    }

    IEnumerator BurstAttack()
    {
        while (!isDead && phase == 3)
        {
            yield return new WaitForSeconds(5f);
            if (TankController.Instance == null) continue;
            // 突進
            var dir = ((Vector2)TankController.Instance.transform.position
                       - rb.position).normalized;
            rb.linearVelocity = dir * p3MoveSpeed * 3f;
            yield return new WaitForSeconds(0.5f);
        }
    }

    void SpawnMinions(int count)
    {
        if (spawnData?.prefab == null) return;
        for (int i = 0; i < count; i++)
        {
            var pos = (Vector2)transform.position + Random.insideUnitCircle.normalized * 3f;
            var go  = Instantiate(spawnData.prefab, pos, Quaternion.identity);
            go.GetComponent<EnemyController>()?.Initialize(spawnData);
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        hp -= amount;
        if (hp <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        if (scrapPrefab != null)
        {
            int count = Random.Range(scrapDropMin, scrapDropMax + 1);
            for (int i = 0; i < count; i++)
            {
                var pos = (Vector2)transform.position + Random.insideUnitCircle * 3f;
                Instantiate(scrapPrefab, pos, Quaternion.identity).gameObject.SetActive(true);
            }
        }
        BattleSceneController.Instance?.OnGoalReached(); // ボス撃破でクリア
        Destroy(gameObject);
    }
}
