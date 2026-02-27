using UnityEngine;

public static class DamagerHelper
{
    public static void ApplyDamage(Health hp, int lethalDamage, bool allowLethal, int mercyLevelAtHP = 1)
    {
        if (hp == null) { return; }

        int dmg = lethalDamage;

        if (!allowLethal)
        {
            int current = (int)hp.currentHealth;
            int maxAllowed = Mathf.Max(0, current - mercyLevelAtHP);
            dmg = Mathf.Clamp(dmg, 0, maxAllowed);
        }

        if (dmg > 0)
        {
            hp.TakeDamage(dmg);
        }
    }
}
