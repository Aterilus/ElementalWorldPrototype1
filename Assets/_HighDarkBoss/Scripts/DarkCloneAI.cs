using UnityEngine;

public class DarkCloneAI : MonoBehaviour
{
    [Header("Element Identity")]
    public string elemetnId;
    public DarkBossAI ownerBoss;
    public DarkBossAI.AbilityId assignedAbility;
    public Transform player;

    [Header("Casting Settings")]
    public float castInterval = 4f;
    public float windup = 0.75f;



    private Health cloneHp;
    private float casting;
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

        casting = Time.time + Random.Range(0.5f, 1.5f);
    }

    private void Update()
    {
        if (cloneHp != null && cloneHp.IsDead)
        {
            

            if (!deathReported)
            {
                deathReported = true;
                ownerBoss?.OnCloneDefeated(elemetnId);
            }

            return;
        }

        if (player == null || ownerBoss == null) { return; }

        Vector3 look = player.position - transform.position;
        look.y = 0f;
        if (look.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(look);
        }

        if (Time.time < casting) { return; }

        casting = Time.time + castInterval;

        ownerBoss.ExecuteAbilityFromCaster(assignedAbility, transform, player, windup);
    }
}
