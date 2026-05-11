using UnityEngine;

public class DestructibleWall : MonoBehaviour, IDamageable
{
    float hp;

    public void Initialize(float initialHp) => hp = initialHp;

    public void TakeDamage(float amount)
    {
        hp -= amount;
        if (hp <= 0) Destroy(gameObject);
    }
}
