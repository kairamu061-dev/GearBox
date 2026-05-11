using UnityEngine;

public class TowerBehaviour : MonoBehaviour
{
    public TowerData Data { get; private set; }
    public TowerInstance Instance { get; private set; }

    IAttackBehaviour attackBehaviour;
    float cooldownTimer;
    bool ctActive;

    public void Initialize(TowerInstance instance)
    {
        Instance = instance;
        Data = instance.data;
        cooldownTimer = 0f;
        ctActive = false;
        attackBehaviour = AttackBehaviourFactory.Create(Data.attackType);
        attackBehaviour?.SetOwner(this);
    }

    void Update()
    {
        if (attackBehaviour == null) return;

        if (ctActive)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                attackBehaviour.Execute();
                // AutoAimAttack が敵なしの場合はctActiveをfalseに戻す
                if (attackBehaviour is AutoAimAttack aa && !aa.HasTarget)
                    ctActive = false;
                else
                    cooldownTimer = Data.cooldown;
            }
        }
        else
        {
            // CT未開始：AutoAimは敵が入ったら即発射
            if (attackBehaviour is AutoAimAttack autoAim)
            {
                if (autoAim.TryInstantFire())
                {
                    ctActive = true;
                    cooldownTimer = Data.cooldown;
                }
            }
            else
            {
                // 照準型・範囲型等はCT開始
                ctActive = true;
                cooldownTimer = Data.cooldown;
            }
        }
    }
}
