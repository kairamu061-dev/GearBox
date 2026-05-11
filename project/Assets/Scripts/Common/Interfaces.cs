using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float amount);
}

public interface IEnemyAI
{
    void Initialize(EnemyData data);
    void UpdateAI(Transform tankTransform);
}

public interface IAttackBehaviour
{
    void Execute();
    void SetOwner(TowerBehaviour owner);
}
