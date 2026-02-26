using UnityEngine;
using System.Collections.Generic;

public class DamageZone : MonoBehaviour
{
    public float damage = 15f;
    public float life = 0.25f;
    public string targetName = "Player";

    public bool hitOncePerTarget = true;
    private HashSet<GameObject> hit = new HashSet<GameObject>();

    private void Start() => Destroy(gameObject, life);

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetName)) { return; }

        if (hitOncePerTarget && hit.Contains(other.gameObject)) {  return; }
        hit.Add(other.gameObject);

        var hp = other.GetComponentInParent<Health>();
        if (hp != null)
        {
            hp.TakeDamage(damage);
        }
    }
}
