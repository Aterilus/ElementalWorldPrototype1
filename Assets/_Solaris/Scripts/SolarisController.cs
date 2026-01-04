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

    [Header("Phases")]
    public SolarisPhase currentPhase = SolarisPhase.Phase1;
    [Range(0f, 1f)] public float phase2HealthThreshold = 0.66f;
    private bool enterPhase2 = false;

    // =================================
    // References
    // =================================
    [Header("References")]
    public Transform playerTransform;
    [SerializeField] public Scene2Health solarisHealth;

    [SerializeField] private SolarisDaggers daggers;
    [SerializeField] private SolarisFlareTelegraph flareTelegraph;

    public Health playerHealth;

    [SerializeField] private WallOfLight wallOfLight;

    // =================================
    // Teleport/ Movement
    // =================================
    [Header("Timing / Movement")]
    public Transform[] teleportPoints;
    public float teleportInterval = 3f;
    private float teleportTimer = 0f;

    // =================================
    // Attack Timing
    // =================================
    [Header("Phase 1 Timing")]
    public float attackDelay = 1.5f;
    

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
    public float wallOfLightDuration = 3f;
    public float damageReductionMultiplier = 0.5f;

    [Header("Phase 2 Timing")]
    public float phase2CycleInterval = 6f;
    public float phase2DaggerBurstTime = 2.5f;

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
        if (daggers == null) daggers = GetComponent<SolarisDaggers>();
        if (flareTelegraph == null) flareTelegraph = GetComponent<SolarisFlareTelegraph>();

        if (solarisHealth == null)
        {
            solarisHealth = GetComponent<Scene2Health>();
        }

        StartPhaseLoop(currentPhase);
    }

    private void Update()
    {
        // Safety checks
        if (!fightActive) return;
        if (playerTransform == null) return;
        if (teleportPoints == null || teleportPoints.Length == 0) return;

        // Always on behavior
        HandleCommon();

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

        // Manual damage for testing
        if (debugManualDamage && Input.GetKeyDown(KeyCode.Y))
        {
            TakeDamage(10f);
            Debug.Log("TEST: Y pressed - trying to fire dagger");
        }
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
            transform.rotation = Quaternion.LookRotation(direction);
    }

    private void TickTeleport()
    {
        teleportTimer += Time.deltaTime;
        if (teleportTimer >= teleportInterval)
        {
            TeleportToRandomPoint();
            teleportTimer = 0f;
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
        if (solarisHealth == null)
            return;

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
            if (flareTelegraph != null)
                flareTelegraph.TriggerFlare(playerTransform);

            yield return new WaitForSeconds(attackDelay);

            if (daggers != null)
                daggers.TryFire(playerTransform);

            yield return new WaitForSeconds(0.4f);
        }
    }

    // =================================
    // Phase 2: Wall of Light + Dagger Bursts
    // =================================

    private IEnumerator Phase2Loop()
    {
        while(fightActive && currentPhase == SolarisPhase.Phase2)
            ActivateWallOfLight();

        float t = 0f;
        while(t < phase2DaggerBurstTime && fightActive && currentPhase == SolarisPhase.Phase2)
            if (flareTelegraph != null)
                flareTelegraph.TriggerFlare(playerTransform);

            yield return new WaitForSeconds(0.6f);

            if (daggers != null)
                daggers.TryFire(playerTransform);

            t += 0.6f;

        yield return new WaitForSeconds(Mathf.Max(0f, wallOfLightDuration - phase2DaggerBurstTime));
        DeactivateWallOfLight();

        yield return new WaitForSeconds(phase2CycleInterval);
    }
    void ActivateWallOfLight()
    {
        wallOfLightActive = true;

        if (wallOfLight != null)
            wallOfLight.SetActive(true);

        Debug.Log("Wall of Light Activated");

        Invoke(nameof(DeactivateWallOfLight), wallOfLightDuration);
    }

    void DeactivateWallOfLight()
    {
        wallOfLightActive = false;

        if (wallOfLight != null)
            wallOfLight.SetActive(false);

        Debug.Log("Wall of Light Deactivated");
        CancelInvoke(nameof(DeactivateWallOfLight));
    }

    // =================================
    // Damage API
    // =================================
    public void TakeDamage(float damageAmount)
    {
        if (solarisHealth == null)
            return;

        float finalDamage = damageAmount;

        if (wallOfLightActive)
            finalDamage *= damageReductionMultiplier;

        solarisHealth.TakeDamage(finalDamage);
    }

    /// =================================
    // End stats
    /// =================================

    void OnPlayerDefeated()
    {
        fightActive = false;

        StopAllCoroutines();

        Debug.Log("Solaris: You are not ready. Rety?");
    }
    
    private void OnSolarisDefeated()
    {
        fightActive = false;
        Debug.Log("Solaris: You have proven yourself worthy...");
    }

}


