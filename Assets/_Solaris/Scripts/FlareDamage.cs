using UnityEngine;

public class FlareDamage : MonoBehaviour
{
    public int damageAmount = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        var playerHealth = other.GetComponentInParent<Health>();

        if (playerHealth == null)
        {
            return;
        }

        playerHealth.TakeDamage(damageAmount);
        Debug.Log("Player hit by flare for " + damageAmount);
    }
}
