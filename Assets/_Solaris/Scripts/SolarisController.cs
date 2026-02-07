using System.Collections;
using UnityEngine;

public class SolarisController : MonoBehaviour
{
    // =================================
    // Phase Management
    // =================================
    public enum SolarisPhase
    {
        Phase1,
        Phase2,
        Phase3
    }

    [Header("Phase")]
    public SolarisPhase currentPhase = SolarisPhase.Phase1;
    [Range(0f, 1f)] public float phase2HealthThreshold = 0.60f;
    private bool enterPhase2 = false;
    private bool wallPermanentlyDisabled = false;

    // =================================
    // References
    // =================================
    [Header("References")]
    public Transform playerTransform;
    [SerializeField] public Health solarisHealth;

    [SerializeField] private SolarisDaggers daggers;
    [SerializeField] private SolarisFlareTelegraph flareTelegraph;

    public Health playerHealth;

    [SerializeField] private WallOfLight wallOfLight;
    [SerializeField] private WallOfLightShield wallOfLightShield;

    // =================================
    // Teleport/ Movement
    // =================================
    [Header("Timing / Movement")]
    public Transform[] teleportPoints;
    public float teleportInterval = 3f;
    public float teleportTimer = 0f;
    public float teleportMaxTimer = 20f;

    // =================================
    // Attack Timing
    // =================================
    [Header("Phase 1 Timing")]
    public float flareAttackDelayMaxTimer = 5f;
    public float flareAttackCoolDown = 0f;
    public float daggersAttackDelayMaxTimer = 10f;
    public float daggersAttackCoolDown = 0f;

    /// =================================
    /// Fight State
    /// =================================
    [Header("Fight State")]
    public bool fightActive = true;

    // =================================
    // Wall of Light (Phase 2)
    // =================================
    [Header("Wall of Light")]
    public bool wallOfLightActive = false;
    [Tooltip("Duration the Wall of Light remains active during Phase 2")]
    public float wallOfLightDuration = 6f;

    [Tooltip("How much Solaris Heals per second while Wall of Light is active")]
    public float wallOfLightHealPerSecond = 5f;

    [Tooltip("Shield HP the player must break to disable the Wall of Light permanently")]
    public float wallOfLightShieldHP = 250f;

    [Header("Wall Damage & Heal Balancing")]
    [Range(0f, 1f)] public float wallDamagePassThroughMultiplier = 0.15f;

    [Range(0f, 1f)] public float wallHealCapPercent = 0.75f;

    [Header("Phase 2 Timing")]
    public float phase2CycleInterval = 6f;
    public float phase2DaggerBurstTime = 2.5f;
    public float phase2BurstTick = 0.6f;

    // =================================
    // Phase 3 Vulnerability
    // =================================
    [Header("Phase 3")]
    [Tooltip("In Phase 3 Solaris takes more damege when wall of light is permanetly disabled")]
    public float phase3DamageMultiplier = 1.5f;

    // =================================
    // Debugging
    // =================================
    [Header("Debugging")]
    public bool debugManualDamage = true;

    private Coroutine phaseRoutine;

    // =================================
    // Unity Methods
    // =================================
    private void Awake()
    {
        if (daggers == null) { daggers = GetComponent<SolarisDaggers>(); }
        if (flareTelegraph == null) { flareTelegraph = GetComponent<SolarisFlareTelegraph>(); }
        if (solarisHealth == null) { solarisHealth = GetComponent<Health>(); }
        if (wallOfLightShield != null)
        {   
            wallOfLightShield.OnShieldBroken += OnWallBroken;
            //wallOfLight.Deactivate();
        }

        DeactivateWallOfLight();
        StartPhaseLoop(currentPhase);
    }

    private void Update()
    {
        // Manual damage for testing
        if (debugManualDamage && Input.GetKeyDown(KeyCode.Y))
        {
            if(currentPhase == SolarisPhase.Phase2 && wallOfLight != null && wallOfLightActive == true)
            {
                float damageToWall = 10f;
                wallOfLightShield.TakeShieldDamage(damageToWall);
                ApplyChipDamageDuringWall(damageToWall);
            }
            else
            {
                TakeDamage(10f);
            }
        }

        // Safety checks
        if (!fightActive) { return; }
        if (playerTransform == null) { return; }
        if (teleportPoints == null || teleportPoints.Length == 0) { return; }

        // Always on behavior
        HandleCommon();

        teleportTimer -= Time.deltaTime;
        flareAttackCoolDown -= Time.deltaTime;
        daggersAttackCoolDown -= Time.deltaTime;

        if (wallOfLightActive && solarisHealth != null)
        {
            float healCap = solarisHealth.maxHealth * wallHealCapPercent;
            if (solarisHealth.currentHealth < healCap)
            {
                float healAmount = wallOfLightHealPerSecond * Time.deltaTime;

                if (solarisHealth.currentHealth + healAmount > healCap)
                {
                    healAmount = healCap - solarisHealth.currentHealth;
                }

                solarisHealth.Heal(healAmount);
            }
        }

        if (playerHealth != null && playerHealth.IsDead)
        {
            OnPlayerDefeated();
            return;
        }

        if (solarisHealth != null && solarisHealth.currentHealth <= 0)
        {
            OnSolarisDefeated();
            return;
        }

        // Phase transitions
        CheckPhaseTransitions();
    }

    // =================================
    // Always-On Behavior
    // =================================
    void HandleCommon()
    {
        FacePlayer();
        TickTeleport();
    }

    void FacePlayer()
    {
        Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0f; // Keep only horizontal direction

        if (direction != Vector3.zero)
        { 
            transform.rotation = Quaternion.LookRotation(direction); 
        }
    }

    private void TickTeleport()
    {

        if (teleportTimer <= 0)
        {
            TeleportToRandomPoint();
            teleportTimer = teleportMaxTimer;
        }
    }

    void TeleportToRandomPoint()
    {
        int randomIndex = Random.Range(0, teleportPoints.Length);
        transform.position = teleportPoints[randomIndex].position;
    }
   

    // =================================
    // Phase Transition
    // =================================
    void CheckPhaseTransitions()
    {
        if (solarisHealth == null) { return; }

        if (!enterPhase2 && currentPhase == SolarisPhase.Phase1)
        {

            float hp01 = (float)solarisHealth.currentHealth / solarisHealth.maxHealth;
            if (hp01 <= phase2HealthThreshold)
            {
                EnterPhase2();
            }
        }
    }

    void EnterPhase2()
    {
        enterPhase2 = true;
        currentPhase = SolarisPhase.Phase2;
        Debug.Log("Entering Phase 2");

        StartPhaseLoop(SolarisPhase.Phase2);
    }

    void EnterPhase3() 
    {
        currentPhase = SolarisPhase.Phase3;
        Debug.Log("Entering Phase 3");

        wallPermanentlyDisabled = true;
        DeactivateWallOfLight();

        StartPhaseLoop(SolarisPhase.Phase3);
    }

    // =================================
    // Phase Loop Management
    // =================================
    private void StartPhaseLoop(SolarisPhase phase)
    {
        if (phaseRoutine != null)
        {
            StopCoroutine(phaseRoutine);
        }
        phaseRoutine = StartCoroutine(PhaseLoop(phase));
    }
    private IEnumerator PhaseLoop(SolarisPhase phase)
    {
        switch (phase)
        {
            case SolarisPhase.Phase1:
                yield return StartCoroutine(Phase1Loop());
                break;
            case SolarisPhase.Phase2:
                yield return StartCoroutine(Phase2Loop());
                break;
            case SolarisPhase.Phase3:
                while (fightActive && currentPhase == SolarisPhase.Phase3)
                {
                    yield return null;
                }
                break;
            default:
                break;

        }
    }

    // =================================
    // Phase 1: Basic Teleport and Dagger Attacks
    private IEnumerator Phase1Loop()
    {
        while (fightActive && currentPhase == SolarisPhase.Phase1)
        {
            if (flareTelegraph != null && flareAttackCoolDown <= 0f)
            {
                flareTelegraph.TriggerFlare(playerTransform);
                Debug.Log("FLARE FIRED " + Time.time);
                flareAttackCoolDown = flareAttackDelayMaxTimer;
            }
                
            yield return new WaitForSeconds(flareAttackDelayMaxTimer);

            if (daggers != null && daggersAttackCoolDown <= 0f)
            {
                daggers.TryFire(playerTransform);
                Debug.Log("DAGGERS FIRED " + Time.time);
                daggersAttackCoolDown = daggersAttackDelayMaxTimer;
            }
                
            yield return new WaitForSeconds(daggersAttackDelayMaxTimer);

            //yield return null;
        }
    }

    // =================================
    // Phase 2: Wall of Light + Dagger Bursts
    // =================================

    private IEnumerator Phase2Loop()
    {
        ActivateWallOfLight();

        while (fightActive && currentPhase == SolarisPhase.Phase2)
        {
            if (!wallOfLightActive) { yield break; }
            if (flareTelegraph != null)
            {
                flareTelegraph.TriggerFlare(playerTransform);
                flareAttackCoolDown = flareAttackDelayMaxTimer;
            }

            if (daggers != null)
            {
                daggers.TryFire(playerTransform);
                daggersAttackCoolDown = daggersAttackDelayMaxTimer;
            }
           
            yield return new WaitForSeconds(phase2BurstTick);

            //if (wallOfLightShieldHP == 0)
            //{
            //    DeactivateWallOfLight();
            //}

            yield break;
        }
    }
    void ActivateWallOfLight()
    {
        if (wallPermanentlyDisabled) { return; }

        wallOfLightActive = true;

        if (wallOfLight != null)
        { 
            wallOfLight.Activate(); 
        }

        if (wallOfLightShield != null)
        {
            wallOfLightShield.maxShieldHP = wallOfLightShieldHP;
            wallOfLightShield.Active(wallOfLightShieldHP);
        }

        Debug.Log("Wall of Light Activated");

        //Invoke(nameof(DeactivateWallOfLight), wallOfLightDuration);
    }

    void DeactivateWallOfLight()
    {
        wallOfLightActive = false;

        if (wallOfLight != null)
        { 
            wallOfLight.Deactivate(); 
        }

        if (wallOfLightShield != null)
        {
            wallOfLightShield.Deactivate();
        }

        Debug.Log("Wall of Light Deactivated");
        //CancelInvoke(nameof(DeactivateWallOfLight));
    }

    public void OnWallBroken()
    {
        Debug.Log("Wall of Light BROKEN by player");
        EnterPhase3();
    }

    // =================================
    // Damage API
    // =================================
    public void TakeDamage(float damageAmount)
    {
        if (solarisHealth == null) { return; }

        float finalDamage = damageAmount;

        if (currentPhase == SolarisPhase.Phase2 && wallOfLightActive)
        {
            finalDamage *= wallDamagePassThroughMultiplier;
        }

        if (currentPhase == SolarisPhase.Phase3 && wallPermanentlyDisabled)
        {
            finalDamage *= phase3DamageMultiplier;
        }

        solarisHealth.TakeDamage(finalDamage);
    }

    public void ApplyChipDamageDuringWall(float incomingDamage)
    {
        if (solarisHealth == null) { return; }
        if (currentPhase != SolarisPhase.Phase2) { return; }
        if (!wallOfLightActive) { return; }

        float chipDamage = incomingDamage * wallDamagePassThroughMultiplier;
        if (chipDamage <= 0f) { return; }

        solarisHealth.TakeDamage(chipDamage);
    }

    /// =================================
    // End stats
    /// =================================

    void OnPlayerDefeated()
    {
        fightActive = false;

        StopAllCoroutines();

        Debug.Log("Solaris: You are not ready. Retry?");
    }
    
    private void OnSolarisDefeated()
    {
        fightActive = false;
        Debug.Log("Solaris: You have proven yourself worthy...");
    }

}


