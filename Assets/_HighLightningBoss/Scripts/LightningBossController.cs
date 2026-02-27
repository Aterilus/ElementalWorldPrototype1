using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LightningBossController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Health playerHealth;
    public NavMeshAgent agent;

    [Header("Health / phase")]
    public float maxHP;
    public float currentHP;
    public bool phase2;

    [Header("Prefabs")]
    public GameObject warningCirclePrefab;
    public GameObject lightningStrikePrefab;
    public GameObject thunderCloudPrefab;
    public GameObject chainBoltPrefab;

    [Header("Thunder Strike")]
    public float strikeTelegrahphTime = 1.0f;
    public float strikeRadius = 2.5f;
    public float strikeDamage = 15f;

    [Header("Flash Step")]
    public float flashStepBehindDistance = 2.0f;
    public float flashStepCooldown = 6.0f;

    [Header("Cloud Chain Combo")]
    public int cloudsToSpawn = 3;
    public float cloudsSpawnArcRadius = 2.0f;
    public float cloudsHoverHeight = 2.5f;
    public float chainDelay = 0.8f;
    public float chainDuration = 1.2f;
    public float chainTickRate = 0.2f;
    public float chainDamagePerTick = 6f;
    public float chainRange = 8f;

    [Header("Storm Phase 2 Pressure")]
    public float stormRandomStrikeInterval = 2.0f;
    public float stormRandomStrikeRadius = 1.7f;
    public float stormRandomStrikeDamage = 10f;

    [Header("Attack Cadence")]
    public float baseAttackInterval = 2.5f;
    public float phase2AttackInterval = 1.7f;

    private float nextFlashStepTime;
    private bool isAttacking;

    private void Awake()
    {
        if(agent == null) { agent = GetComponent<NavMeshAgent>(); }
        currentHP = maxHP;
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject plyr = GameObject.FindGameObjectWithTag("Player");
            if (plyr != null )
            {
                player = plyr.transform;
            }
        }

        StartCoroutine(AttackLoop());
    }

    private void Update()
    {
        if (!phase2 && currentHP <= maxHP * 0.5f)
        {
            phase2 = true;
            OnPhase2Start();
        }

        if (player != null && agent != null && !isAttacking)
        {
            agent.SetDestination(player.position);
        }
    }

    IEnumerator AttackLoop()
    {
        while (true)
        {
            if (player == null)
            {
                yield return null;
                continue;
            }

            float interval = phase2 ?phase2AttackInterval : baseAttackInterval;

            if (Time.time >= nextFlashStepTime)
            {
                yield return FlashStepCloudChainCombo();
                nextFlashStepTime = Time.time + (phase2 ? flashStepCooldown * 0.75f : flashStepCooldown);
            }
            else
            {
                yield return ThunderStrikeAtPlayer();
            }

            yield return new WaitForSeconds(interval);
        }
    }

    IEnumerator ThunderStrikeAtPlayer()
    {
        isAttacking = true;
        if (agent != null) { agent.isStopped = true; }

        Vector3 targetPos = player.position;

        GameObject warn = null;
        if (warningCirclePrefab != null)
        {
            warn = Instantiate(warningCirclePrefab, targetPos, Quaternion.identity);
            warn.transform.localScale = new Vector3(strikeRadius * 2f, 0.1f, strikeRadius * 2f);
        }

        yield return new WaitForSeconds(strikeTelegrahphTime);

        if (lightningStrikePrefab != null)
        {
            Instantiate(lightningStrikePrefab, targetPos, Quaternion.identity);
        }

        float dist = Vector3.Distance(player.position, targetPos);
        if (dist <= strikeRadius)
        {
            playerHealth.TakeDamage(strikeDamage);
        }

        if (warn != null) { Destroy(warn); }
        if (agent != null) { agent.isStopped = true; }
        isAttacking = false;
    }

    IEnumerator FlashStepCloudChainCombo()
    {
        isAttacking = true;
        if (agent != null) { agent.isStopped = true; }


        FlashStepBehindPlayer();

        List<LightningCloud> clouds = SpawnCloudsBehindPlayer();

        yield return new WaitForSeconds(chainDelay);

        float elapsed = 0f;
        float tick = 0f;

        while (elapsed < chainDuration)
        {
            elapsed += Time.deltaTime;
            tick += Time.deltaTime;

            if (tick >= chainTickRate)
            {
                tick = 0f;

                bool hitThisTick = false;

                foreach (var cloud in clouds)
                {
                    if (cloud == null) continue;
                    float d = Vector3.Distance(cloud.transform.position, player.position);
                    if (d <= chainRange)
                    {
                        hitThisTick = true;

                        Vector3 cloudPos = cloud.transform.position;
                        Vector3 playerPos = player.position + Vector3.up * 1.0f;
                        SpawnChainBolt(cloudPos, playerPos);
                    }
                }

                if (hitThisTick)
                {
                    playerHealth.TakeDamage(chainDamagePerTick);
                }
            }

            yield return null;
        }

        if (agent != null) { agent.isStopped = false; }
        isAttacking = false;
    }

    void FlashStepBehindPlayer()
    {
        if (player == null)
        {
            return;
        }

        Vector3 behind = player.position - player.forward * flashStepBehindDistance;
        behind.y = transform.position.y;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.Warp(behind);
        }
        else
        {
            transform.position = behind;
        }

        Vector3 look = player.position - transform.position;
        look.y = 0f;
        if (look.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(look);
        }
    }

    List<LightningCloud> SpawnCloudsBehindPlayer()
    {
        var list = new List<LightningCloud>();
        if (thunderCloudPrefab == null || player == null) { return list; }

        Vector3 basedPos = player.position - player.forward * 1.5f;
        basedPos.y += cloudsHoverHeight;

        for (int i = 0; i < cloudsToSpawn; ++i)
        {
            float t = (cloudsToSpawn <= 1) ? 0.05f : (float)i / (cloudsToSpawn - 1);
            float angle = Mathf.Lerp(-40f, 40f, t);
            Vector3 offset = Quaternion.Euler(0f, angle, 0f) * (-player.forward * cloudsSpawnArcRadius);

            Vector3 spawnPos = basedPos+ offset;
            GameObject go = Instantiate(thunderCloudPrefab, spawnPos, Quaternion.identity);

            var cloud = go.GetComponent<LightningCloud>();
            if (cloud == null) { cloud = go.AddComponent<LightningCloud>(); }
            cloud.Init(this);

            list.Add(cloud);
        }

        return list;
    }

    void OnPhase2Start()
    {
        StartCoroutine(StormRandomStrikes());
    }

    IEnumerator StormRandomStrikes()
    {
        while(phase2)
        {
            if (player == null)
            {
                yield return null;
                continue;
            }

            Vector2 r = Random.insideUnitCircle * 4f;
            Vector3 pos = player.position + new Vector3(r.x, 0f, r.y);

            GameObject warn = null;
            if (warningCirclePrefab != null)
            {
                warn = Instantiate(warningCirclePrefab, pos, Quaternion.identity);
                warn.transform.localScale = new Vector3(stormRandomStrikeRadius * 2f, 0.1f, stormRandomStrikeRadius * 2f);
            }

            yield return new WaitForSeconds(0.7f);

            if (lightningStrikePrefab != null)
            {
                Instantiate(lightningStrikePrefab, pos, Quaternion.identity);
            }

            float dist = Vector3.Distance(player.position, pos);
            if (dist <= stormRandomStrikeRadius)
            {
                playerHealth.TakeDamage(stormRandomStrikeDamage);
            }

            if (warn != null)
            {
                Destroy(warn);
            }

            yield return new WaitForSeconds(stormRandomStrikeInterval);
        }
    }

    void SpawnChainBolt(Vector3 from, Vector3 to, float lifetime = 0.15f)
    {
        if (chainBoltPrefab == null) { return; }

        GameObject go = Instantiate(chainBoltPrefab, Vector3.zero, Quaternion.identity);
        var lr = go.GetComponent<LineRenderer>();
        if (lr != null)
        {
            lr.positionCount = 2;
            lr.SetPosition(0, from);
            lr.SetPosition(1, to);
        }

        Destroy(go, lifetime);
    }
}
