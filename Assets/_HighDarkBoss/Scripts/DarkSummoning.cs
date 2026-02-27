using UnityEngine;

public class DarkSummoning : MonoBehaviour
{
    public float spawnRadius = 6f;
    public bool overrideHealth = true;
    public float cloneMaxHealth = 120f;

    [Header("Ground Snap")]
    public bool snapToGround = true;
    public float raycastHeight = 50f;
    public LayerMask groundMask = ~0;
    public float groundOffset = 0.05f;

    private void Start()
    {
        Debug.Log("DarkSummoning started. Finding boss and pool...");
        var bossCtrlr = FindFirstObjectByType<DarkBossAI>();
        var bossGO = GameObject.FindGameObjectWithTag("Enemy");

        if (bossCtrlr == null || bossGO == null)
        {
            Debug.Log("DarkSummoning: Boss or Boss GameObject not found. Destroying self.");
            Destroy(gameObject);
            return;
        }

        var pool = bossCtrlr.GetRandomSummonablePool();
        if (pool == null || pool.clonePrefab == null)
        {
            Debug.Log("DarkSummoning: No valid summon pool or clone prefab found. Destroying self.");
            Destroy(gameObject);
            return;
        }

        float minFromBoss = 4f;
        float minFromPlayer = 3f;
        float checkRadius = 0.6f;
        LayerMask blockMask = ~0;

        Transform playerTf = null;
        var pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj) { playerTf = pObj.transform; }

        Vector3 center = bossGO.transform.position;
        Vector3 pos = center;

        bool found = false;
        for (int i = 0; i < 25; ++i)
        {
            Vector2 circle = Random.insideUnitSphere.normalized;
            float dist = Random.Range(minFromBoss, spawnRadius);

            Vector3 candidate = center + new Vector3(circle.x, 0f, circle.y) * dist;

            if (playerTf != null && Vector3.Distance(candidate, playerTf.position) < minFromPlayer)
            {
                continue;
            }

            if (Physics.CheckSphere(candidate + Vector3.up * 0.5f, checkRadius, blockMask, QueryTriggerInteraction.Ignore))
            {
                continue;
            }

            pos = candidate;
            found = true;
            break;
        }

        if (!found)
        {
            Vector3 fallbackDir = (playerTf ? (center - playerTf.position) : Vector3.forward);
            fallbackDir.y = 0f;
            if (fallbackDir.sqrMagnitude < 0.001f) { fallbackDir = Vector3.forward; }
            fallbackDir.Normalize();
            pos = center + fallbackDir * minFromBoss;
        }

        if (snapToGround) { pos = SnapToGround(pos); }

        GameObject clone = Object.Instantiate(pool.clonePrefab, pos, bossGO.transform.rotation);
        Debug.Log("Spawned Clone: " + clone.name);

        var ai = clone.GetComponent<DarkCloneAI>();
        if (ai != null)
        {
            ai.elemetnId = pool.elementID;
            ai.ownerBoss = bossCtrlr;

           
            ai.assignedAbility = bossCtrlr.PickOneBorrowedAbilityIdFrom(pool);

            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) { ai.player = p.transform; }

            bossCtrlr.OnCloneSpawned(ai.elemetnId);
        }

        var notifier = clone.GetComponent<CloneDeathNotifier>();
        if (notifier != null)
        {
            notifier.boss = bossCtrlr;
            notifier.elementId = pool.elementID;
        }

        var hp = clone.GetComponent<Health>();
        if (hp != null)
        {
            hp.maxHealth = cloneMaxHealth;
            hp.currentHealth = cloneMaxHealth;
        }

        Debug.Log($"Clone {clone.name} spawned with element {ai.elemetnId} and ability {ai.assignedAbility}");

        Destroy(gameObject);
    }

    Vector3 SnapToGround(Vector3 pos)
    {
        Ray ray = new Ray(pos + Vector3.up * raycastHeight, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastHeight * 2f, groundMask, QueryTriggerInteraction.Ignore))
        {
            pos.y = hit.normal.y + groundOffset;
        }
        return pos;
    }
}
