using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour, IDamageable
{
    [SerializeField] ScrapObject scrapPrefab;

    EnemyData data;
    IEnemyAI ai;
    float currentHp;
    bool isDead;

    Rigidbody2D rb;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    public void Initialize(EnemyData enemyData)
    {
        data = enemyData;
        currentHp = data.maxHp;
        isDead = false;
        ai = EnemyAIFactory.Create(data.aiType, this, rb);
        ai?.Initialize(data);
    }

    void Update()
    {
        if (isDead || TankController.Instance == null) return;
        ai?.UpdateAI(TankController.Instance.transform);
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;
        currentHp -= amount;
        if (currentHp <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        DropScrap();
        gameObject.SetActive(false);
    }

    void DropScrap()
    {
        if (scrapPrefab == null || data == null) return;
        int count = Random.Range(data.scrapDropMin, data.scrapDropMax + 1);
        for (int i = 0; i < count; i++)
        {
            var pos = (Vector2)transform.position + Random.insideUnitCircle * 1.5f;
            var obj = Instantiate(scrapPrefab, pos, Quaternion.identity);
            obj.gameObject.SetActive(true);
        }
    }
}
