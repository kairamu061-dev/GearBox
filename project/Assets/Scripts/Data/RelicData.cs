using UnityEngine;

public enum RelicEffectType
{
    // 今後追加
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
