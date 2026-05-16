using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TankController : MonoBehaviour, IDamageable
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float collectRadius = 2f;
    [SerializeField] Transform towerMount;

    Rigidbody2D rb;
    float speedMultiplier = 1f;

    public static TankController Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        // 取得済みレリック効果を再適用
        foreach (var r in RunManager.Instance.RelicInventory)
            RelicEffectApplier.Apply(r, RunManager.Instance);
    }

    void Start() => SpawnTowers();

    void Update()
    {
        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        rb.linearVelocity = input * moveSpeed * speedMultiplier;
    }

    void SpawnTowers()
    {
        var rm = RunManager.Instance;
        for (int x = 0; x < rm.GridSize.x; x++)
            for (int y = 0; y < rm.GridSize.y; y++)
            {
                var inst = rm.GridLayout[x, y];
                if (inst?.data?.prefab == null) continue;
                var go = Instantiate(inst.data.prefab, towerMount);
                go.GetComponent<TowerBehaviour>()?.Initialize(inst);
            }
    }

    public void TakeDamage(float amount)
    {
        RunManager.Instance.TakeDamage((int)amount);
        if (RunManager.Instance.CurrentHp <= 0)
            BattleSceneController.Instance?.OnGameOver();
    }

    public void ApplySpeedBonus(float addRatio)      => speedMultiplier += addRatio;
    public void ApplyCollectRadiusBonus(float add)
    {
        collectRadius += add;
        var col = GetComponent<CircleCollider2D>();
        if (col) col.radius = collectRadius;
    }
}
