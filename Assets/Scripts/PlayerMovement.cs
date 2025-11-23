using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;        //Movement speed
    public float gravity = -9.81f; //Gravity force
    public float jumpHeight = 2f;  //Jump height

    private CharacterController controller;
    private Vector3 velocity;       //Vertical velocity for gravity and jumping

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
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
    }
}
