using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public bool IsDead => currentHealth <= 0f;

    public event Action<Health> OnDied;

    private bool died;

    private void Awake()
    {
        if (currentHealth <= 0f) { currentHealth = maxHealth; }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10f);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (died) { return; }
        if (damageAmount <= 0f) { return; }

        Debug.Log($"[Health] TakeDamage called on {gameObject.name}. amount = {damageAmount} BEFORE = {currentHealth}");
        currentHealth -= damageAmount;

        Debug.Log($"[Health] AFTER = {currentHealth}");
        if (currentHealth <= 0f)         
        {
            currentHealth = 0f;
            Debug.Log($"[Health] {gameObject.name} is now dead.");
            Die();
        }
        //currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    public void Heal(float healAmount)
    {
        if (died) { return; }
        if (healAmount <= 0f) { return; }

        currentHealth += healAmount;
        if (currentHealth > maxHealth)         
        {
            currentHealth = maxHealth;
        }

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    private void Die()
    {
        if (died) { return; }
        died = true;
        OnDied?.Invoke(this);
        Destroy(gameObject);
    }
}
