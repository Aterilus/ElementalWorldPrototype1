using UnityEngine;

public class CalamityAttackDriver : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int LethalDamage = 999;
    [SerializeField] private int mercyLeaveAtHP = 1;

    [Header("Hitbox")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float hitRadius = 2.5f;
    [SerializeField] private LayerMask playerMask;

    public void DoAttack(Transform player, bool allowLethal)
    {
        Vector3 center = attackPoint ? attackPoint.position : (transform.position + transform.forward * 1.5f);

        Collider[] hits = Physics.OverlapSphere(center, hitRadius, playerMask);

        foreach (Collider hit in hits)
        {
            var hp = hit.GetComponent<Health>();
            
            if (hp == null ) continue;

            int dmg = LethalDamage;

            if (!allowLethal)
            {
                int current = (int)hp.currentHealth;
                int maxAllowedDamage = Mathf.Max(0, current - mercyLeaveAtHP);
                dmg = Mathf.Clamp(dmg, 0, maxAllowedDamage);
            }

            if (dmg > 0)
            {
                hp.TakeDamage(dmg);
            }
        }
    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        Vector3 center = attackPoint ? attackPoint.position : (transform.position + transform.forward * 1.5f);

        Gizmos.color = new Color(1f, 0.9f, 0.2f, 0.25f);
        Gizmos.DrawSphere(center, hitRadius);
        Gizmos.color = new Color(1f, 0.9f, 0.2f, 0.85f);
        Gizmos.DrawWireSphere(center, hitRadius);
    }

#endif
}
