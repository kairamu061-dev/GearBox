using UnityEngine;

public class TowerBehaviour : MonoBehaviour
{
    public TowerData Data { get; private set; }
    public TowerInstance Instance { get; private set; }

    IAttackBehaviour attackBehaviour;
    float cooldownTimer;
    bool ctActive;

    float cooldownMultiplier = 1f;

    public void Initialize(TowerInstance instance)
    {
        Instance = instance;
        Data = instance.data;
        cooldownTimer = 0f;
        ctActive = false;
        attackBehaviour = AttackBehaviourFactory.Create(Data.attackType);
        attackBehaviour?.SetOwner(this);
        Log.Info($"Initialize: {Data.displayName}, cooldown={Data.cooldown}, type={Data.attackType}, behaviour={attackBehaviour}", LogTag.Tower);
    }

    public void ApplyCooldownMultiplier(float mult) => cooldownMultiplier = mult;

    void Update()
    {
        if (attackBehaviour == null) return;

        if (ctActive)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                Log.Info($"{Data?.displayName} 攻撃実行", LogTag.Tower, LogTag.Battle);
                attackBehaviour.Execute();
                // AutoAimAttack が敵なしの場合はctActiveをfalseに戻す
                if (attackBehaviour is AutoAimAttack aa && !aa.HasTarget)
                    ctActive = false;
                else
                    cooldownTimer = Data.cooldown * cooldownMultiplier;
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
                    cooldownTimer = Data.cooldown * cooldownMultiplier;
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
