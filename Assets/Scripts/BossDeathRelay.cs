using UnityEngine;

public class BossDeathRelay :MonoBehaviour
{
    [Header("Who gets notified when the boss dies?")]
    public WaveEncounterManager waveEncounterManager;

    private Health health;

    void Awake()
    {
        health = GetComponent<Health>();
        if (health == null)
        {
            Debug.LogError($"[BossDeathRelay] No Health component found on {gameObject.name}!");
            return;
        }

        health.OnDied += HandleMidBossDeath;
    }

    private void OnDestroy()
    {
        if (health != null)
        {
            health.OnDied -= HandleMidBossDeath;
        }
    }

    private void HandleMidBossDeath(Health health)
    {
        Debug.Log($"[BossDeathRelay] Boss {gameObject.name} has died. Notifying WaveEncounterManager.");
        if (waveEncounterManager != null)
        {
            waveEncounterManager.OnMidBossDeath();
        }
        else
        {
            Debug.LogWarning($"[BossDeathRelay] No WaveEncounterManager assigned to {gameObject.name}!");
        }
    }
}

