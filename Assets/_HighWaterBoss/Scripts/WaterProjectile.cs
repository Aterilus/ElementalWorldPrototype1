using UnityEngine;

public class WaterProjectile : MonoBehaviour 
{
    public float lifeTime = 4f;

    private float speed;
    private int damage;
    private Vector3 dir;

    public void Init(Vector3 targetPos, float projectileSpeed, int dmg)
    {
        speed = projectileSpeed;
        damage = dmg;
        dir = (targetPos - transform.position).normalized;
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.position += dir * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Health playerHealth =other.GetComponent<Health>();
            if(playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Destroy(gameObject);
            }
            else if (!other.isTrigger)
            {
                Destroy(gameObject);
            }
        }
    }
}
