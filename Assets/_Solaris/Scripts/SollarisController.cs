using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SollarisController : MonoBehaviour
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
    private bool isAttacking = false;
    private Coroutine attackRoutine;

    // =================================
    // Wall of Light
    // =================================
    [Header("Wall of Light")]
    public bool wallOfLightActive = false;
    public float wallOfLightDuration = 3f;
    public float damageReductionMultiplier = 0.5f;

    // =================================
    // Debugging
    // =================================
    [Header("Debugging")]
    public bool debugManualDamage = true;

    // =================================
    // Unity Methods
    // =================================
    private void Awake()
    {
        daggers = GetComponent<SolarisDaggers>();
        flareTelegraph = GetComponent<SolarisFlareTelegraph>();

        if (solarisHealth == null)
        {
            solarisHealth = GetComponent<Scene2Health>();
        }
    }

    void Update()
    {
        // Safety checks
        if (playerTransform == null) return;
        if (teleportPoints == null || teleportPoints.Length == 0) return;

        // Always on behavior
        HandleCommon();

        // Phase transitions
        CheckPhaseTransitions();

        // Manual damage for testing
        if (debugManualDamage && Input.GetKeyDown(KeyCode.Y))
        {
            TakeDamage(10f);
            Debug.Log("TEST: Y pressed - trying to fire dagger");
        }

        TickPhase(); 
    }

    // =================================
    // Common
    // =================================
    void HandleCommon()
    {
        FacePlayer();
        TickTeleport();
    }
 
    // =================================
    // Phase Tick
    // =================================
    private void TickPhase()
    {
        switch (currentPhase)
        {
            case SolarisPhase.Phase1:
                HandlePhase1();
                break;
            case SolarisPhase.Phase2:
                HandlePhase2();
                break;
            case SolarisPhase.Phase3:
                HandlePhase3();
                break;
        }
    }
    public void HandlePhase1()
    {
        TryStartPhase1Attack();
    }
    public void HandlePhase2()
    {

    }
    public void HandlePhase3()
    {

    }

    // =================================
    // Transitions
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
        
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
        isAttacking = false;
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

    // =================================
    // Teleportation / Movement
    // =================================
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
    void FacePlayer()
    {
        Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0f; // Keep only horizontal direction

        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    // =================================
    // Attacks
    // =================================
    void TryStartPhase1Attack()
    {
        if (isAttacking)
            return;
        attackRoutine = StartCoroutine(Phase1Routine());
    }

    private IEnumerator Phase1Routine()
    {
        isAttacking = true;

        TeleportToRandomPoint();

        yield return new WaitForSeconds(attackDelay);


        if (flareTelegraph != null)
        {
            flareTelegraph.TriggerFlare(playerTransform);
        }

        yield return new WaitForSeconds(1.5f);


        if (daggers != null)
        {
            daggers.TryFire(playerTransform);
        }

        isAttacking = false;
        attackRoutine = null;
    }

    // =================================
    // Wall of Light
    // =================================
    void ActivateWallOfLight()
    {
        if(wallOfLightActive)
            return;
        wallOfLightActive = true;
        Debug.Log("Wall of Light Activated");

        Invoke(nameof(DeactivateWallOfLight), wallOfLightDuration);
    }

    void DeactivateWallOfLight()
    {
        wallOfLightActive = false;
        Debug.Log("Wall of Light Deactivated");
    }

    

    

   
}


