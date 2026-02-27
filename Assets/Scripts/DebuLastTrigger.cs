using UnityEngine;

public class DebuLastTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player triggered: " + other.name);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Player collided with: " + collision.collider.name);
    }
}
