using System;
using UnityEngine;

public class WallOfLightShield : MonoBehaviour
{
    [Header("Shield Settings")]
    public float maxShieldHP = 250f;
    public float currentShieldHP;

    private SolarisController solaris;

    

    public Action OnShieldBroken;

    private Collider shieldCollider;

    private void OnEnable()
    {
        currentShieldHP = maxShieldHP;
        solaris = GetComponentInParent<SolarisController>();
    }

    public void TakeDamage(float damage)
    {
        currentShieldHP -= damage;

        if (currentShieldHP <= 0f)
        {
            BreakShield();
        }
    }

    public void BreakShield() 
    {
        gameObject.SetActive(false);

        if (solaris != null)
        {
            solaris.OnWallBroken();
        }
    }
}
