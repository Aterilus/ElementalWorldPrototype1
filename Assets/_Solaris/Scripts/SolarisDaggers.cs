using UnityEngine;

public class SolarisDaggers : MonoBehaviour
{
    public GameObject daggerPrefab;
    public Transform daggerSpawnPoint;
    public float daggerCooldown = 4f;

    private float lastFireTime = -999f;

    public void TryFire(Transform player)
    {
        Debug.Log("TryFire called");

        Transform found = transform.Find("DaggerSpawnPoint");
        if (found != null)
        {
            daggerSpawnPoint = found;
            Debug.Log("Auto-found DaggerSpawnPoint on Solaris.");
        }
        else
        {
            GameObject go = new GameObject("DaggerSpawnPoint");
            go.transform.SetParent(transform);
            go.transform.localPosition = new Vector3(0f, 1.5f, 1.5f);

            daggerSpawnPoint = go.transform;
            Debug.LogWarning("Created DaggerSpawnPoint on Solaris.");

        }

        if (daggerSpawnPoint == null)
        {
            Debug.LogError("Dagger spawn point isn't assigned");
            return;
        }

        GameObject daggerInstance = Instantiate(daggerPrefab, daggerSpawnPoint.position, daggerSpawnPoint.rotation);

        DaggerProjectile daggerProjectile = daggerInstance.GetComponent<DaggerProjectile>();
        if (daggerProjectile != null)
        {
            daggerProjectile.SetTarget(player);
            Debug.Log("Dagger projectile target set to player");
        }
        else
        {
            Debug.LogError("DaggerPrefab does not have a DaggerProjectile component");
        }

        daggerSpawnPoint.transform.SetParent(null);

        Debug.Log("Dagger spawned at " + daggerSpawnPoint.position);
    }
}
