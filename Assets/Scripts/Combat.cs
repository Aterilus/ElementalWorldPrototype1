using UnityEngine;
using UnityEngine.Rendering;

public class Combat : MonoBehaviour
{
    // Decide one input (keyboard/mouse/controller), on input enable the playerhitbox for a short duration, add an attack cooldown
    [SerializeField]
    public GameObject hitboxPrefab;
    [SerializeField]
    private Transform hitboxSpawnPoint;
    [SerializeField]
    private float attackCooldown = 1.0f;
    private float lastAttackTime = -Mathf.Infinity;
    private void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= lastAttackTime + attackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }
    } 
    private void PerformAttack()
    {
        // Instantiate the hitbox at the spawn point
        if (hitboxPrefab == null || hitboxSpawnPoint == null)
        {
            return;
        }

        Debug.Log("ATTACK! Spawning hitbox at " + hitboxSpawnPoint.position);

        var hb = Instantiate(hitboxPrefab, hitboxSpawnPoint.position, hitboxSpawnPoint.rotation);
        Destroy(hb, 0.15f);
        // Additional attack behavior can be implemented here
    }
    // Alternative implementation using AttackBehavior
}
