using UnityEngine;

public class Damage : MonoBehaviour
{
    public int damageAmount = 10; // Amount of damage to apply
    public Health playerHealth;

    private void Start()
    {
        
    }

    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Enemy")
        {
            playerHealth.TakeDamage(damageAmount);
        }
    }
}
