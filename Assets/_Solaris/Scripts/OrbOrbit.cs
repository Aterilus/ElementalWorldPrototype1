using UnityEngine;

public class OrbOrbit : MonoBehaviour
{
    public Transform centerPoint;
    public float orbitSpeed = 20f;
    public float hoverAmplitude = 0.2f;
    public float hoverSpeed = 2f;

    Vector3 startOffset;
    private void Start()
    {
        startOffset = transform.localPosition;
    }

    private void Update()
    {
        transform.RotateAround(centerPoint.position, Vector3.up, orbitSpeed * Time.deltaTime);

        float hover = Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
        transform.localPosition = startOffset + Vector3.up * hover;
    }
}
