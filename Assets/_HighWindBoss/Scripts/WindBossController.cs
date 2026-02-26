using System.Collections;
using UnityEngine;

public class WindBossController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Movement (V1 simple")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float stopDistance = 8f;
    [SerializeField] private bool facePlayer = true;

    [Header("Attack Ranges")]
    [SerializeField] private float cycloneRange = 15f;
    [SerializeField] private float blowbackRange = 6f;
    [SerializeField] float hurricaneRange = 20f;

    [Header("Cooldowns")]
    [SerializeField] private float cycloneCooldown = 5f;
    [SerializeField] private float blowbackCooldown = 8f;
    [SerializeField] private float hurricaneCooldown = 10f;

    [Header("Attack Prefabs")]
    [SerializeField] private GameObject cyclonePrefab;
    [SerializeField] private GameObject hurricanePrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform cycloneSpawn;
    [SerializeField] private Transform hurricaneSpawn;

    [Header("Cyclone Settings")]
    //[SerializeField] private int cycloneDamage = 10;
    //[SerializeField] float cycloneSpeed = 12f;

    [Header("Blowback Settings")]
    [SerializeField] private float blowbackForce = 15f;
    [SerializeField] float disorientDurration = 3f;
    [SerializeField] private float disorientMoveMultiplier = 1f;
    [SerializeField] private bool invertControls = true;

    [Header("Hurricane Strikes Settings")]
    [SerializeField] private int hurricaneDamage = 8;
    [SerializeField] float hurricaneSpeed = 10f;
    [SerializeField] int hurricaneCount = 5;
    //[SerializeField] float hurricaneSpreadDegrees = 12f;

    private float cycloneTimer;
    private float blowbackTimer;
    private float hurricaneTimer;

    private void Awake()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                player = p.transform;
            }
        }
    }

    private void Update()
    {
        if (player == null) { return; }

        cycloneTimer -= Time.deltaTime;
        blowbackTimer -= Time.deltaTime;
        hurricaneTimer -= Time.deltaTime;

        float dist = Vector3.Distance(transform.position, player.position);

        if (facePlayer)
        {
            Vector3 lookDir = (player.position - transform.position);
            lookDir.y = 0f;
            if (lookDir.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 8f);
            }
        }

        if (dist > stopDistance)
        {
            Vector3 dir = (player.position - transform.position);
            dir.y = 0f;
            dir.Normalize();
            transform.position += dir * moveSpeed * Time.deltaTime;
        }

        if (dist <= blowbackRange && blowbackTimer <= 0f)
        {
            DoBlowback();
            blowbackTimer = blowbackCooldown;
            return;
        }

        if (dist <= hurricaneRange && hurricaneTimer <= 0f)
        {
            DoHurricaneStrike();
            hurricaneTimer = hurricaneCooldown;
            return;
        }

        if (dist <= cycloneRange && cycloneTimer <= 0f)
        {
            DoCyclone();
            cycloneTimer = cycloneCooldown;
            return;
        }
    }

    private void DoCyclone()
    {
        if (cyclonePrefab == null || cycloneSpawn == null) { return; }

        Vector3 dir = (player.position - cycloneSpawn.position);
        dir.y = 0f;
        dir.Normalize();

        Instantiate(cyclonePrefab, cycloneSpawn.position, Quaternion.LookRotation(dir));
    }

    private void DoBlowback()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, blowbackRange);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Player")) { continue; }

            Rigidbody rb = hit.attachedRigidbody;
            Vector3 away = (hit.transform.position - transform.position);
            away.y = 0.25f;
            away.Normalize();

            if (rb != null)
            {
                rb.AddForce(away * blowbackForce, ForceMode.VelocityChange);
            }

            var disorient = hit.GetComponentInParent<PlayerDisorientReceiver>();
            if (disorient != null)
            {
                disorient.ApplyDisorient(disorientDurration, disorientMoveMultiplier, invertControls);
            }
        }
    }

    private void DoHurricaneStrike()
    {
        if (hurricanePrefab == null || hurricaneSpawn == null) { return; }

        StartCoroutine(HurricaneVollyRoutine());
    }

    private IEnumerator HurricaneVollyRoutine()
    {
        Vector3 baseDir = (player.position - hurricaneSpawn.position);
        baseDir.y = 0f;
        baseDir.Normalize();

        float spacing = 2.8f;
        float delay = 0.50f;

        for (int i = 0; i < hurricaneCount; ++i)
        {
            float offset = (i - hurricaneCount / 2f) * spacing;

            Vector3 side = Vector3.Cross(Vector3.up, baseDir);
            Vector3 spawnPos = hurricaneSpawn.position + side * offset;

            GameObject obj = Instantiate(hurricanePrefab, spawnPos, Quaternion.LookRotation(baseDir));

            var proj = obj.GetComponent<HurricaneStrikeProjectile>();
            if (proj != null)
            {
                proj.Init(baseDir, hurricaneSpeed, hurricaneDamage, player);
            }

            yield return new WaitForSeconds(delay);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, cycloneRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hurricaneRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, blowbackRange);
    }
#endif
}
