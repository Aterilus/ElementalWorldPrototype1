using UnityEngine;

public class CalamityWorldBounds : MonoBehaviour
{
    public static CalamityWorldBounds Instance { get; private set; }

    [SerializeField] private BoxCollider bounds;

    private void Awake()
    {
        Instance = this;
        if (!bounds) { bounds = GetComponent<BoxCollider>(); }
    }

    public Vector3 GetRandomPointHigh()
    {
        Bounds b = bounds.bounds;
        float x = Random.Range(b.min.x, b.max.x);
        float z = Random.Range(b.min.z, b.max.z);
        float y = b.max.y + 50f;
        return new Vector3(x, y, z);
    }

    public bool IsInside(Vector3 point) => bounds.bounds.Contains(point);
}
