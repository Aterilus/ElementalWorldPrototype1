using UnityEngine;

public class FrostNova : MonoBehaviour
{
    public float duration = 0.35f;
    public float maxRadius = 6f;

    float t;

    private void Update()
    {
        t += Time.deltaTime;
        float a = Mathf.Clamp01(t / duration);
        float r = Mathf.Lerp(0.1f, maxRadius, a);

        Collider[] hits = Physics.OverlapSphere(transform.position, r);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                var playerHealth = hit.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(10);
                }
                hit.GetComponent<FrostbiteStatus>()?.AddStack();
            }
        }
        if (t >= duration)
        {
            Destroy(gameObject);
        }
    }
}
