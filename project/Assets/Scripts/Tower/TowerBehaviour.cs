using UnityEngine;

public class TowerBehaviour : MonoBehaviour
{
    public TowerData Data { get; private set; }
    public TowerInstance Instance { get; private set; }

    IAttackBehaviour attackBehaviour;
    float cooldownTimer;

    public void Initialize(TowerInstance instance)
    {
        Instance = instance;
        Data = instance.data;
        cooldownTimer = 0f;
        attackBehaviour = AttackBehaviourFactory.Create(Data.attackType);
        attackBehaviour?.SetOwner(this);
    }

    void Update()
    {
        if (attackBehaviour == null) return;
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0f)
        {
            attackBehaviour.Execute();
            cooldownTimer = Data.cooldown;
        }
    }
}
