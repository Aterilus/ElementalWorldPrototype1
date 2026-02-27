using UnityEngine;
using System.Collections.Generic;

public class RoamingCalamityManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask groundMask;

    [SerializeField] private List<CalamityDefinition> calamities = new();

    private void Start()
    {
        if (!player)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        SpawnAllCalamities();
    }

    private void SpawnAllCalamities()
    {
        if (!player || CalamityWorldBounds.Instance == null)
        {
            return;
        }

        foreach (var calamity in calamities)
        {
            if (!calamity || !calamity.prefab) continue;

            Vector3? pos = FindValidSpawn(calamity);
            if (pos == null)
            {
                Debug.LogWarning($"Failed to spawn {calamity.calamityName}. Check groundMask/layer/bounds.");
                continue;
            }

            GameObject go = Instantiate(calamity.prefab, pos.Value, Quaternion.identity);

            var ctrl = go.GetComponent<RoamingCalamityController>();
            if (ctrl) { ctrl.Initialize(calamity, player, groundMask); }
        }
    }

    private Vector3? FindValidSpawn(CalamityDefinition calamity)
    {
        for (int attemt = 0; attemt < 30; attemt++)
        {
            Vector3 candiidate = CalamityWorldBounds.Instance.GetRandomPointHigh();

            if (Physics.Raycast(candiidate, Vector3.down, out RaycastHit hit, 2000f, groundMask))
            {
                float d = Vector3.Distance(hit.point, player.position);
                if (d < calamity.minDistanceFromPlayer) { continue; }
                if (d > calamity.maxDistanceFromPlayer) { continue; }

                return hit.point;
            }
        }

        return null;
    }
}
