using UnityEngine;
using UnityEngine.SceneManagement;

public class BossFightExitController : MonoBehaviour 
{
    [Header("Scene Names")]
    [SerializeField] private string openWorldSceneName = "OpenWorld";

    [Header("References")]
    [SerializeField] private Health bossHealth;
    [SerializeField] private Health playerHealth;

    [Header("Return Gate")]
    [SerializeField] private GameObject returnGatePrefab;
    [SerializeField] private Transform gateSpawnPoint;

    [Header("Message (V1)")]
    [SerializeField] private string playerDeathMessage = "You aren't ready, train more to defeat thie boss.";

    private GameObject spawnGate;

    private void Awake()
    {
        if (bossHealth != null)
        {
            bossHealth = GameObject.FindWithTag("Enemy")?.GetComponentInParent<Health>();
        }

        if (playerHealth != null)
        {
            playerHealth = GameObject.FindWithTag("Player")?.GetComponentInParent<Health>();
        }
    }

    private void OnEnable()
    {
        if (bossHealth != null)
        {
            bossHealth.OnDied += OnBossDied;
        }

        if (playerHealth != null)
        {
            playerHealth.OnDied += OnPlayerDied;
        }
    }

    private void OnDisable()
    {
        if (bossHealth != null)
        {
            bossHealth.OnDied -= OnBossDied;
        }

        if (playerHealth != null)
        {
            playerHealth.OnDied -= OnPlayerDied;
        }
    }

    private void OnBossDied(Health health)
    {
        SpawnReturnGate();
    }

    private void OnPlayerDied(Health health)
    {
        Debug.Log($"[BossFightExitController] {playerDeathMessage}");
        SceneManager.LoadScene(openWorldSceneName);
    }

    private void SpawnReturnGate()
    {
        if(spawnGate != null) { return; }

        if (returnGatePrefab == null || gateSpawnPoint == null)
        {
            Debug.LogWarning("[BossFightExitController] Missing returnGatePefab or gateSpawnPoint.");
            return;
        }

        spawnGate = Instantiate(returnGatePrefab, gateSpawnPoint.position, gateSpawnPoint.rotation);
        Debug.Log("[BossFightExitController] Return gate spawned.");
    }
}
