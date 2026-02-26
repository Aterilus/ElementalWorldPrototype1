using UnityEngine;
using System.Collections;

public class DarkCloneAI : MonoBehaviour
{
    [Header("Element Identity")]
    public string elemetnId;

    public DarkBossAI ownerBoss;

    [Header("Assigned Move")]
    public GameObject assignedMovePrefab;
    public Transform castPoint;

    [Header("Targeting")]
    public Transform player;

    [Header("Casting Settings")]
    public float castInterval = 4f;
    public float windup = 0.75f;



    private Health cloneHp;
    private bool casting;
    private bool deathReported;

    private void Start()
    {
        cloneHp = GetComponent<Health>();

        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p)
            {
                player = p.transform;
            }
        }

        StartCoroutine(Loop());
    }

    private void Update()
    {
        if (!deathReported && cloneHp != null && cloneHp.IsDead)
        {
            deathReported = true;

            if (ownerBoss != null && !string.IsNullOrEmpty(elemetnId))
            {
                ownerBoss.OnCloneDefeated(elemetnId.ToLower().Trim());
            }
        }
    }

    private IEnumerator Loop()
    {
        yield return new WaitForSeconds(Random.Range(0.2f, 1.2f));

        while (true)
        {
            if (cloneHp != null && cloneHp.IsDead) { yield break; }
            if (player == null || assignedMovePrefab == null || casting) { yield return null; continue; }

            casting = true;

            Vector3 look = player.position - transform.position;
            look.y = 0f;
            if (look.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(look);
            }

            yield return new WaitForSeconds(windup);

            if (cloneHp != null && cloneHp.IsDead) { yield break; }

            Vector3 pos = castPoint ? castPoint.position : transform.position;
            Quaternion rot = castPoint ? castPoint.rotation : transform.rotation;

            GameObject obj = Instantiate(assignedMovePrefab, pos, rot);

            var tr = obj.GetComponent<IAttackTargetReceiver>();
            if (tr != null)
            {
                tr.SetTarget(player);
            }

            yield return new WaitForSeconds(castInterval);
            casting = false;
        }
    }
}
