using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    float launchForceMax;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void LaunchProjectile(Vector2 launchDirection, float launchIntensity)
    {
        Vector2 force = launchForceMax * launchIntensity * launchDirection;
        rb.AddForce(force, ForceMode2D.Impulse);
    }
}
