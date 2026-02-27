using UnityEngine;

public class IceNovaAttack : MonoBehaviour, ICalamityAttack
{
    public float radius = 6f;
    public int lethalDamage = 999;
    public LayerMask playerMask;

    [Header("Visuals")]
    public GameObject novaVFXPrefab;
    public float vfxLifetime = 1.5f;

    public void Execute(Transform caster, Transform player, bool allowLethal)
    {
        Debug.Log("ICE Execute called.");
        if (novaVFXPrefab)
        {
            var vfx = GameObject.Instantiate(novaVFXPrefab, caster.position, Quaternion.identity);
            GameObject.Destroy(vfx, vfxLifetime);
        }
        Collider[] hits = Physics.OverlapSphere(caster.position, radius, playerMask);
        foreach (Collider hit in hits)
        {
            Health hp = hit.GetComponent<Health>();
            if (hp != null)
            {
                DamagerHelper.ApplyDamage(hp, lethalDamage, allowLethal, 1);
            }
        }
    }
}
