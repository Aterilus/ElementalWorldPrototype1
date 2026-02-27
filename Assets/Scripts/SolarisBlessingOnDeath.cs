using System;
using UnityEngine;

public class SolarisBlessingOnDeath : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private bool grantBlessingOnce = true;

    private bool granted = false;

    private void Awake()
    {
        if (health == null)
        {
            health = GetComponent<Health>();
        }
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.OnDied += HandleDied;
        }
    }

    private void OnDisable()
    {
        if (health != null) 
        {
            health.OnDied -= HandleDied;
        }
    }

    private void HandleDied(Health health)
    {
        if (grantBlessingOnce && granted)
        {
            return;
        }

        granted = true;

        if (PlayerProgress.Instance != null)
        {
            PlayerProgress.Instance.GrantSolarisBlessing();
        }
        else
        {
            Debug.LogWarning("[SolarisBlessingOnDeath] PlayerProgress not found.");
        }
    }
}
