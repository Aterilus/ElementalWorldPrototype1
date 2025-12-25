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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PerformAttack();
            lastAttackTime = Time.time;
            var health = GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(10);
            }
        }
    }
    // Preform the attack logic
    private void PerformAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown)
        {
            return; // Still in cooldown
        }
        if (hitboxPrefab == null || hitboxSpawnPoint == null)
        {
            return;
        }
        Debug.Log("ATTACK! Spawning hitbox at " + hitboxSpawnPoint.position);
        var hb = Instantiate(hitboxPrefab, hitboxSpawnPoint.position, hitboxSpawnPoint.rotation);
        Destroy(hb, 0.15f);
    }
    //private void PerformAttack()
    //{
    //    Instantiate the hitbox at the spawn point
    //    if (hitboxPrefab == null || hitboxSpawnPoint == null)
    //    {
    //        return;
    //    }

    //    Debug.Log("ATTACK! Spawning hitbox at " + hitboxSpawnPoint.position);

    //    var hb = Instantiate(hitboxPrefab, hitboxSpawnPoint.position, hitboxSpawnPoint.rotation);
    //    Destroy(hb, 0.15f);
    //    Additional attack behavior can be implemented here
    //}
    // Alternative implementation using AttackBehavior
}
