using UnityEngine;

public class Scene2Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public bool IsDead => currentHealth <= 0f;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        if (IsDead) return;
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    public void Heal(float healAmount)
    {
        if (IsDead) return;
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }
}
