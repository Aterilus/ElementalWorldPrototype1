using UnityEngine;

public class DaggerProjectile: MonoBehaviour
{
    public float speed = 12f;
    public int damage = 5;
    public float lifetime = 5f;

    private Transform target;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
        Debug.Log("Dagger target set to " + (targetTransform ? targetTransform.name : "NULL"));
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
        }
        if(other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
