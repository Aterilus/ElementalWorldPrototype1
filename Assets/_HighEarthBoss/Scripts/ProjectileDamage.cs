using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ProjectileDamage : MonoBehaviour
{
    [SerializeField] private int damage = 20;
    [SerializeField] private float destroyAfter = 6f;
    [SerializeField] private string targetTag = "Player";

    private bool hitSomething;

    private void Start()
    {
        Destroy(gameObject, destroyAfter);
    }

    private void OnCollisionEnter(Collision coll)
    {
        if (hitSomething) { return; }

        if (coll.collider.CompareTag(targetTag))
        {
            hitSomething = true;
            Health playerHP = gameObject.GetComponent<Health>();
            if (playerHP != null)
            {
                playerHP.TakeDamage(damage);
                return;
            }

            Destroy(gameObject);
        }
    }
}
