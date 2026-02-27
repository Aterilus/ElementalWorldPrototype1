using UnityEngine;

public class Fireball : MonoBehaviour
{
    private Vector3 dir;
    private float speed;
    private float damage;
    private float dieTime;
    private LayerMask playerMask;
    private bool charged;

    public void Init(Vector3 direction, float spd, float dmg, float lifetime, LayerMask mask, bool isCharged)
    {
        dir = direction.normalized;
        speed = spd;
        damage = dmg;
        dieTime = Time.time + lifetime;
        playerMask = mask;
        charged = isCharged;
    }

    private void Update()
    {
        transform.position += dir * speed * Time.deltaTime;
        if (Time.time >= dieTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerMask) == 0) { return; }

        var hp = other.GetComponentInParent<Health>();
        if (hp != null)
        {
            hp.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
