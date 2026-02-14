using UnityEngine;

public class Fireball : MonoBehaviour
{
    private Vector3 dir;
    private float speed;
    private float damage;

    public void Init(Vector3 direction, float spd, float dmg)
    {
        dir = direction;
        speed = spd;
        damage = dmg;
    }

    private void Update()
    {
        transform.position += dir * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Implement damage logic here if needed
        Destroy(gameObject);
    }
}
