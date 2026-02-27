using UnityEngine;

public class SimpleFadeDestroy : MonoBehaviour
{
    public float lifetime = 0.25f;
    public float startScale = 1f;
    public float endScale = 1.5f;

    private float t;
    private Material m;

    private void Start()
    {
        transform.localScale *= startScale;

        var renderer = GetComponent<Renderer>();
        if (renderer)
        {
            m = renderer.material;
        }
    }

    private void Update()
    {
        t += Time.deltaTime;
        float k = Mathf.Clamp01(t / lifetime);

        transform.localScale = Vector3.Lerp(transform.localScale, transform.localScale * endScale, k);

        if (m)
        {
            Color c = m.color;
            c.a = 1f - k;
            m.color = c;
        }

        if (k > 1f)
        {
            Destroy(gameObject);
        }
    }
}
