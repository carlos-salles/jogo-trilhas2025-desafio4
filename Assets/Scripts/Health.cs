using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField, Min(0f), Tooltip("Vida inicial. Se o valor for 0, será utilizado maxHealth")]
    float health;
    [SerializeField, Min(0f)]
    float maxHealth;
    [SerializeField]
    Timer invencibilityTimer;
    [SerializeField]
    Image healthBar;

    public UnityEvent onDeath;
    public UnityEvent onDamage;
    public UnityEvent invencibilityFinished;

    public bool IsInvincible { get => invencibilityTimer?.IsRunning ?? false; }
    public bool IsAlive { get => health > 0; }

    // Start is called before the first frame update
    void Start()
    {
        if (CompareTag("Player"))
        {
            healthBar.fillAmount = health / maxHealth;
        }

        if (health == 0 || health > maxHealth)
        {
            health = maxHealth;
        }
        invencibilityTimer?.onFinished?.AddListener(() => invencibilityFinished?.Invoke());
        invencibilityFinished?.AddListener(() => Debug.Log("FINISHED"));
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log($"{transform.name} -> {(IsInvincible? "Invincible": "not invincible")}");
        if (CompareTag("Player"))
        {
            healthBar.fillAmount = health / maxHealth;
        }
    }

    public void takeDamage(float damage)
    {
        if (!IsAlive || IsInvincible) { return; }

        health -= damage;
        if (health > 0)
        {
            invencibilityTimer?.StartTimer();
            onDamage?.Invoke();
        }
        else
        {
            onDeath?.Invoke();
        }
    }
}
