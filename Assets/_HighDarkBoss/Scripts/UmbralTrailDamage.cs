using UnityEngine;
using System.Collections.Generic;

public class UmbralTrailDamage : MonoBehaviour
{
    public float damage = 12f;
    public float lifetime = 0.2f;
    public string targetTag = "Player";

    private HashSet<GameObject> hitTargets = new HashSet<GameObject>();

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetTag)) { return; }

        if (hitTargets.Contains(other.gameObject)) { return; }
        hitTargets.Add(other.gameObject);

        var hp = other.GetComponentInParent<Health>();
        if (hp != null)
        {
            hp.TakeDamage(damage);
        }
    }
}
