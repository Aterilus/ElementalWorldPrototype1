using UnityEngine;

public class SolarisLookAt : MonoBehaviour
{
    public Transform player;
    public float rotationSpeed = 5.0f;

    void Update()
    {
        if (player == null)
        {
            return;
        }
        Vector3 direction = player.position - transform.position;
        direction.y = 0f; // Keep only the horizontal direction
        if (direction.sqrMagnitude < 0.001f)
        {
            return; // Avoid zero-length direction
        }

        
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
