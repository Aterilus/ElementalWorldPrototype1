using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class WaterAbilityPack : MonoBehaviour
{
    public Transform projectileSpawn;
    public GameObject hydroProjectilePrefab;

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
    public float globalAttackLock = 0.5f;

    public float hydroSnipeTelegraph = 0.35f;
    public float hydroSnipeProjectileSpeed = 28f;
    public int hydroSnipeProjectileDamage = 12;

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

    public Transform arenaCenter;
    Coroutine acidLoop;

    public void CastHydroSnipe(Transform caster, Transform target)
    {
        if (caster == null || target == null) { return; }
        StartCoroutine(HydroSnipeRoutine(caster, target));
    }

    public void CastWaterPrison(Transform target)
    {
        if (target == null) { return; }
        StartCoroutine(WaterPrisonRoutine(target));
    }

    public void StartAcidWaves(Transform target)
    {
        if (acidLoop != null) { StopCoroutine(acidLoop); }
        acidLoop = StartCoroutine(AcidWavesLoop(target));
    }

    public void StopAcidWaves()
    {
        if (acidLoop != null)
        {
            StopCoroutine(acidLoop);
        }
        acidLoop = null;
    }

    public IEnumerator HydroSnipeRoutine(Transform caster, Transform target)
    {
        yield return new WaitForSeconds(hydroSnipeTelegraph);

        Transform spawn = projectileSpawn != null ? projectileSpawn : caster;

        if (hydroProjectilePrefab != null && spawn != null)
        {
            GameObject proj = Instantiate(hydroProjectilePrefab, spawn.position, Quaternion.identity);
            WaterProjectile wp = proj.GetComponent<WaterProjectile>();
            if (wp != null)
            {
                wp.Init(target.position, hydroSnipeProjectileSpeed, hydroSnipeProjectileDamage);
            }
            else
            {
                Rigidbody rb = proj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 dir = (target.position - spawn.position).normalized;
                    rb.linearVelocity = dir * hydroSnipeProjectileSpeed;
                }
            }
        }

        yield return new WaitForSeconds(globalAttackLock);
    }

    public IEnumerator WaterPrisonRoutine(Transform target)
    {
        GameObject warning = null;
        if (prisonWarningPrefab != null)
        {
            Vector3 warnPos = new Vector3(target.position.x, target.position.y + 0.05f, target.position.z);
            warning = Instantiate(prisonWarningPrefab, warnPos, Quaternion.identity);
        }

        yield return new WaitForSeconds(waterPrisonTelegraph);

        PlayerPrisonTarget prisonTarget = target.GetComponent<PlayerPrisonTarget>();
        if (prisonTarget != null)
        {
            GameObject bubble = null;
            if (prisonBubblePrefab != null)
            {
                Vector3 bubblepos = target.position + new Vector3(0f, prisonBubbleYOffset, 0f);
                bubble = Instantiate(prisonBubblePrefab, bubblepos, Quaternion.identity);

                FollowTarget followTarget = bubble.GetComponent<FollowTarget>();
                if (followTarget != null)
                {
                    followTarget.target = target;
                    followTarget.offset = new Vector3(0f, prisonBubbleYOffset, 0f);
                }
            }

            var spawnedMinions = SpawnPrisonMinionsTracked(target);

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

            if (bubble != null)
            {
                Destroy(bubble);
            }
        }

        yield return new WaitForSeconds(globalAttackLock);
    }

    public IEnumerator AcidWavesLoop(Transform target)
    {
        yield return new WaitForSeconds(acidStartDelay);

        while (target != null)
        {
            Transform spawnT = acidWaveVFXSpawnPoint != null ? acidWaveVFXSpawnPoint : arenaCenter;
            if (spawnT != null && acidWaveVFXPrefab != null)
            {
                GameObject vfx = Instantiate(acidWaveVFXPrefab, spawnT.position, Quaternion.identity);
                Destroy(vfx, acidVFXLifetime);
            }

            SpawnAcidArenaVFX();

            Health playerHealth = target.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(acidTickDamage);
            }

            yield return new WaitForSeconds(acidTickInterval);
        }
    }

    public GameObject[] SpawnPrisonMinionsTracked(Transform target)
    {
        if (prisonMinionPrefabs == null || prisonMinionPrefabs.Length == 0) { return null; }
        if (target == null) { return null; }

        int count = Random.Range(minionsMin, minionsMax + 1);
        GameObject[] spawned = new GameObject[count];

        for (int i = 0; i < count; ++i)
        {
            Vector2 r = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 spawnPos = target.position + new Vector3(r.x, 0f, r.y);

            GameObject prefab = prisonMinionPrefabs[Random.Range(0, prisonMinionPrefabs.Length)];
            spawned[i] = Instantiate(prefab, spawnPos, Quaternion.identity);
        }

        return spawned;
    }

    public void SpawnAcidArenaVFX()
    {
        if (acidWaveVFXPrefab == null)
        {
            return;
        }

        Vector3 center = (arenaCenter != null) ? arenaCenter.position : transform.position;
        center.y = 0f;

        if (!acidSpawnAcrossArena)
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

        GameObject c = Instantiate(acidWaveVFXPrefab, center, Quaternion.identity);
        Destroy(c, acidVFXLifetime);
    }
}
