using UnityEngine;

public class ExpandAndDestroy : MonoBehaviour
{
    public float startScale = 0.5f;
    public float endScale = 6f;
    public float duration = 0.4f;

    private float t;

    private void OnEnable()
    {
        transform.localScale = Vector3.one * startScale;
    }

    private void Update()
    {
        t += Time.deltaTime;
        float k = Mathf.Clamp01(t / duration);
        transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, k);

        if (k >= 1f)
        {
            Destroy(gameObject);
        }
    }
}
