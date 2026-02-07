using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int spawnCount = 5;

    [ContextMenu("Spawn Enemies")]
    public void SpawnEnimies()
    {
        if (enemyPrefab == null || spawnPoints == null || spawnPoints.Length == 0) { return; }

        for (int i = 0; i < spawnCount; ++i)
        {
            Transform p = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(enemyPrefab, p.position, p.rotation);
        }
    }
}
