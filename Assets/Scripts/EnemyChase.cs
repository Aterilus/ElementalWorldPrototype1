using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    [Header("Target")]
   public GameObject player;

    [Header("Movement")]
    public float speed = 3f;
    public float stopDistance = 1.5f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (player == null) { return; }

        FacePlayer();

        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0f; // Keep movement on the horizontal plane

        if (direction.magnitude <= stopDistance)
        {
            return; // Stop moving if within stop distance
        }

        Vector3 move = direction.normalized * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);

        if (direction != Vector3.zero)
        {
            rb.rotation = Quaternion.LookRotation(direction);
        }
    }

    void FacePlayer()
    {
        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0f; // Keep rotation on the horizontal plane

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
