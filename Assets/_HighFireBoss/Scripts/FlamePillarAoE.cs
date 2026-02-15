using UnityEngine;
using System.Collections;

public class FlamePillarAoE : MonoBehaviour
{
    public float tickRate;
    public float damagePerTick;
    public float lifetime;
    public float radius;

    public LayerMask playerMask;

    public void Init(float dmgTick, float tick, float life, float rad, LayerMask mask)
    {
        damagePerTick = dmgTick;
        tickRate = tick;
        lifetime = life;
        radius = rad;
        playerMask = mask;
    }

    private void Start()
    {
        StartCoroutine(TickDamage());
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
                    playerHealth.TakeDamage(damagePerTick);
                    break;
                }
            }

            yield return new WaitForSeconds(tickRate);
        }
    }
}
