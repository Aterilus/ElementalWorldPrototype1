using UnityEngine;

public class EnemyDeath : MonoBehaviour
{
    [Header("Optional: quick feedback")]
    public float despawnDelay = 0f;

    private Health health;

    [HideInInspector] public EnemyRespawner spawner;

    private void Awake()
    {
        health = GetComponent<Health>();

        if (health != null)
        {
            health.OnDied += HandleDied;
        }
    }

    private void OnDestroy()
    {
        if (health != null)
        {
            health.OnDied -= HandleDied;
        }
    }

    private void HandleDied(Health health)
    {
        if (spawner != null)
        {
            spawner.NotifyEnemyDied(this);
        }
        if (despawnDelay > 0f)
        {
            Destroy(gameObject, despawnDelay);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
