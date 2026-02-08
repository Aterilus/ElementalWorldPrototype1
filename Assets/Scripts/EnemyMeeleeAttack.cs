using UnityEngine;

public class EnemyMeeleeAttack : MonoBehaviour
{
    [Header("Damage")]
    public float damageAmount = 10f;
    public float coolDownTime = 4f;

    [Header("Targeting")]
    public string targetTag = "Player";

    private float nextHitTime;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[Melee] ENTER hit: {other.name} (tag = {other.tag}, layer= {LayerMask.LayerToName(other.gameObject.layer)}");
        TryDamage(other);
    }
    private void OnTriggerStay(Collider other)
    {
        TryDamage(other);
    }

    private void TryDamage(Collider other)
    {
        if (Time.time < nextHitTime) { return; }
        
        bool tagOK = string.IsNullOrEmpty(targetTag) || other.CompareTag(targetTag) || 
            (other.transform.root != null && other.transform.root.CompareTag(targetTag));

        if (!tagOK) { return; }

        Health health = other.GetComponent<Health>();

        if (health == null) { health = other.GetComponentInParent<Health>(); }
        if (health == null) { health = other.transform.root.GetComponent<Health>(); }

        if (health == null)
        {
            Debug.LogWarning($"[Melee] No Health component found on {other.name} or its parents.");
            return;
        }

        Debug.Log($"[Melee] Damaging {other.name} for {damageAmount} damage.");
        health.TakeDamage(damageAmount);

        nextHitTime = Time.time + coolDownTime;
    }
}
