using System;
using UnityEngine;

public class EVRewardOnDeath : MonoBehaviour
{
    public int evPointsAwarded = 2;

    private Health health;

    private void Awake()
    {
        health = GetComponent<Health>();
        if (health == null)
        {
            Debug.LogError($"[EVRewardOnDeath] No Health component found on {gameObject.name}. This script requires a Health component.");
            enabled = false;
            return;
        }

        health.OnDied += HandleDeath;
    }

    private void OnDestroy()
    {
        if (health != null)
        {
            health.OnDied -= HandleDeath;
        }
    }

    private void HandleDeath(Health health)
    {
        if (PlayerProgress.Instance != null)
        {
            PlayerProgress.Instance.AddEvPoints(evPointsAwarded);
        }
        else { 
            Debug.LogWarning($"[EVRewardOnDeath] PlayerProgress instance not found. Cannot award EV points for {gameObject.name}'s death.");
        }
    }
}

