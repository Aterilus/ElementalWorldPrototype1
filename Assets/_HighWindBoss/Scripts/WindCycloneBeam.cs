using UnityEngine;

public class WindCycloneBeam : MonoBehaviour
{
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private float length = 15f;
    [SerializeField] private int damage = 15;

    private float timer;

    private void Start()
    {
        timer = duration;

        transform.localScale = new Vector3(1.5f, 1.5f, length);
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Health health = GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }
    }
}
