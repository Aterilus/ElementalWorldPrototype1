using System.Collections;
using UnityEngine;

public class BurningTrail : MonoBehaviour
{
    private float damagePerTick;
    private float tickRate;
    private float lifetime;
    private LayerMask playerMask;

    private Transform followTarget;

    public void Init(float dmgPerTick, float tick, float life, LayerMask mask)
    {
        damagePerTick = dmgPerTick;
        tickRate = tick;
        lifetime = life;
        playerMask = mask;
    }

    public void Follow(Transform target)
    {
        followTarget = target;
    }

    private void Start()
    {
        StartCoroutine(Tick());
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (followTarget != null)
        {
            Vector3 back = -followTarget.forward;
            back.y = 0f;
            transform.position = followTarget.position + back.normalized * 1.0f;
        }
    }

    private IEnumerator Tick()
    {
        float end = Time.time + lifetime;
        
        while (Time.time < end)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 2.0f, playerMask);
            for (int i = 0; i < hits.Length; ++i)
            {
                Health playerHP = hits[i].GetComponentInParent<Health>();
                if (playerHP != null)
                {
                    playerHP.TakeDamage(damagePerTick);
                    break;
                }
            }
            yield return new WaitForSeconds(tickRate);
        }
    }
}
