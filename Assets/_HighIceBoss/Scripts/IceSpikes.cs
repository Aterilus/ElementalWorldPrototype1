using UnityEngine;

public class IceSpikes : MonoBehaviour
{
    public float growTime = 0.15f;
    public float stayTime = 0.4f;
    public float damage = 15f;
    public float hitRadius = 1.2f;

    float t;
    Vector3 targetScale;

    private void Start()
    {
        targetScale = transform.localScale;
        transform.localScale = new Vector3(targetScale.x, 0.01f, targetScale.z);
    }

    private void Update()
    {
        t += Time.deltaTime;

        if (t <= growTime)
        {
            float a = t / growTime;
            transform.localScale = Vector3.Lerp(new Vector3(targetScale.x, 0.01f, targetScale.z), targetScale, a);
        }

        if (t > growTime && t < growTime + 0.05f)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, hitRadius);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    var playerHealth = hit.GetComponent<Health>();
                    if (playerHealth != null)
                    {
                        playerHealth.TakeDamage(damage);
                    }
                    hit.GetComponent<FrostbiteStatus>()?.AddStack();
                }
            }
        }

        if (t >= growTime + stayTime )
        {
            Destroy(gameObject);
        }
    }
}
