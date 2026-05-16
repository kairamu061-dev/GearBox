using UnityEngine;

public enum RelicEffectType
{
    MaxHpUp,              // 最大HP増加
    ScrapBonus,           // スクラップ即時獲得
    MoveSpeedUp,          // 移動速度アップ（割合）
    AllCooldownReduction, // 全タワーCT短縮（割合）
    ScrapCollectRadius,   // スクラップ回収半径アップ
}

[CreateAssetMenu(fileName = "RelicData", menuName = "GearBox/RelicData")]
public class RelicData : ScriptableObject
{
    public string relicId;
    public string displayName;
    [TextArea] public string description;
    public Sprite icon;
    public RelicEffectType effectType;
    public float effectValue;
}
