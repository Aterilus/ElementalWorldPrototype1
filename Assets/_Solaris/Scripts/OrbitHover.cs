using UnityEngine;

public class OrbitHover : MonoBehaviour
{
    public float amplitude = 0.15f;
    public float speed = 2f;

    Vector3 startLocal;

    void Start()
    {
        startLocal = transform.localPosition;
    }

    void Update()
    {
        float y = Mathf.Sin(Time.time * speed) * amplitude;
        transform.localPosition = startLocal + Vector3.up * y;
    }
}
