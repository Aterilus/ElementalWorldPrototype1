using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int enemiesToSpawn = 2;
    public float spawnDelay = 0.5f;

    private void Start()
    {
        SpawnEnimies();
    }
    public void SpawnEnimies()
    {
        if (enemyPrefab == null || spawnPoints.Length == 0) { return; }

        for (int i = 0; i < enemiesToSpawn; ++i)
        {
            Transform p = spawnPoints[i % spawnPoints.Length];
            Instantiate(enemyPrefab, p.position, p.rotation);
        }
    }
}
