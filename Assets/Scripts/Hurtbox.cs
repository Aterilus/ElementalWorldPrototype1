using UnityEngine;

public class HurtboxScript : MonoBehaviour
{
    public Health healthComponent; // Reference to the Health component
    //public CharacterController characterController; // Reference to the CharacterController component

    private void Start()
    {
        healthComponent = transform.GetComponentInParent<Health>(); // Get the Health component from the same GameObject
        //characterController = transform.GetComponentInParent<CharacterController>(); // Get the CharacterController component from the same GameObject

    }
}
