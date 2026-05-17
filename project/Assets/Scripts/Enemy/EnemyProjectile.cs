using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class EnemyProjectile : MonoBehaviour
{
    float damage;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        GetComponent<CircleCollider2D>().isTrigger = true;
        GetComponent<CircleCollider2D>().radius = 0.15f;
    }

    public void Initialize(Vector2 dir, float dmg, float spd = 8f)
    {
        damage = dmg;
        rb.linearVelocity = dir * spd;
        StartCoroutine(AutoDestroy(5f));
    }

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
