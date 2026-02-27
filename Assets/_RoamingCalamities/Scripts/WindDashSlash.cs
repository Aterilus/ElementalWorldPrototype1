using System.Collections;
using UnityEngine;

public class WindDashSlash : MonoBehaviour, ICalamityAttack
{
    [Header("Dash")]
    public float dashDistance = 6f;
    public float dashDuration = 0.15f;

    [Header("Hit")]
    public float hitRadius = 2.5f;
    public int lethalDamage = 999;
    public LayerMask playerMask;

    private bool dashing;

    [Header("Visuals")]
    public GameObject slashVFXPrefab;

    public void Execute(Transform caster, Transform player, bool allowLethal)
    {
        Debug.Log("WIND Execute called.");
        if (dashing)
        {
            return;
        }

        StartCoroutine(DashThenHit(caster, allowLethal));
    }

    private System.Collections.IEnumerator DashThenHit(Transform caster, bool allowLethal)
    {
        dashing = true;

        Vector3 start = caster.position;
        Vector3 end = start + caster.forward * dashDistance;

        float t = 0f;
        while (t < dashDuration)
        {
            t += Time.deltaTime;
            caster.position = Vector3.Lerp(start, end, t / dashDuration);
            yield return null;
        }

        if (slashVFXPrefab)
        {
            Vector3 spawnPos = caster.position + caster.forward * 1.5f;
            Instantiate(slashVFXPrefab, spawnPos, caster.rotation);
        }

        Collider[] hits = Physics.OverlapSphere(caster.position + caster .forward * 1.2f, hitRadius, playerMask);
        foreach (Collider hit in hits)
        {
            Health health = hit.GetComponent<Health>();
            if (health != null)
            {
                DamagerHelper.ApplyDamage(health, lethalDamage, allowLethal, 1);
            }
        }

        dashing = false;
    }
}
