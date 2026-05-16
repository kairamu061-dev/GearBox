using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FortressController : MonoBehaviour, IDamageable
{
    [Header("パラメータ")]
    [SerializeField] float maxHp            = 250f;
    [SerializeField] float detectRadius     = 12f;
    [SerializeField] float attackCooldown   = 2f;
    [SerializeField] float attackDamage     = 20f;
    [SerializeField] float spawnCooldown    = 5f;
    [SerializeField] int   spawnLimit       = 4;

    [Header("参照")]
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] EnemyData  spawnEnemyData;
    [SerializeField] ScrapObject scrapPrefab;

    [Header("ドロップ")]
    [SerializeField] int scrapDropMin = 60;
    [SerializeField] int scrapDropMax = 80;

    float hp;
    bool isDead;
    bool isActive;
    int  spawnedCount;

    void Start() => hp = maxHp;

    void Update()
    {
        if (isDead) return;
        if (TankController.Instance == null) return;

        float dist = Vector2.Distance(transform.position,
                                      TankController.Instance.transform.position);
        isActive = dist <= detectRadius;
    }

    void OnEnable()
    {
        StartCoroutine(AttackLoop());
        StartCoroutine(SpawnLoop());
    }

    IEnumerator AttackLoop()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(attackCooldown);
            if (!isActive || projectilePrefab == null) continue;
            if (TankController.Instance == null) continue;

            var dir = (TankController.Instance.transform.position - transform.position).normalized;
            var proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            if (proj.TryGetComponent<EnemyProjectile>(out var ep))
                ep.Initialize((Vector2)dir, attackDamage);
        }
    }

    IEnumerator SpawnLoop()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(spawnCooldown);
            if (!isActive || spawnEnemyData?.prefab == null) continue;
            if (spawnedCount >= spawnLimit) continue;

            var pos = (Vector2)transform.position + Random.insideUnitCircle.normalized * 2f;
            var go  = Instantiate(spawnEnemyData.prefab, pos, Quaternion.identity);
            go.GetComponent<EnemyController>()?.Initialize(spawnEnemyData);
            spawnedCount++;
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
        DropScrap();
        Destroy(gameObject);
    }

    void DropScrap()
    {
        if (scrapPrefab == null) return;
        int count = Random.Range(scrapDropMin, scrapDropMax + 1);
        for (int i = 0; i < count; i++)
        {
            var pos = (Vector2)transform.position + Random.insideUnitCircle * 2f;
            Instantiate(scrapPrefab, pos, Quaternion.identity).gameObject.SetActive(true);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
