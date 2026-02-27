using UnityEngine;

public class PillarRise : MonoBehaviour
{
    [SerializeField] private float riseDuration = 0.25f;
    [SerializeField] private float targetHeight = 6f;

    private Vector3 startScale;
    private Vector3 targetScale;
    private float t;

    private void Awake()
    {
        startScale = transform.localScale;
        targetScale = new Vector3(startScale.x, targetHeight, startScale.z);

        transform.localScale = new Vector3(startScale.x, 0.01f, startScale.z);
        t = 0f;
    }

    private void Update()
    {
        if (t >= 1f) { return; }

        t += Time.deltaTime / Mathf.Max(0.001f, riseDuration);
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, t);
    }
}
