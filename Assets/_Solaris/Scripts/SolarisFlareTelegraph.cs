using UnityEngine;

public class SolarisFlareTelegraph : MonoBehaviour
{
    public GameObject flareMarkerPrefab;
    public float warningDuration = 1.5f;
    public int flareCount = 3;
    public float radius = 6f;

    private Transform player;

    public void TriggerFlare(Transform playerTransform)
    {
        player = playerTransform;
        StartCoroutine(SpawnFlares());
    }

    System.Collections.IEnumerator SpawnFlares() {
        for (int i = 0; i < flareCount; ++i) { 
            Vector3 randomPos = player.position + Random.insideUnitSphere * radius;
            randomPos.y = 0f; // Keep on ground level

            GameObject marker = Instantiate(flareMarkerPrefab, randomPos, Quaternion.identity);

            Destroy(marker, warningDuration);
        }  
        yield return null;
    }
}
