using UnityEngine;

public enum AttackType
{
    Aimed,
    AutoAim,
    Area,
    Placement,
    Barrier,
    Intercept,
    Heal,
    Buff,
    Beam,
}

public enum DamageType
{
    Single,
    Area,
}

[CreateAssetMenu(fileName = "TowerData", menuName = "GearBox/TowerData")]
public class TowerData : ScriptableObject
{
    public string towerId;
    public string displayName;
    public AttackType attackType;
    public DamageType damageType;
    public Vector2Int size = Vector2Int.one;
    public float damage;
    public float cooldown;
    public float range;
    public int basePrice;
    public Sprite icon;
    public GameObject prefab;
    [TextArea] public string description;
}
