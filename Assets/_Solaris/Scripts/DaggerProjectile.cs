using UnityEngine;

public class DaggerProjectile: MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    public int damage = 5;
    [SerializeField] private float lifetime = 5f;

    private Transform target;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
        Debug.Log("Dagger target set to " + targetTransform.name);
        //Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (target == null)
        {
            return;
        }
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
        transform.forward = direction;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        var health = other.GetComponentInParent<Health>();
        if (health != null)
        {
            health.TakeDamage(damage);
            Debug.Log("Player hit by dagger for " + damage);
            Destroy(gameObject);
            return;
        }

        var bridge = other.GetComponentInParent<PlayerDamageBridge>();
        if (bridge != null)
        {
            bridge.TakeDamage(damage);
            Debug.Log("Player hit by dagger for " + damage + " via PlayerDamageBridge");
            Destroy(gameObject);
            return;
        }
        Debug.LogWarning("Player hit by dagger but no Health or PlayerDamageBridge found.");
    }
}
