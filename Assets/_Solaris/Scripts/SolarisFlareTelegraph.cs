using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
public class SolarisFlareTelegraph : MonoBehaviour
{
    public GameObject flareMarkerPrefab;
    public GameObject flareDamagePrefab;

    public float warningDuration = 1.5f;
    public float damageDuration = 0.5f;

    public int flareCount = 5;
    public float radius = 6f;

    public void TriggerFlare(Transform playerTransform)
    {
        StartCoroutine(FlareSequence(playerTransform));
    }

    IEnumerator FlareSequence(Transform player)
    {
        List<Vector3> positions = new List<Vector3>();

        for(int i = 0; i < flareCount; ++i)
        {
            Vector3 p = player.position + Random.insideUnitSphere * radius;
            p.y = 0f;
            positions.Add(p);
        }
        List<GameObject> markers = new List<GameObject>();
        foreach (var pos in positions)
        {
           GameObject m = Instantiate(flareMarkerPrefab, pos, Quaternion.identity);
           markers.Add(m);
        }

        yield return new WaitForSeconds(warningDuration);

        foreach(var m in markers)
        {
            if (m != null)
                Destroy(m);
        }
        foreach (var pos in positions)
        {
           GameObject dz = Instantiate(flareDamagePrefab, pos, Quaternion.identity);
           Destroy(dz, damageDuration);
        }
    }
    
}
