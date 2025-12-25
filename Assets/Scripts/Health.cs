using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth;  // Maximum health
    public int currentHealth;    // Current health
    private bool isDead = false; // Flag to check if the player is dead

    void Start()
    {
        currentHealth = maxHealth; // Initialize current health to maximum health
    }

    // Method to apply damage to the player
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Reduce current health by damage amount

        if (currentHealth <= 0)
        {
            Destroy(gameObject); // Destroy the player GameObject if health drops to zero or below
            //playerDied(); // Call playerDied method if health drops to zero or below
        }
    }

    // Method to heal the player
    public void Heal(int amount)
    {
        currentHealth += amount; // Increase current health by heal amount
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth; // Cap current health at maximum health
        }
    }

    // Method to handle player death
    private void playerDied()
    {
        if (currentHealth <= 0)
        {
            // Additional death handling logic can be added here
            isDead = true;
            Debug.Log("Player has died.");
            
        }
    }
}
