using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth;  
    public int currentHealth;
    internal readonly bool IsDead;

    private bool isDead => currentHealth <= 0f;

    void Awake()
    {
        currentHealth = maxHealth; 
    }

    // Method to apply damage to the player
    public void TakeDamage(int damage)
    {
        if (isDead) return; // If already dead, do nothing
        currentHealth = Mathf.Max(currentHealth - damage, 0, maxHealth); // Decrease current health by damage amount, not going below 0
    }

    // Method to heal the player
    public void Heal(int amount)
    {
        if (isDead) return; // If already dead, do nothing
        currentHealth = Mathf.Min(currentHealth + amount, 0, maxHealth); // Increase current health by heal amount, not exceeding max health
    }
}
