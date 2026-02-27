using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // Method for trigger enter
    private void OnTriggerEnter(Collider other)
    {
        // Check if the other object has the tag "Enemy"
        if (other.CompareTag("Enemy"))
        {
            // Log a message to the console
            Debug.Log("Enemy hit!");
            Debug.Log("Hit: " + other.name);
            // Call the TakeDamage method on the enemy's Health component
            
            if (other.CompareTag("Enemy"))
            {
                Health enemyHealth = other.GetComponent<Health>();
                enemyHealth.TakeDamage(10);
            }
        }
    }
}
