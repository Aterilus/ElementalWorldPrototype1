using UnityEngine;

public class MeteorProjectile : MonoBehaviour
{
    private Vector3 targetPoint;
    private float fallSpeed;
    private float impactDamage;
    private float impactRadius;
    private LayerMask playerMask;

    public void Init(Vector3 target, float speed, float dmg, float radius, LayerMask mas)
    {
        targetPoint = target;
        fallSpeed = speed;
        impactDamage = dmg;
        impactRadius = radius;
        playerMask = mas;
    }

    private void Update()
    {
        Vector3 dir = (targetPoint - transform.position);
        float dist  = dir.magnitude;

        if (dist <= 0.3f)
        {
            Impact();
            return;
        }

        dir = dir.normalized;
        transform.position += dir * fallSpeed * Time.deltaTime;
    }

    private void Impact()
    {
        Collider[] hits = Physics.OverlapSphere(targetPoint, impactRadius, playerMask);

        for (int i = 0; i < hits.Length; ++i)
        {
            var hp = hits[i].GetComponentInParent<Health>();
            if (hp != null)
            {
                hp.TakeDamage(impactDamage);
                break;
            }
        }

        Destroy(gameObject);
    }
}
