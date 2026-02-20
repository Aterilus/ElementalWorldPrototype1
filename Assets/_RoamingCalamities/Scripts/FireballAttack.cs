using UnityEngine;

public class FireballAttack : MonoBehaviour, ICalamityAttack
{
    public Transform spawnPoint;
    public FireballProjectile projectilePrefab;
    public LayerMask playerMask;
    public int lethalDamage = 999;

    public void Execute(Transform caster, Transform player, bool allowLethal)
    {
        if(!projectilePrefab) { return; }

        Transform sp = spawnPoint ? spawnPoint : caster;
        var proj = Instantiate(projectilePrefab, sp.position, sp.rotation);
        proj.lethalDamage = lethalDamage;
        proj.allowLethal = allowLethal;
        proj.playerMask = playerMask;
    }
}
