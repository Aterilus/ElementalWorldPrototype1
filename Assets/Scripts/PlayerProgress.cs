using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    public static PlayerProgress Instance { get; private set; }

    [Header("Progress")]
    public int evPoints = 0;

    [Header("Boss Flags (V1)")]
    public bool midBoss1Defeated = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[PlayerProgress] Multiple instances detected! Destroying duplicate on {gameObject.name}.");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddEvPoints(int points)
    {
        evPoints += points;
        Debug.Log($"[PlayerProgress] Added {points} EV points. Total now: {evPoints}");
    }
}
