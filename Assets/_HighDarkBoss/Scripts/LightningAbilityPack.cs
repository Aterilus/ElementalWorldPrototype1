using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class LightningAbilityPack : MonoBehaviour
{
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

    Coroutine stormRoutine;

    public void CastLightningStrike(Transform target)
    {
        if (target == null) { return; }
        Health targetHp = target.GetComponentInParent<Health>();
        StartCoroutine(LightningStrikeRoutine(target, targetHp));
    }

    public void CastCloudChainCombo(Transform caster, Transform target, bool warpBehind = true)
    {

        if (caster == null || target == null) { return; }
        Health targetHp = target.GetComponentInParent<Health>();
        NavMeshAgent casterAgent = caster.GetComponentInParent<NavMeshAgent>();
        StartCoroutine(CloudChainComboRoutine(caster, casterAgent, target, targetHp, warpBehind));
    }

    public void StartStorm(Transform target, Health targetHp)
    {
        StopStorm();
        stormRoutine = StartCoroutine(StormRandomStrikesRoutine(target, targetHp));
    }

    public void StopStorm()
    {
        if (stormRoutine != null)
        {
            StopCoroutine(stormRoutine);
            stormRoutine = null;
        }
    }

    IEnumerator LightningStrikeRoutine( Transform target, Health targetHp)
    {
        Vector3 targetPos = target.position;
        targetPos.y = 0f;

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

        float dist = Vector3.Distance(new Vector3(target.position.x, 0f, target.position.z), targetPos);
        if (dist <= strikeRadius && targetHp != null)
        {
            targetHp.TakeDamage(strikeDamage);
        }

        if (warn != null) { Destroy(warn); }
    }

    IEnumerator CloudChainComboRoutine(Transform caster, NavMeshAgent casterAgent, Transform target, Health targetHp, bool warpBehind = true)
    {
        if (warpBehind)
        {
            FlashStepBehindPlayer(caster, casterAgent, target);
        }
        
        List<LightningCloud> clouds = SpawnCloudsBehindPlayer(target);

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
                    float d = Vector3.Distance(cloud.transform.position, target.position);
                    if (d <= chainRange)
                    {
                        hitThisTick = true;

                        Vector3 cloudPos = cloud.transform.position;
                        Vector3 playerPos = target.position + Vector3.up * 1.0f;
                        SpawnChainBolt(cloudPos, playerPos);
                    }
                }

                if (hitThisTick)
                {
                    targetHp.TakeDamage(chainDamagePerTick);
                }
            }

            yield return null;
        }
    }

    void FlashStepBehindPlayer(Transform caster, NavMeshAgent agent, Transform target)
    {
        Vector3 behind = target.position - target.forward * flashStepBehindDistance;
        behind.y = caster.position.y;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.Warp(behind);
        }
        else
        {
            caster.position = behind;
        }

        Vector3 look = target.position - caster.position;
        look.y = 0f;
        if (look.sqrMagnitude > 0.001f)
        {
            caster.rotation = Quaternion.LookRotation(look);
        }
    }

    List<LightningCloud> SpawnCloudsBehindPlayer(Transform target)
    {
        var list = new List<LightningCloud>();
        if (thunderCloudPrefab == null || target == null) { return list; }

        Vector3 basedPos = target.position - target.forward * 1.5f;
        basedPos.y += cloudsHoverHeight;

        for (int i = 0; i < cloudsToSpawn; ++i)
        {
            float t = (cloudsToSpawn <= 1) ? 0.05f : (float)i / (cloudsToSpawn - 1);
            float angle = Mathf.Lerp(-40f, 40f, t);
            Vector3 offset = Quaternion.Euler(0f, angle, 0f) * (-target.forward * cloudsSpawnArcRadius);

            Vector3 spawnPos = basedPos + offset;
            GameObject go = Instantiate(thunderCloudPrefab, spawnPos, Quaternion.identity);

            var cloud = go.GetComponent<LightningCloud>();
            if (cloud == null) { cloud = go.AddComponent<LightningCloud>(); }
            cloud.Init(this);

            list.Add(cloud);
        }

        return list;
    }

    public IEnumerator StormRandomStrikesRoutine(Transform target, Health targetHp)
    {
        while (true)
        {
            if (target == null)
            {
                yield return null;
                continue;
            }

            Vector2 r = Random.insideUnitCircle * 4f;
            Vector3 pos = target.position + new Vector3(r.x, 0f, r.y);

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

            float dist = Vector3.Distance(target.position, pos);
            if (dist <= stormRandomStrikeRadius)
            {
                targetHp.TakeDamage(stormRandomStrikeDamage);
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
