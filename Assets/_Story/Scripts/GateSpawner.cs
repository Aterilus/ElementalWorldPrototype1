using UnityEditor.Search;
using UnityEngine;

public class GateSpawner : MonoBehaviour
{
    public enum TriggerType { AfterSeconds, WhenFlagTrue }

    [Header("Gate")]
    [SerializeField] private GameObject gatePrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Trigger")]
    [SerializeField] private TriggerType triggerType = TriggerType.AfterSeconds;
    [SerializeField] private float secondsDelay = 60f;

    [Header("Flag Trigger")]
    [SerializeField] private string flagName = "FirstCalamityDefeated";

    private bool spawned;
    private float startTime;

    private void Start()
    {
        startTime = Time.time;
    }

    private void Update()
    {
        if (spawned) { return; }

        bool shouldSpawn = triggerType switch
        {
            TriggerType.AfterSeconds => Time.time >= startTime + secondsDelay,
            TriggerType.WhenFlagTrue => GetFlag(flagName),
            _ => false
        };

        if (shouldSpawn) { SpawnGate(); }
    }

    private bool GetFlag(string name)
    {
        return name switch
        {
            "EnteredOpenWorld" => StoryFlags.EnteredOpenWorld,
            "FirstCalamitySeen" => StoryFlags.FirstCalamitySeen,
            "FirstCalamityDefeated" => StoryFlags.FirstCalamityDefeated,
            "DarkBossUnlocked" => StoryFlags.DarkBossUnlocked,
            _ => false
        };
    }

    private void SpawnGate()
    {
        spawned = true;
        if ( !gatePrefab || !spawnPoint) { return; }

        Instantiate(gatePrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
