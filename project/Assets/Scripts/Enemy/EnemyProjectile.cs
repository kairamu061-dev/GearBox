using System.Collections;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    float damage;
    float speed = 8f;
    Vector2 direction;

    public void Initialize(Vector2 dir, float dmg, float spd = 8f)
    {
        direction = dir;
        damage = dmg;
        speed = spd;
        StartCoroutine(AutoDestroy(5f));
    }

    void Update() => transform.Translate(direction * speed * Time.deltaTime, Space.World);

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<TankController>(out var tank))
        {
            tank.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    IEnumerator AutoDestroy(float t)
    {
        yield return new WaitForSeconds(t);
        Destroy(gameObject);
    }
}
