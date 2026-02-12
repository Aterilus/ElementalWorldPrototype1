using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class WaterBossController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform arenaCenter;
    public Transform projectileSpawn;
    public GameObject hydroProjectilePrefab;

    [Header("Movement")]
    public float moveSpeed = 4f;
    public float strafeSpeed = 2.5f;
    public float keepDistance = 10f;
    public float engageRange = 25f;
    public float arenaRadius = 18f;

    [Header("Attack Timing")]
    public float globalAttackLock = 0.5f;

    [Header("Water Prison")]
    public bool waterPrisonEnabled = true;
    public bool waterPrisonAvoidable = false;
    public float waterPrisonCoolDown = 14f;
    public float waterPrisonTelegraph = 0.7f;
    public float waterPrisonDuration = 2.5f;
    public int waterPrisonDamageChunk = 20;

    [Header("Water Prison VFX")]
    public GameObject prisonWarningPrefab;
    public GameObject prisonBubblePrefab;
    public float prisonBubbleYOffset = 1f;

    [Header("Water Prison Minions")]
    public GameObject[] prisonMinionPrefabs;
    public int minionsMin = 2;
    public int minionsMax = 3;
    public float spawnRadius = 4f;
    public float minionLifeTime = 13f;

    [Header("Hydro Snipe")]
    public bool hydroSnipeEnabled = true;
    public float hydroSnipeCoolDown = 4.5f;
    public float hydroSnipeTelegraph = 0.35f;
    public float hydroSnipeProjectileSpeed = 28f;
    public int hydroSnipeProjectileDamage = 12;

    [Header("Acid Waves")]
    public bool acidWaveEnabled = true;
    public float acidTickInterval = 1.2f;
    public int acidTickDamage = 2;
    public float acidStartDelay = 1.5f;

    [Header("Acid Waves VFX")]
    public GameObject acidWaveVFXPrefab;
    public Transform acidWaveVFXSpawnPoint;
    public float acidVFXLifetime = 1.5f;
    public bool acidSpawnAcrossArena = true;
    public int acidVFXCount = 16;
    public float acidArenaVFXRadius = 12f;

    private float nextPrisonTime;
    private float nextSnipeTime;

    private bool isDead;
    private bool isBusy;
    private int strafeDir = 1;
    private bool acidSpawnAcrossArenaVFX;

    private void Start()
    {
        if (player == null)
        {
            GameObject plyr = GameObject.FindGameObjectWithTag("Player");
            if (plyr != null)
            {
                player = player.transform;
            }
        }

        nextPrisonTime = Time.time + 2f;
        nextSnipeTime = Time.time + 1f;

        if (acidWaveEnabled)
        {
            StartCoroutine(AcidWavesLoop());
        }
    }

    private void Update()
    {
        if (isDead || player == null || arenaCenter == null) { return; }

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > engageRange)
        {
            MoveToward(arenaCenter.position, moveSpeed);
            Face(player.position);
            return;
        }

        CombatMove(dist);

        if (!isBusy)
        {
            if (waterPrisonEnabled && Time.time >= nextPrisonTime)
            {
                StartCoroutine(WaterPrisonAttack());
            }
            else if (hydroSnipeEnabled && Time.time >= nextSnipeTime)
            {
                StartCoroutine(HydroSnipeAttack());
            }
        }
    }

    private void CombatMove(float distToPlayer)
    {
        Vector3 centerPos = arenaCenter.position;

        Vector3 toCenter = (centerPos - transform.position);
        float distFromCenter = toCenter.magnitude;

        if (distFromCenter > arenaRadius)
        {
            MoveToward(centerPos, moveSpeed);
            Face(player.position);
            return;
        }

        Vector3 toPlayer = (player.position - transform.position);
        Vector3 desired;

        if (distToPlayer < keepDistance * 0.5f)
        {
            desired = transform.position - toPlayer.normalized * moveSpeed * Time.deltaTime;
        }
        else if (distToPlayer > keepDistance * 1.25f)
        {
            desired = transform.position + toPlayer.normalized * moveSpeed * Time.deltaTime;
        }
        else
        {
            Vector3 strafe = Vector3.Cross(Vector3.up, toPlayer.normalized) * (strafeDir * strafeSpeed * Time.deltaTime);
            desired = transform.position + strafe;
        }

        transform.position = desired;
        Face(player.position);

        if (Random.value < 0.005f)
        {
            strafeDir *= -1;
        }
    }

    private void MoveToward(Vector3 target, float speed)
    {
        Vector3 dir = (target - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.01f)
        {
            return;
        }
        transform.position += dir.normalized * speed * Time.deltaTime;
    }

    private void Face(Vector3 target)
    {
        Vector3 dir = (target - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) { return; }

        Quaternion rot = Quaternion.LookRotation(dir.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 8f);
    }

    [System.Obsolete]
    private IEnumerator HydroSnipeAttack()
    {
        isBusy = true;
        nextSnipeTime = Time.time + hydroSnipeCoolDown;

        yield return new WaitForSeconds(hydroSnipeTelegraph);

        if (hydroProjectilePrefab != null && projectileSpawn != null)
        {
            GameObject proj = Instantiate(hydroProjectilePrefab, projectileSpawn.position, Quaternion.identity);    
            WaterProjectile wp = proj.GetComponent<WaterProjectile>();
            if (wp != null)
            {
                wp.Init(player.position, hydroSnipeProjectileSpeed, hydroSnipeProjectileDamage);
            }
            else
            {
                Rigidbody rb = proj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 dir = (player.position - projectileSpawn.position).normalized;
                    rb.velocity = dir * hydroSnipeProjectileSpeed;
                }
            }
        }

        yield return new WaitForSeconds(globalAttackLock);
        isBusy = false;
    }

    [System.Obsolete]
    private IEnumerator WaterPrisonAttack()
    {
        isBusy = true;
        nextPrisonTime = Time.time + waterPrisonCoolDown;

        if (player== null)
        {
            isBusy= false;
            yield break;
        }

        GameObject warning = null;
        if (prisonWarningPrefab != null)
        {
            Vector3 warnPos = new Vector3(player.position.x, player.position.y + 0.05f, player.position.z);
            warning = Instantiate(prisonWarningPrefab, warnPos, Quaternion.identity);
        }

        yield return new WaitForSeconds(waterPrisonTelegraph);

        if (isDead) { yield break; }

        PlayerPrisonTarget prisonTarget = player.GetComponent<PlayerPrisonTarget>();
        if (prisonTarget != null)
        {
            GameObject bubble = null;
            if (prisonBubblePrefab != null)
            {
                Vector3 bubblepos = player.position + new Vector3(0f, prisonBubbleYOffset, 0f);
                bubble = Instantiate(prisonBubblePrefab, bubblepos, Quaternion.identity);

                FollowTarget followTarget = bubble.GetComponent<FollowTarget>();
                if (followTarget != null)
                {
                    followTarget.target = player;
                    followTarget.offset = new Vector3(0f, prisonBubbleYOffset, 0f);
                }
            }

            var spawnedMinions = SpawnPrisonMinionsTracked();

            prisonTarget.ApplyPrison(waterPrisonDuration, waterPrisonDamageChunk);

            yield return new WaitForSeconds(waterPrisonDuration);

            if (spawnedMinions != null)
            {
                foreach (var minion in spawnedMinions)
                {
                    if (minion != null)
                    {
                        Destroy(minion);
                    }
                }
            }
        }

        yield return new WaitForSeconds(globalAttackLock);
        isBusy = false;
    }

    private IEnumerator AcidWavesLoop()
    {
        yield return new WaitForSeconds(acidStartDelay);

        while (!isDead)
        {
            if (player != null)
            {
                Transform spawnT = acidWaveVFXSpawnPoint != null ? acidWaveVFXSpawnPoint : arenaCenter;
                if (spawnT != null && spawnT != null)
                {
                    GameObject vfx = Instantiate(acidWaveVFXPrefab, spawnT.position, Quaternion.identity);
                    Destroy(vfx, acidVFXLifetime);
                }

                SpawnAcidArenaVFX();

                Health playerHealth = player.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(acidTickDamage);
                }
            }

            yield return new WaitForSeconds(acidTickInterval);
        }
    }

    private GameObject[] SpawnPrisonMinionsTracked()
    {
        if (prisonMinionPrefabs == null || prisonMinionPrefabs.Length == 0) { return null; }
        if (player == null) { return null; }

        int count = Random.Range(minionsMin, minionsMax + 1);
        GameObject[] spawned = new GameObject[count];

        for (int i = 0; i < count; ++i)
        {
            Vector2 r = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 spawnPos = player.position + new Vector3(r.x, 0f, r.y);

            GameObject prefab = prisonMinionPrefabs[Random.Range(0, prisonMinionPrefabs.Length)];
            spawned[i] = Instantiate(prefab, spawnPos, Quaternion.identity);
        }

        return spawned;
    }

    private void SpawnAcidArenaVFX()
    {
        if ( acidWaveVFXPrefab == null)
        {
            return;
        }

        Vector3 center = (arenaCenter != null) ? arenaCenter.position : transform.position;

        if (!acidSpawnAcrossArenaVFX)
        {
            GameObject one = Instantiate(acidWaveVFXPrefab, center, Quaternion.identity);
            Destroy(one, acidVFXLifetime);
            return;
        }

        for (int i = 0; i < acidVFXCount; ++i)
        {
            float t = (float)i / acidVFXCount;
            float angle = t * Mathf.PI * 2f;

            Vector3 pos = center + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * acidArenaVFXRadius;
            GameObject vfx = Instantiate(acidWaveVFXPrefab, pos, Quaternion.identity);
            Destroy(vfx, acidVFXLifetime);
        }

        GameObject c = Instantiate(acidWaveVFXPrefab, center, Quaternion .identity);
        Destroy(c, acidVFXLifetime);
    }
}
