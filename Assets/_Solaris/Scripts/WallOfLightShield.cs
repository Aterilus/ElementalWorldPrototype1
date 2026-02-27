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
    private bool isActive;

    private void OnEnable()
    {
        currentShieldHP = maxShieldHP;
        solaris = GetComponentInParent<SolarisController>();
    }

    public void TakeShieldDamage(float damage)
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

    public void Active(float newShieldHP = -1f)
    {
        if (newShieldHP > 0f)
        {
           maxShieldHP = newShieldHP;
        }
        currentShieldHP = maxShieldHP;
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void SetActive(bool active)
    {
        isActive = active;

        var collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = active;
        }
    }
}
