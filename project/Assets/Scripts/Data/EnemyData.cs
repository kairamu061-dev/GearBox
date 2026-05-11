using UnityEngine;

public enum AIType { Chaser, Turret, Rusher, Fortress, Boss }

[CreateAssetMenu(fileName = "EnemyData", menuName = "GearBox/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public GameObject prefab;
    public float maxHp = 40f;
    public float moveSpeed = 3f;
    public float attackDamage = 10f;
    public float attackCooldown = 1f;
    public float attackRange = 1f;
    public AIType aiType;
    public int scrapDropMin = 5;
    public int scrapDropMax = 10;
    public float projectileSpeed = 8f;
    public GameObject projectilePrefab;
}
