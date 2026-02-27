using UnityEngine;

public class HitboxScript : MonoBehaviour
{
    public float damageAmount = 10f; // Amount of damage this hitbox deals
    public Vector3 v3Knockback = new Vector3(0, 10, 10); // Knockback force applied on hit

    public LayerMask hurtboxLayer; // Layer mask to identify hurtboxes

    private void OnTriggerEnter(Collider other)
    {
        if ( hurtboxLayer == (hurtboxLayer | (1 << other.transform.gameObject.layer))) // Check if the collided object is in the hurtbox layer
        {
            HurtboxScript hurtbox = other.GetComponent<HurtboxScript>(); // Try to get the Hurtbox component from the collided object
            var react = other.GetComponentInParent<SolarisHitReacts>();
            if (react != null)
            {
                react.OnHit();
            }

            if (hurtbox != null) 
            {
                hurtbox.healthComponent.TakeDamage((int)damageAmount); // Apply damage to the hurtbox's health component
                //hurtbox.characterController.Move(v3Knockback * Time.deltaTime); // Apply knockback to the hurtbox's character controller
                Destroy(this.gameObject); // Destroy the hitbox after applying damage and knockback

            }
        }
    }
}
