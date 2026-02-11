using UnityEngine;

public class HurricaneStrikeProjectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private float steerStrength = 3f;

    private Vector3 dir;
    private float speed;
    private int damage;
    private Transform player;

    public void Init(Vector3 direction, float movespeed, int dmg, Transform target)
    {
        dir = direction.normalized;
        speed = movespeed;
        damage = dmg;
        player = target;

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (player != null)
        {
            Vector3 toPlayer = (player.position - transform.position);
            toPlayer.y = 0f;

            if (toPlayer.sqrMagnitude > 0.1f)
            {
                Vector3 desired = toPlayer.normalized;
                dir = Vector3.Slerp(dir, desired, Time.deltaTime * steerStrength).normalized;
            }
        }

        transform.position += dir * speed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(dir);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Health health = GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
