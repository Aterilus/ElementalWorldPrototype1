using UnityEngine;
using System.Collections.Generic;

public class DarkSummoning : MonoBehaviour
{
    public float spawnRadius = 6f;
    public bool overrideHealth = true;
    public float cloneMaxHealth = 120f;


    private void Start()
    {
        Debug.Log("DarkSummoning started. Finding boss and pool...");
        var bossCtrlr = FindFirstObjectByType<DarkBossAI>();
        var bossGO = GameObject.FindGameObjectWithTag("Enemy");

        if (bossCtrlr == null || bossGO == null)
        {
            Destroy(gameObject);
            return;
        }

        var pool = bossCtrlr.GetRandomSummonablePool();
        if (pool == null || pool.clonePrefab == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 pos = bossGO.transform.position + Random.insideUnitSphere * spawnRadius;
        pos.y = 0f;

        GameObject clone = Object.Instantiate(pool.clonePrefab, pos, bossGO.transform.rotation);
        Debug.Log("Spawned Clone: " + clone.name);

        var ai = clone.GetComponent<DarkCloneAI>();
        if (ai != null)
        {
            ai.elemetnId = pool.elementID.ToLower().Trim();
            ai.ownerBoss = bossCtrlr;

            var move = bossCtrlr.PickOneBorrowedMoveFrom(pool);
            ai.assignedMovePrefab = move != null ? move.movePrefab : null;

            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) { ai.player = p.transform; }
        }

        var hp = clone.GetComponent<Health>();
        if (hp != null)
        {
            hp.maxHealth = cloneMaxHealth;
            hp.currentHealth = cloneMaxHealth;
        }

        Destroy(gameObject);
    }
}
