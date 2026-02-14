using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class FireBossAI : MonoBehaviour
{
    [Header("Rreferences")]
    public Transform player;
    public Transform fireBoss;
    public Animator anim;
    public Rigidbody body;

    [Header("Prefabs")]
    public GameObject warningCirclePrefab;
    public GameObject flamePillarPrefab;
    public GameObject meteorPrefab;
    public GameObject fireballProjectilePrefab;
    public GameObject auraVFXPrefab;
    public Transform fireballMuzzle;

    [Header("Arena / Movement")]
    public Transform arenaCenter;
    public float arenaRadius = 18f;

    [Header("Health")]
    public Health maxHP;
    public float currentHP;

    //[Header("Core State")]
    private enum BossState
    {
        Idle,
        Chasing,
        Recovering,
        Attacking,
        Stunned,
        Dead
    }
    private BossState bossState = BossState.Idle;

    private bool isBusy = false;
    private bool isEnraged = false;

    public float rotateSpeed = 12f;
    public float desiredCombatRange = 6f;

    [Header("Damage / Multiplier")]
    public float baseDamageMultiplier = 1.0f;
    public float enragedDamageMultiplier = 1.5f;

    [Header("Timers and Cooldown")]
    public float meteorCooldown = 7.0f;
    public float meteorTime;

    public float dashCooldown = 2.0f;
    public float dashTime;

    public float pillarCooldown = 3.5f;
    public float pillarTime;

    public float fireballCooldown = 4.0f;
    public float fireballTime;

    [Header("Attack Data")]
    [Header("Dash")]
    public float dashWindup = 0.25f;
    public float dashDuration = 0.35f;
    public float dashSpeed = 25f;
    public float dashDamage = 20f;
    public float dashHitRadius = 2.0f;

    [Header("Pillaers")]
    public int pillarsCount = 3;
    public float pillarTelegraphTime = 0.8f;
    public float pillarLifetime = 2.2f;
    public float pillarDamagePerTick = 7f;
    public float pillarTickRate = 0.25f;

    [Header("Fireball")]
    public float fireballChargeTime = 1.2f;
    public float fireballSpeed = 35f;
    public float fireballDamage = 40f;

    public float enragedCooldownMultiplier = 0.75f;

    public void Start()
    {
        if (fireBoss != null)
        {
            Health bossHealth = GetComponent<Health>();
            currentHP = bossHealth.maxHealth;
        }
       
        SetAura(false);

        dashTime = -999f;
        pillarTime = -999f;
        meteorTime = -999f;
        fireballTime = -999f;

        bossState = BossState.Chasing;
    }

    public void Update()
    {
        if (bossState == BossState.Dead)
        {
            return;
        }
        if (player == null)
        {
            return;
        }

        CheckEnrage();
        FacePlayerSmooth();
        MaintainArenaBounds();

        if (isBusy) { return; }

        float dist = DistanceToPlayerFlat();
        if (dist > desiredCombatRange)
        {
            bossState = BossState.Attacking;
            ChooseAndExecuteAttack();
        }
    }

    

    private void CheckEnrage()
    {
        if (fireBoss != null)
        {
            Health bossHealth = GetComponent<Health>();
            currentHP = bossHealth.maxHealth;
            if (!isEnraged && currentHP <= bossHealth.maxHealth * 0.5f)
            {
                EnterEnrageMode();
            }
        }
    }

    private void EnterEnrageMode()
    {
        isEnraged = true;
        baseDamageMultiplier = enragedDamageMultiplier;

        SetAura(true);

        dashCooldown *= enragedCooldownMultiplier;
        pillarCooldown *= enragedCooldownMultiplier;
        meteorCooldown *= enragedCooldownMultiplier;
        fireballCooldown *= enragedCooldownMultiplier;

        rotateSpeed *= 1.15f;
    }    

    private void SetAura(bool on)
    {
        if (auraVFXPrefab != null)
        {
            auraVFXPrefab.SetActive(on);
        }
    }    

    private void ChooseAndExecuteAttack()
    {
        if (isEnraged && IsReady(fireballTime, fireballCooldown))
        {
            StartCoroutine(Fireball());
            return;
        }

        if (isEnraged && IsReady(dashTime, dashCooldown))
        {
            StartCoroutine(Dash());
            return;
        }

        if (isEnraged && IsReady(pillarTime, pillarCooldown))
        {
            StartCoroutine(FlamePillars());
            return;
        }

        if (isEnraged && IsReady(meteorTime, meteorCooldown))
        {
            StartCoroutine(MeteorBurst());
            return;
        }
    }

    private bool IsReady(float lastTime, float cooldown)
    {
        return Time.time >= lastTime + cooldown;
    }

    public IEnumerator Dash()
    {
        isBusy = true;
        dashTime = Time.time;

        yield return new WaitForSeconds(dashWindup);

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        Vector3 dashDir = toPlayer.normalized;

        float endTime = Time.time + dashDuration;

        while (Time.time < endTime)
        {
            body.angularVelocity = dashDir * dashSpeed;
            if (Physics.CheckSphere(transform.position, dashHitRadius))
            {
                Health playerHealth = GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(dashDamage);
                }
            }
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        isBusy = false;
    }

    private IEnumerator FlamePillars()
    {
        isBusy = true;
        pillarTime = Time.time;

        anim.SetTrigger("CastPillars");

        Vector3 basePos = player.position;
        basePos.y = arenaCenter.position.y;

        for (int i = 0; i < pillarsCount; ++i)
        {
            Vector3 p = GetRandomPointNear(basePos, 3.5f);
            p = ClampToArena(p);

            GameObject warn = SpawnWarning(p, pillarTelegraphTime);

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

        StartCoroutine(PillarDamageTick(pillar));

        if (pillar != null)
        {
            Destroy(pillar, pillarLifetime);
        }
    }

    private IEnumerator MeteorBurst()
    {
        isBusy = true;
        meteorTime = Time.time;

        anim.SetTrigger("Meteor");

        int count = 5;
        float telegraph = 0.08f;

        for (int i = 0; i < count; ++i)
        {
            Vector3 p = GetRandomPointInArena();
            SpawnWarning(p, telegraph);
            StartCoroutine(SpawnMeteorAfterDelay(p, telegraph));
        }

        yield return new WaitForSeconds(0.25f);
        isBusy = false;
    }

    private IEnumerator SpawnMeteorAfterDelay(Vector3 pos, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 spawnPos = pos + Vector3.up * 18f;

        if (meteorPrefab != null)
        {
            Instantiate(meteorPrefab, spawnPos, Quaternion.identity);
        }
    }

    private IEnumerator Fireball()
    {
        isBusy = true;
        fireballTime = Time.time;

        anim.SetTrigger("ChargeFireBall");

        body.angularVelocity = Vector3.zero;
        yield return new WaitForSeconds(fireballChargeTime);

        Vector3 dir = (player.position - fireballMuzzle.position);
        dir.y = 0f;
        dir = dir.normalized;

        SpawnFireball(fireballMuzzle.position, dir);

        yield return new WaitForSeconds(0.15f);
        isBusy = false;
    }

    private IEnumerator SpawnFireball(Vector3 pos, Vector3 dir)
    {
        if (fireballProjectilePrefab != null)
        {
            return null;
        }

        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
        GameObject proj = Instantiate(fireballProjectilePrefab, pos, rot);

        proj.GetComponent<Fireball>().Init(dir, fireballSpeed, fireballDamage * baseDamageMultiplier);

        yield return new WaitForSeconds(fireballCooldown);
    }

    private IEnumerator MicroReposition()
    {
        isBusy = true;

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        Vector3 foward = toPlayer.normalized;
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

        Vector3 dir = toPlayer.normalized;
        float moveSpeed = isEnraged ? 7.5f : 6.5f;

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
        Vector3 clamp = ClampToArena(transform.position);
        if ((clamp - transform.position).sqrMagnitude > 0.0001f)
        {
            body.MovePosition(clamp);
        }
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
        Vector3 p = arenaCenter.position + new Vector3(r.x, 0f, r.y);
        return p;
    }

    private Vector3 GetRandomPointNear(Vector3 origin, float radius)
    {
        Vector2 r = Random.insideUnitCircle * radius;
        Vector3 p = origin + new Vector3(r.x, 0f, r.y);
        return p;
    }

    private GameObject SpawnWarning(Vector3 pos, float duration)
    {
        if (warningCirclePrefab == null) { return null; }

        GameObject w = Instantiate(warningCirclePrefab, pos, Quaternion.identity);
        Destroy(w, duration);
        return w;
    }

   private void PillarDamageTick(GameObject pillar)
    {

    }
}
