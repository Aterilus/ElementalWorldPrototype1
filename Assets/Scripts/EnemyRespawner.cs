using UnityEngine;

public class EnemyRespawner : MonoBehaviour 
{
    [Header("Spawner")]
    public GameObject enemyPrefab;
    public bool spawnOnStart = true;

    [Header("Respawn")]
    public float respawnDelay = 5f;
    public int maxAliveFromThisSpawner = 3;

    private int aliveCount;

    private void Start()
    {
        if (spawnOnStart)
        {
            TrySpawn();
        }
    }

    public void TrySpawn()
    {
        if (enemyPrefab == null) { return; }
        if (aliveCount >= maxAliveFromThisSpawner) { return; }

        GameObject enemyInstance = Instantiate(enemyPrefab, transform.position, transform.rotation);
        aliveCount++;

        EnemyDeath enemyDeath = enemyInstance.GetComponentInChildren<EnemyDeath>();
        if (enemyDeath != null)
        {
            enemyDeath.spawner = this;
        }
    }

    public void NotifyEnemyDied(EnemyDeath enemyDeath)
    {
        aliveCount = Mathf.Max(0, aliveCount - 1);
        Invoke(nameof(TrySpawn), respawnDelay);
    }
}
