using UnityEngine;
using System.Collections.Generic;

public class SpawnZone : MonoBehaviour
{
    [Header("Spawn")]
    public GameObject prefabToSpawn;
    public int minSpawnCount = 1;
    public int maxSpawnCount = 3;
    public float spawnRadius = 6f;

    [Header("Activation")]
    public Transform player;
    public float activateDistance = 25f;
    public float deactivationDistance = 50f;

    [Header("Limits")]
    public int maxAliveFromThisZone = 4;

    private readonly List<GameObject> alive = new();

    private void Update()
    {
        if(!player) {  return; }

        float d = Vector3.Distance(player.position, gameObject.transform.position);

        alive.RemoveAll(x => x == null);

        if (d <= activateDistance)
        {
            TrySpawn();
        }
        else if (d >= deactivationDistance)
        {
            DespawnAll();
        }
    }

    void TrySpawn()
    {
        if (!prefabToSpawn) { return; }
        if (alive.Count >= maxAliveFromThisZone) { return; }

        int target = Random.Range(minSpawnCount, maxSpawnCount + 1);
        int needed = Mathf.Clamp(target - alive.Count, 0, maxAliveFromThisZone - alive.Count);

        for (int i = 0; i < needed; ++i)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            pos.y = transform.position.y;

            var go = Instantiate(prefabToSpawn, pos, Quaternion.identity);
            alive.Add(go);
        }
    }

    void DespawnAll()
    {
        for (int i = 0; i < alive.Count; ++i)
        {
            if (alive[i])
            {
                Destroy(alive[i]);
            }
        }

        alive.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, activateDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, deactivationDistance);
    }
}
