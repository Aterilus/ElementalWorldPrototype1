using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    public float damageAmount = 10f;
    
    private void OnTriggerEnter(Collider other)
    {
        HurtboxScript hb = other.GetComponent<HurtboxScript>();
        if (hb != null)
        {
            if (hb.healthComponent != null)
            {
                hb.healthComponent.TakeDamage(damageAmount);
            }
            else {

                Debug.LogWarning($"Hurtbox '{other.name}' has no health assigned in hurtbox component.");
            }

            return;
        }
        Health health = other.GetComponentInParent<Health>();
        Debug.Log(health != null
            ? $"Found Health component on {other.gameObject.name}."
            : $"No Health component found on {other.gameObject.name}.");

        if (health != null)
        {
            health.TakeDamage(damageAmount);
            return;
        }

        Debug.LogWarning($"DamageDealer hit '{other.name}' but found no hurtbox or health");
    }
}
