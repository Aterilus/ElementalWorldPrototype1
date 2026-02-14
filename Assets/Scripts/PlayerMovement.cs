using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;        //Movement speed
    public float gravity = -9.81f; //Gravity force
    public float jumpHeight = 2f;  //Jump height

    [Header("Attack")]
    public Transform attackPoint;
    public GameObject projectilePrefab;
    public float fireCooldown = 0.25f;

    [Header("Freese")]
    public bool isFrozen;
    float frozenTimer;

    private CharacterController controller;
    private Vector3 velocity;       //Vertical velocity for gravity and jumping
    float cd;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        cd -= Time.deltaTime;

        if (isFrozen)
        {
            frozenTimer -= Time.deltaTime;
            if (frozenTimer <= 0f)
            {
                isFrozen = false;
            }
            return;
        }

        // Movement input
        float x = Input.GetAxis("Horizontal");      // Left and right movement or A/D
        float z = Input.GetAxis("Vertical");        // Forward and backward movement or W/S

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // Jumping
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small negative value to keep the player grounded
        }
        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        if (Input.GetMouseButtonDown(0) && cd <= 0f)
        {
            Shoot();
            cd = fireCooldown;
        }
    }

    void Shoot()
    {
        if ( !attackPoint || !projectilePrefab)
        {
            return;
        }

        var go = Instantiate(projectilePrefab, attackPoint.position, attackPoint.rotation);
        var mover = go.GetComponent<ProjectileMover>();
        if (mover != null)
        {
            mover.Fire(attackPoint.forward);
        }
    }

    public void Freeze(float seconds)
    {
        isFrozen = true;
        frozenTimer = Mathf.Max(frozenTimer, seconds);
    }
}
