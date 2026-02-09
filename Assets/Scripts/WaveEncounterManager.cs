using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveEncounterManager : MonoBehaviour
{
    [Header("Player")]
    public Health playerHealth;

    [Header("Lower Enemies")]
    public GameObject lowerEnemyPrefab;
    public Transform[] lowerEnemySpawnPoints;

    [Header("Mid Enemies")]
    public GameObject midEnemyPrefab;
    public Transform midEnemySpawnPoints;

    [Header("Goals")]
    public int lowerEnemiesToDefeat = 15;
    public int minAliveAtOnce = 2;
    public int maxAliveAtOnce = 4;

    [Header("Timing")]
    public float spawnGap = 0.25f;
    public float intermissionSeconds = 5f;

    [Header("Intermission Heal")]
    public float healPerSecond = 10f;

    private int lowEnemiesDefeated = 0;
    private readonly List<Health> aliveLowEnimies = new List<Health>();
    private bool encounterRunning;

    private void Start()
    {
        encounterRunning = true;
        StartCoroutine(EncounterLoop());
    }

    private IEnumerator EncounterLoop()
    {
        while (encounterRunning && lowEnemiesDefeated < lowerEnemiesToDefeat)
        {
            aliveLowEnimies.RemoveAll(e => e == null || e.IsDead);

            int desiredAlive = Random.Range(minAliveAtOnce, maxAliveAtOnce + 1);

            int remainingToDefeat = lowerEnemiesToDefeat - lowEnemiesDefeated;
            desiredAlive = Mathf.Min(desiredAlive, remainingToDefeat);

            desiredAlive = Mathf.Min(desiredAlive, maxAliveAtOnce);

            while (encounterRunning && lowEnemiesDefeated < lowerEnemiesToDefeat && aliveLowEnimies.Count < desiredAlive)
            {
                SpawnLowerEnemy();
                yield return new WaitForSeconds(spawnGap);

                if (aliveLowEnimies.Count >= maxAliveAtOnce)
                {
                    break;
                }
            }

            yield return null;
        }

        yield return StartCoroutine(IntermissionHeal());

        SpawnMidBoss();
    }

    private void SpawnLowerEnemy()
    {
        if (lowerEnemyPrefab == null || lowerEnemySpawnPoints == null || lowerEnemySpawnPoints.Length == 0) { return; }

        Transform spawnPoint = lowerEnemySpawnPoints[Random.Range(0, lowerEnemySpawnPoints.Length)];
        GameObject enemyGO = Instantiate(lowerEnemyPrefab, spawnPoint.position, spawnPoint.rotation);

        Health enemyHealth = enemyGO.GetComponentInChildren<Health>();

        if (enemyHealth == null)
        {
            Debug.LogError("Lower enemy prefab is missing a Health component.");
            return;
        }

        aliveLowEnimies.Add(enemyHealth);
        enemyHealth.OnDied += OnLowerEnemyDied;
    }

    private void OnLowerEnemyDied(Health enemyHealth)
    {
        enemyHealth.OnDied -= OnLowerEnemyDied;

        if (aliveLowEnimies.Contains(enemyHealth))
        {
            aliveLowEnimies.Remove(enemyHealth);
        }
        
        lowEnemiesDefeated++;
        
        Destroy(enemyHealth.gameObject);

        Debug.Log($"Lower enemy defeated! Total defeated: {lowEnemiesDefeated}/{lowerEnemiesToDefeat}");
    }

    private IEnumerator IntermissionHeal()
    {
        Debug.Log("Intermission started. Healing player...");

        float elapsed = 0f;
        while (elapsed < intermissionSeconds && !playerHealth.IsDead)
        {
            if (playerHealth != null)
            {
                playerHealth.Heal(healPerSecond * Time.deltaTime);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Intermission ended.");
    }

    void SpawnMidBoss()
    {
        Debug.Log("Spawning mid boss...");

        if(midEnemyPrefab == null || midEnemySpawnPoints == null)
        {
            Debug.LogError("Mid boss prefab or spawn points not set.");
            return;
        }

        Instantiate(midEnemyPrefab, midEnemySpawnPoints.position, midEnemySpawnPoints.rotation);
    }

    public void OnMidBossDeath()
    {
        Debug.Log("Mid boss has been defeated! Encounter complete.");

        if (PlayerProgress.Instance != null)
        {
            Debug.Log($"Victory! Total EV earned: {PlayerProgress.Instance.evPoints}");
            PlayerProgress.Instance.midBoss1Defeated = true;
        }

        encounterRunning = false;

        SceneManager.LoadScene("OpenWorld");
    }
}
