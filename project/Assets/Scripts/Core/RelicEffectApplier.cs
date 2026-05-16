using UnityEngine;

// RunManager.AddRelic() から呼ばれてパッシブ効果を即時適用する
public static class RelicEffectApplier
{
    public static void Apply(RelicData relic, RunManager rm)
    {
        if (relic == null) return;
        switch (relic.effectType)
        {
            case RelicEffectType.MaxHpUp:
                rm.Heal((int)relic.effectValue);
                break;
            case RelicEffectType.ScrapBonus:
                rm.AddScrap((int)relic.effectValue);
                break;
            case RelicEffectType.MoveSpeedUp:
                // TankController が生きていれば即時反映
                TankController.Instance?.ApplySpeedBonus(relic.effectValue);
                break;
            case RelicEffectType.AllCooldownReduction:
                // バトル中のタワーに適用（準備フェーズ外では TowerBehaviour が存在しない）
                foreach (var t in Object.FindObjectsByType<TowerBehaviour>(FindObjectsSortMode.None))
                    t.ApplyCooldownMultiplier(1f - relic.effectValue);
                break;
            case RelicEffectType.ScrapCollectRadius:
                TankController.Instance?.ApplyCollectRadiusBonus(relic.effectValue);
                break;
        }
    }
}
