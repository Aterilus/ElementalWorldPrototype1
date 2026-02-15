using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class FireBossAI : MonoBehaviour
{
    [Header("Rreferences")]
    public Transform player;
    public Animator anim;
    public Rigidbody body;

    [Header("Prefabs")]
    public GameObject warningCirclePrefab;
    public GameObject flamePillarPrefab;
    public GameObject meteorPrefab;
    public GameObject fireballProjectilePrefab;
    public GameObject auraVFXPrefab;
    public GameObject dashTrailPrefab;
    public Transform fireballMuzzle;
    public Transform dashTrailSpawn;

    [Header("Arena")]
    public Transform arenaCenter;
    public float arenaRadius = 18f;

    [Header("Health / Enrage State")]
    public Health maxHP;
    public float currentHP;
    private bool isEnraged = false;
    public float enragedDamageMultiplier = 1.5f;
    public float enragedCooldownMultiplier = 0.75f;
    public float baseDamageMultiplier = 1.0f;

    [Header("Movement")]
    public float rotateSpeed = 12f;
    public float desiredCombatRange = 6f;
    public float chaseSpeed = 6.5f;
    public float chaseSpeedEnraged = 8.5f;

    [Header("Layers")]
    public LayerMask groundMask;
    public LayerMask playerMask;

    [Header("Timers and Cooldown")]
    public float meteorCooldown = 7.0f;
    public float meteor;

    public float dashCooldown = 2.0f;
    public float dash;

    public float pillarCooldown = 3.5f;
    public float pillar;

    public float fireballCooldown = 4.0f;
    public float fireball;

    [Header("Attack Data")]
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

    private bool isBusy = false;

    public void Start()
    {
        if (body == null) { GetComponent<Rigidbody>(); }
        Health bossHealth = GetComponent<Health>();
        if (bossHealth != null)
        {
            currentHP = bossHealth.maxHealth;
        }
        SetAura(false);
    }

    [System.Obsolete]
    public void Update()
    {
        if(player == null || isBusy) { return; }

        CheckEnrage();
        FacePlayerSmooth();
        MaintainArenaBounds();

        float dist = DistanceToPlayerFlat();
        if (dist > desiredCombatRange)
        {
            ChasePlayerFlat();
        }
        else
        {
            ChooseAndExecuteAttack();
        }
    }

    

    private void CheckEnrage()
    {
       if (isEnraged) { return; }
       Health bossHealth = GetComponent<Health>();
       if (currentHP <= bossHealth.maxHealth * 0.5f)
        {
            isEnraged = true;
            baseDamageMultiplier = enragedDamageMultiplier;
            SetAura(true);

            pillar *= enragedCooldownMultiplier;
            meteor *= enragedCooldownMultiplier;
            fireball *= enragedCooldownMultiplier;
            dash *= enragedCooldownMultiplier;

            rotateSpeed *= 2f;
        }
    }   

    private void SetAura(bool on)
    {
        if (auraVFXPrefab != null)
        {
            auraVFXPrefab.SetActive(on);
        }
    }

    [System.Obsolete]
    private void ChooseAndExecuteAttack()
    {
        if (!isEnraged && IsReady(fireball, fireballCooldown)) 
        {
            StartCoroutine(Fireball());
            return; 
        }
        if (isEnraged && IsReady(fireball, fireballCooldown))
        {
            StartCoroutine(FireballCharged());
            return;
        }

        if (IsReady(dash, dashCooldown))
        {
            StartCoroutine(Dash());
            return;
        }

        if (IsReady(pillar, pillarCooldown))
        {
            StartCoroutine(FlamePillars());
            return;
        }

        if (IsReady(meteor, meteorCooldown))
        {
            StartCoroutine(MeteorBurst());
            return;
        }

        StartCoroutine(MicroReposition());
    }

    private bool IsReady(float lastTime, float cooldown) => Time.time >= lastTime + cooldown;

    [System.Obsolete]
    public IEnumerator Dash()
    {
        isBusy = true;
        dash = Time.time;

        body.velocity = Vector3.zero;

        yield return new WaitForSeconds(dashWindup);

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        Vector3 dashDir =( toPlayer.sqrMagnitude < 0.01f) ? transform.forward : toPlayer.normalized;

        if (dashTrailPrefab != null)
        {
            Vector3 trailPos = dashTrailSpawn != null ? dashTrailSpawn.position : transform.position;
            GameObject trail = Instantiate(dashTrailPrefab, trailPos, Quaternion.identity);

            var t = trail.GetComponent<BurningTrail>();
            if (t != null)
            {
                t.Init(dashTrailDamagePerTick * baseDamageMultiplier, dashTrailtickRate, dashTrailDuration, playerMask);
                t.Follow(transform);
            }

            Destroy(trail, dashTrailDuration + 0.05f);
        }

        float endTime = Time.time + dashDuration;
        bool didHit = false;

        while (Time.time < endTime)
        {
            body.velocity = dashDir * dashSpeed;
            if (!didHit)
            {
                //Health playerHealth = GetComponent<Health>();
                //if (playerHealth != null)
                //{
                //    playerHealth.TakeDamage(dashDamage);
                //}

                Collider[] hits = Physics.OverlapSphere(transform.position, dashHitRadius, playerMask);
                if (hits.Length > 0)
                {
                    didHit = true;
                    DealSplashDamage(transform.position, dashSplashRadius, dashDamage * baseDamageMultiplier);
                }
            }
            yield return null;
        }

        body.velocity = Vector3.zero;

        yield return new WaitForSeconds(0.2f);
        isBusy = false;
    }

    private void DealSplashDamage(Vector3 center, float radius, float damage)
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

    private IEnumerator FlamePillars()
    {
        isBusy = true;
        pillar = Time.time;

        anim.SetTrigger("CastPillars");

        Vector3 basePos = player.position;

        for (int i = 0; i < pillarsCount; ++i)
        {
            Vector3 p = GetRandomPointNear(basePos, pillarRadiusFromPlayer);
            p = SnapToGround(p);
            p = ClampToArena(p);

            SpawnWarning(p, pillarTelegraphTime);

            StartCoroutine(SpawnPillarAfterDelay(p, pillarTelegraphTime));
        }

        yield return new WaitForSeconds(0.15f);
        isBusy = false;
    }

    private IEnumerator SpawnPillarAfterDelay(Vector3 pos, float delay)
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

    private IEnumerator MeteorBurst()
    {
        isBusy = true;
        meteor = Time.time;

        //anim.SetTrigger("Meteor");

        for (int i = 0; i < meteorCount; ++i)
        {
            Vector3 p = GetRandomPointInArena();
            p = SnapToGround(p);
            SpawnWarning(p, meteorTelegraphTime);
            StartCoroutine(SpawnMeteorAfterDelay(p, meteorTelegraphTime));
        }

        yield return new WaitForSeconds(0.25f);
        isBusy = false;
    }

    private IEnumerator SpawnMeteorAfterDelay(Vector3 impactPoint, float delay)
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

    [System.Obsolete]
    private IEnumerator Fireball()
    {
        isBusy = true;
        fireball = Time.time;

        body.velocity = Vector3.zero;

        // anim.SetTrigger("ChargeFireBall");

        yield return new WaitForSeconds(0.15f);

        Vector3 dir = (player.position - fireballMuzzle.position);
        dir.y = 0f;
        dir = dir.normalized;

        SpawnFireball(dir, fireballSpeed, fireballDamage * baseDamageMultiplier, false);

        yield return new WaitForSeconds(0.15f);
        isBusy = false;
    }

    [System.Obsolete]
    private IEnumerator FireballCharged()
    {
        isBusy = true;
        fireball = Time.time;

        body.velocity = Vector3.zero;

        //anim.SetTrigger("ChargeFireBall");

        yield return new WaitForSeconds(enragedChargeTime);

        Vector3 dir = (player.position - fireballMuzzle.position);
        dir.y = 0f;
        dir = dir.normalized;

        SpawnFireball(dir, enragedFireballSpeed, enragedFireBallDamage * baseDamageMultiplier, true);

        yield return new WaitForSeconds(0.15f);
        isBusy = false;
    }

    private void SpawnFireball(Vector3 dir, float speed, float damage, bool charged)
    {
        if (fireballProjectilePrefab == null || fireballMuzzle == null) { return; }
        
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
        GameObject fb = Instantiate(fireballProjectilePrefab, fireballMuzzle .position, rot);
        var proj = fb.GetComponent<Fireball>();
        if (proj != null)
        {
            proj.Init(dir, speed, damage, fireballLifetime, playerMask, charged);
        }
    }

    private IEnumerator MicroReposition()
    {
        isBusy = true;

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        Vector3 foward = (toPlayer.sqrMagnitude < 0.01f) ? transform.forward : toPlayer.normalized;
        Vector3 right = Vector3.Cross(Vector3.up, foward).normalized;

        float sign = (Random.value < 0.5f) ? -1f : 1f;
        Vector3 target = transform.position + right * sign * 2.0f + foward * 1.0f;
        target = ClampToArena(target);

        float t = 0f;
        float dur = 0.2f;
        Vector3 start = transform.position;

        while (t < dur)
        {
            t += Time.deltaTime;
            float a = t/ dur;
            Vector3 next = Vector3.Lerp(start, target, a);
            body.MovePosition(next);
            yield return null;
        }

        isBusy = false;
    }

    private void FacePlayerSmooth()
    {
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;

        if (toPlayer.sqrMagnitude >0.0001f) { return; }

        Quaternion targerRot = Quaternion.LookRotation(toPlayer.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targerRot, rotateSpeed * Time.deltaTime);
    }

    private void ChasePlayerFlat()
    {
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        if (toPlayer.sqrMagnitude < 0.0001f) { return; }

        Vector3 dir = toPlayer.normalized;
        float moveSpeed = isEnraged ? chaseSpeedEnraged : chaseSpeed;

        Vector3 next = transform.position + dir * moveSpeed * Time.deltaTime;
        next = ClampToArena(next);
        body.MovePosition(next);
    }

    private float DistanceToPlayerFlat()
    {
        Vector3 a = transform.position;
        Vector3 b = player.position;
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }

    private void MaintainArenaBounds()
    {
        Vector3 center = arenaCenter.position;
        Vector3 pos = transform.position;

        Vector3 offset = pos - center;
        offset.y = 0f;

        float dist = offset.magnitude;
        if (dist <= arenaRadius) { return; }

        Vector3 clampedOffset = offset.normalized * arenaRadius;
        Vector3 clampedPos = center + clampedOffset;
        clampedPos.y = pos.y;

        body.MovePosition(clampedPos);
        
    }

    private Vector3 ClampToArena(Vector3 pos)
    {
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

    private Vector3 GetRandomPointInArena()
    {
        Vector2 r = Random.insideUnitCircle * arenaRadius;
        return arenaCenter.position + new Vector3(r.x, 0f, r.y);
    }

    private Vector3 GetRandomPointNear(Vector3 origin, float radius)
    {
        Vector2 r = Random.insideUnitCircle * radius;
        return origin + new Vector3(r.x, 0f, r.y);
    }

    private Vector3 SnapToGround(Vector3 pos)
    {
        Ray ray = new Ray(pos + Vector3.up * 15f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 50f, groundMask))
        {
            return hit.point;
        }
        return pos;
    }

    private GameObject SpawnWarning(Vector3 pos, float duration)
    {
        if (warningCirclePrefab == null) { return null; }

        GameObject w = Instantiate(warningCirclePrefab, pos, Quaternion.identity);
        Destroy(w, duration);
        return w;
    }
}
