using UnityEngine;

public class SolarisDaggers : MonoBehaviour
{
    public GameObject daggerPrefab;

    [SerializeField] private Transform daggerSpawnPoint;
    public float daggerCooldown = 4f;

    private float lastFireTime = -999f;

    private void Awake()
    {
        EnsureSpawnPoint();
    }

    void EnsureSpawnPoint()
    {
        if (daggerSpawnPoint == null)
        {
            Transform found = transform.Find("DaggerSpawnPoint");
            if (found != null)
            {
                daggerSpawnPoint = found;
                Debug.Log("Auto-found DaggerSpawnPoint on Solaris.");
                return;
            }
            else
            {
                GameObject go = new GameObject("DaggerSpawnPoint");
                go.transform.SetParent(transform);
                go.transform.localPosition = new Vector3(0f, 1.5f, 1.5f);

                daggerSpawnPoint = go.transform;
                Debug.LogWarning("Created DaggerSpawnPoint on Solaris.");
            }
        }
    }
    public void TryFire(Transform player)
    {
        EnsureSpawnPoint();

        if (daggerSpawnPoint == null)
        {
            Debug.LogError("Dagger spawn point isn't assigned");
            return;
        }

        
        if (daggerPrefab == null)
        {
            Debug.Log("Dagger prefab isn't assigned");
            return;
        }

        if (Time.time < lastFireTime + daggerCooldown)
        {
            return;
        }

        lastFireTime = Time.time;

        GameObject daggerInstance = Instantiate(daggerPrefab, daggerSpawnPoint.position, daggerSpawnPoint.rotation);

        DaggerProjectile daggerProjectile = daggerInstance.GetComponent<DaggerProjectile>();
        if (daggerProjectile != null)
        {
            daggerProjectile.SetTarget(player);
            Debug.Log("Dagger projectile target set to player");
        }

        Debug.Log("Dagger spawned at " + daggerSpawnPoint.position);
    }
}
