using UnityEngine;

public class Stats : MonoBehaviour 
{
    [Header("EV Points")]
    public int evPoints = 0;

    [Header("EV Allocations")]
    public int evHP = 0;
    public int evAttack = 0;
    public int evDefense = 0;
    public int evSpeed = 0;

    [Header("Derived Status")]
    public int bonusMaxHP;
    public float damageMultiplier = 1.0f;
    public float damageReduction = 0f;
    public float moveSpeedBonuses = 0f;

    public void Recalculate()
    {
        bonusMaxHP = evHP * 5;
        damageMultiplier = 1f + (evAttack * 0.02f);
        damageReduction = Mathf.Clamp01(evDefense * 0.01f);
        moveSpeedBonuses = evSpeed * 0.05f;
    }
}
