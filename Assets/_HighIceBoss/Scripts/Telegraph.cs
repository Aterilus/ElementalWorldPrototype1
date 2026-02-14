using UnityEngine;

public class Telegraph : MonoBehaviour
{
    [Header("Timings")]
    public float lifetime = 1.0f;

    [Header("Visuals")]
    public Transform visuals;
    public bool pulse = true;
    public float pulseSpeed = 6f;
    public float pulseAmount = 0.11f;

    Vector3 baseScale;

    private void Awake()
    {
        if (!visuals) { visuals = transform; }
        baseScale = transform.localScale;
    }

    private void Update()
    {
        if (pulse)
        {
            float s = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            visuals.localScale = baseScale * s;
        }

        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
