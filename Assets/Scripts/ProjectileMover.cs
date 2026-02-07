using UnityEngine;

public class ProjectileMover : MonoBehaviour
{
    public float speed = 18f;
    public float lifeTime = 5f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Fire(Vector3 direction)
    {
        direction.y = 0f;
        direction.Normalize();
        rb.linearVelocity = direction * speed;
    }
}
