using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class TankController : MonoBehaviour, IDamageable
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] Transform towerMount;

    Rigidbody2D rb;
    Camera mainCam;

    public static TankController Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Start() => SpawnTowers();

    void Update()
    {
        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        rb.velocity = input * moveSpeed;
    }

    void SpawnTowers()
    {
        var rm = RunManager.Instance;
        for (int x = 0; x < rm.GridSize.x; x++)
        {
            for (int y = 0; y < rm.GridSize.y; y++)
            {
                var inst = rm.GridLayout[x, y];
                if (inst?.data?.prefab == null) continue;
                var go = Instantiate(inst.data.prefab, towerMount);
                go.GetComponent<TowerBehaviour>()?.Initialize(inst);
            }
        }
    }

    public void TakeDamage(float amount)
    {
        RunManager.Instance.TakeDamage((int)amount);
        if (RunManager.Instance.CurrentHp <= 0)
            BattleSceneController.Instance?.OnGameOver();
    }
}
