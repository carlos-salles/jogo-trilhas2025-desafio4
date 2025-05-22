using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    float launchForceMax;
    [SerializeField]
    float damage;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject gameObject = collision.gameObject;
        if (gameObject.CompareTag("Enemy"))
        {
            gameObject.GetComponent<Health>().takeDamage(damage);
        }
    }

    public void LaunchProjectile(Vector2 launchDirection, float launchIntensity)
    {
        Vector2 force = launchForceMax * launchIntensity * launchDirection;
        rb.AddForce(force, ForceMode2D.Impulse);
    }

}
