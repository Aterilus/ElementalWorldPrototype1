using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    public float speed = 18f;
    public float lietime = 3f;

    public int lethalDamage = 999;
    public bool allowLethal;

    public LayerMask playerMask;

    private void Start() => Destroy(gameObject, lietime);

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerMask) == 0) { return; }

        Health hp = other.GetComponent<Health>();
        if (hp != null)
        {
            DamagerHelper.ApplyDamage(hp, lethalDamage, allowLethal, 1);
        }

        Destroy(gameObject);
    }
}
