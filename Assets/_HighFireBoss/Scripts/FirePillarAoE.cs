using UnityEngine;
using System.Collections;

public class FirePillarAoE : MonoBehaviour
{
    public float tickRate = 0.25f;
    public float damagePerTick = 8f;
    public float lifetime = 2.2f;
    public float radius = 2.0f;

    public LayerMask playerMask;

    private float damageMultiplier = 1f;

    public void Init(float dmgTick, float tick, float life, float rad, float multiplier)
    {
        damagePerTick = dmgTick;
        tickRate = tick;
        lifetime = life;
        radius = rad;
        damageMultiplier = multiplier;
    }

    private void Start()
    {
        StartCoroutine(TickDamage());
        Destroy(gameObject, lifetime);
    }

    private IEnumerator TickDamage()
    {
        float endTime = Time.time + lifetime;

        while (Time.time < endTime)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, radius, playerMask);

            foreach (Collider hit in hits)
            {
                Health playerHealth = hit.GetComponentInParent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damagePerTick * damageMultiplier);
                    break;
                }
            }

            yield return new WaitForSeconds(tickRate);
        }
    }
}
