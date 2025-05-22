using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [SerializeField, Min(0f), Tooltip("Vida inicial. Se o valor for 0, será utilizado maxHealth")]
    float health;
    [SerializeField, Min(0f)]
    float maxHealth;
    [SerializeField, Min(0f)]
    float invincinbilityTimeSeconds;
    
    public UnityEvent onDeath;

    float invincinbilityRemainingSeconds;
    public bool IsInvincible { get => invincinbilityRemainingSeconds > 0; }

    // Start is called before the first frame update
    void Start()
    {
        if (health == 0 || health > maxHealth)
        {
            health = maxHealth;
        }
    }

    // Update is called once per frame
    void Update()
    {
        invincinbilityTimeSeconds = Mathf.Max(0f, invincinbilityTimeSeconds - Time.deltaTime);
    }

    public void takeDamage(float damage)
    {
        if (IsInvincible) { return; }

        health -= damage;
        if (health > 0)
        {
            invincinbilityRemainingSeconds = invincinbilityTimeSeconds;
        }
        else
        {
            onDeath?.Invoke();   
        }
    }
}
