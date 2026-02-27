using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class FireAbilityPack : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject warningCirclePrefab;
    public GameObject flamePillarPrefab;
    public GameObject meteorPrefab;
    public GameObject fireballProjectilePrefab;
    public GameObject dashTrailPrefab;
    public Transform fireballMuzzle;
    public Transform dashTrailSpawn;

    [Header("Arena")]
    public Transform arenaCenter;
    public float arenaRadius = 18f;

    [Header("Layers")]
    public LayerMask groundMask;
    public LayerMask playerMask;

    [Header("Dash")]
    public float dashWindup = 0.25f;
    public float dashDuration = 0.35f;
    public float dashSpeed = 25f;
    public float dashDamage = 20f;
    public float dashHitRadius = 2.0f;
    public float dashSplashRadius = 3f;
    public float dashTrailDuration = 0.25f;
    public float dashTrailtickRate = 0.25f;
    public float dashTrailDamagePerTick = 6f;

    [Header("Pillaers")]
    public int pillarsCount = 3;
    public float pillarRadiusFromPlayer = 4f;
    public float pillarAoERadiuls = 2.2f;
    public float pillarTelegraphTime = 0.8f;
    public float pillarLifetime = 2.2f;
    public float pillarDamagePerTick = 7f;
    public float pillarTickRate = 0.25f;

    [Header("Fireball")]
    public float fireballLifetime = 7f;
    public float fireballSpeed = 35f;
    public float fireballDamage = 40f;

    [Header("Enraged Fireball")]
    public float enragedChargeTime = 1.2f;
    public float enragedFireballSpeed = 38f;
    public float enragedFireBallDamage = 45f;

    [Header("Meteor Drop")]
    public int meteorCount = 8;
    public float meteorTelegraphTime = 0.9f;
    public float meteorSpawnHeight = 20f;
    public float meteorImpactRadius = 2.6f;
    public float meteorImpactDamage = 25f;
    public float meteorFallSpeed = 22f;

    public float baseDamageMultiplier = 1.0f;

    public void CastFireball(Transform caster, Transform target, bool charged = false)
    {
        if (caster == null || target == null) { return; }
        StartCoroutine(Fireball(caster, target, charged));
    }

    public void CastFirePillars(Transform target)
    {
        if (target == null) { return; }
        StartCoroutine(FlamePillars(target));
    }

    public void CastMeteorBurst()
    {
        StartCoroutine(MeteorBurst());
    }

    public void CastDash(Rigidbody casterBody, Transform casterTransform, Transform target)
    {
        if (casterBody == null || casterTransform == null || target == null) { return; }
        StartCoroutine(Dash(casterBody, casterTransform, target));
    }

    public IEnumerator Dash(Rigidbody body, Transform caster, Transform target)
    {
        body.linearVelocity = Vector3.zero;

        yield return new WaitForSeconds(dashWindup);

        Vector3 toPlayer = target.position - caster.position;
        toPlayer.y = 0f;
        Vector3 dashDir = (toPlayer.sqrMagnitude < 0.01f) ? caster.forward : toPlayer.normalized;

        if (dashTrailPrefab != null)
        {
            Vector3 trailPos = dashTrailSpawn != null ? dashTrailSpawn.position : caster.position;
            GameObject trail = Instantiate(dashTrailPrefab, trailPos, Quaternion.identity);

            var t = trail.GetComponent<BurningTrail>();
            if (t != null)
            {
                t.Init(dashTrailDamagePerTick * baseDamageMultiplier, dashTrailtickRate, dashTrailDuration, playerMask);
                t.Follow(caster);
            }

            Destroy(trail, dashTrailDuration + 0.05f);
        }

        float endTime = Time.time + dashDuration;
        bool didHit = false;

        while (Time.time < endTime)
        {
            body.linearVelocity = dashDir * dashSpeed;
            if (!didHit)
            {
                //Health playerHealth = GetComponent<Health>();
                //if (playerHealth != null)
                //{
                //    playerHealth.TakeDamage(dashDamage);
                //}

                Collider[] hits = Physics.OverlapSphere(caster.position, dashHitRadius, playerMask);
                if (hits.Length > 0)
                {
                    didHit = true;
                    DealSplashDamage(caster.position, dashSplashRadius, dashDamage * baseDamageMultiplier);
                }
            }
            yield return null;
        }

        body.linearVelocity = Vector3.zero;
    }

    public void DealSplashDamage(Vector3 center, float radius, float damage)
    {
        Collider[] hits = Physics.OverlapSphere(center, radius, playerMask);

        for (int i = 0; i < hits.Length; ++i)
        {
            var hp = hits[i].GetComponentInParent<Health>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
                break;
            }
        }
    }

    public IEnumerator FlamePillars(Transform target)
    {
        Vector3 basePos = target.position;

        for (int i = 0; i < pillarsCount; ++i)
        {
            Vector3 p = GetRandomPointNear(basePos, pillarRadiusFromPlayer);
            p = SnapToGround(p);
            p = ClampToArena(p);

            SpawnWarning(p, pillarTelegraphTime);

            StartCoroutine(SpawnPillarAfterDelay(p, pillarTelegraphTime));
        }

        yield return new WaitForSeconds(0.15f);
    }

    public IEnumerator SpawnPillarAfterDelay(Vector3 pos, float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject pillar = null;
        if (flamePillarPrefab != null)
        {
            pillar = Instantiate(flamePillarPrefab, pos, Quaternion.identity);
        }

        var aoe = pillar.GetComponent<FlamePillarAoE>();
        if (aoe != null)
        {
            aoe.Init(pillarDamagePerTick * baseDamageMultiplier, pillarTickRate, pillarLifetime, pillarAoERadiuls, playerMask);
        }

        Destroy(pillar, pillarLifetime + 0.05f);
    }

    public IEnumerator MeteorBurst()
    {
        for (int i = 0; i < meteorCount; ++i)
        {
            Vector3 p = GetRandomPointInArena();
            p = SnapToGround(p);
            SpawnWarning(p, meteorTelegraphTime);
            StartCoroutine(SpawnMeteorAfterDelay(p, meteorTelegraphTime));
        }

        yield return null;
    }

    public IEnumerator SpawnMeteorAfterDelay(Vector3 impactPoint, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (meteorPrefab == null) { yield break; }

        Vector3 spawnPos = impactPoint + Vector3.up * meteorSpawnHeight;

        GameObject meteor = Instantiate(meteorPrefab, spawnPos, Quaternion.identity);

        var proj = meteor.GetComponent<MeteorProjectile>();
        if (proj != null)
        {
            proj.Init(impactPoint, meteorFallSpeed, meteorImpactDamage * baseDamageMultiplier, meteorImpactRadius, playerMask);
        }
    }

    public IEnumerator Fireball(Transform caster, Transform target, bool charged)
    {
        yield return charged ? new WaitForSeconds(enragedChargeTime) : new WaitForSeconds(0.15f);

        Transform muzzle = fireballMuzzle != null ? fireballMuzzle : caster;

        Vector3 dir = (target.position - muzzle.position);
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) { dir = caster.forward; }
        dir = dir.normalized;

        float speed = charged ? enragedFireballSpeed : fireballSpeed;
        float damage = charged ? enragedFireBallDamage : fireballDamage;

        SpawnFireball(muzzle.position, dir, speed, damage * baseDamageMultiplier, charged);
    }

    //[System.Obsolete]
    //private IEnumerator FireballCharged()
    //{
    //    isBusy = true;
    //    fireball = Time.time;

    //    body.velocity = Vector3.zero;

    //    //anim.SetTrigger("ChargeFireBall");

    //    yield return new WaitForSeconds(enragedChargeTime);

    //    Vector3 dir = (player.position - fireballMuzzle.position);
    //    dir.y = 0f;
    //    dir = dir.normalized;

    //    SpawnFireball(dir, enragedFireballSpeed, enragedFireBallDamage * baseDamageMultiplier, true);

    //    yield return new WaitForSeconds(0.15f);
    //    isBusy = false;
    //}

    public void SpawnFireball(Vector3 spawnPos, Vector3 dir, float speed, float damage, bool charged)
    {
        if (fireballProjectilePrefab == null) { return; }

        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
        GameObject fb = Instantiate(fireballProjectilePrefab, spawnPos, rot);
        var proj = fb.GetComponent<Fireball>();
        if (proj != null)
        {
            proj.Init(dir, speed, damage, fireballLifetime, playerMask, charged);
        }
    }

    public Vector3 ClampToArena(Vector3 pos)
    {
        if (arenaCenter == null) { return pos; }

        Vector3 center = arenaCenter.position;

        Vector3 flat = pos - center;
        flat.y = 0f;

        float dist = flat.magnitude;
        if (dist > arenaRadius)
        {
            Vector3 clampFlat = flat.normalized * arenaRadius;
            Vector3 outPos = center + clampFlat;
            outPos.y = clampFlat.y;
            return outPos;
        }

        return pos;
    }

    public Vector3 GetRandomPointInArena()
    {
        if (arenaCenter == null) { return transform.position; }
        Vector2 r = Random.insideUnitCircle * arenaRadius;
        return arenaCenter.position + new Vector3(r.x, 0f, r.y);
    }

    public Vector3 GetRandomPointNear(Vector3 origin, float radius)
    {
        Vector2 r = Random.insideUnitCircle * radius;
        return origin + new Vector3(r.x, 0f, r.y);
    }

    public Vector3 SnapToGround(Vector3 pos)
    {
        Ray ray = new Ray(pos + Vector3.up * 15f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 50f, groundMask))
        {
            return hit.point;
        }
        return pos;
    }

    public GameObject SpawnWarning(Vector3 pos, float duration)
    {
        if (warningCirclePrefab == null) { return null; }

        GameObject w = Instantiate(warningCirclePrefab, pos, Quaternion.identity);
        Destroy(w, duration);
        return w;
    }
}
