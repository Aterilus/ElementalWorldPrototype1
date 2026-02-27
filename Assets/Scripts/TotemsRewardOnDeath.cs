using UnityEngine;

public class TotemsRewardOnDeath : MonoBehaviour 
{
    [SerializeField] private Health health;
    [SerializeField] private int totemsToAward = 1;

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
        if(health != null)
        {
            health.OnDied -= HandleDied;
        }
    }

    private void HandleDied(Health health)
    {
        if (PlayerProgress.Instance != null)
        {
            PlayerProgress.Instance.AddTotem(totemsToAward);
            Debug.Log($"[TotemRewardOnDeath] Awarded {totemsToAward}. Total = {PlayerProgress.Instance.TotemsCollected}");
        }
        else
        {
            Debug.LogWarning("[TotemRewardOnDeath] PlayerProgress not found.");
        }
    }
}
