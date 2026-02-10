using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgress : MonoBehaviour
{
    public static PlayerProgress Instance { get; private set; }

    [Header("Progress")]
    public int evPoints = 0;

    [Header("Boss Flags (V1)")]
    public bool midBoss1Defeated = false;

    [Header("Story Flags")]
    [SerializeField] private bool hasSolarisBlessing = false;

    [Header("Totems")]
    [SerializeField] private int totemsCollected = 0;
    [SerializeField] private int totemsRequiredForDarkBoss = 7;

    [Header("Totems (Name-based)")]
    [SerializeField] private List<string> collectedTotemsDebug = new List<string>();
    private HashSet<string> collectedTotems = new HashSet<string>();
    public bool HasSolarisBlessing => hasSolarisBlessing;
    public int TotemsCollected => totemsCollected;
    public int TotemsRequiredForDarkBoss => totemsRequiredForDarkBoss;

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
        collectedTotems = new HashSet<string>(collectedTotemsDebug);
    }

    public void AddEvPoints(int points)
    {
        evPoints += points;
        Debug.Log($"[PlayerProgress] Added {points} EV points. Total now: {evPoints}");
    }

    public void GrantSolarisBlessing()
    {
        hasSolarisBlessing = true;
    }

    public void AddTotem(int totem = 1)
    {
        totemsCollected += totem;
        if (totemsCollected < 0)
        {
            totemsCollected = 0;
        }
    }

    public void AddTotem(string totemName)
    {
        if (string.IsNullOrWhiteSpace(totemName))
        {
            Debug.LogWarning("[PlayerProgress] Tried to add a totem with an empty name.");
            return;
        }

        bool added = collectedTotems.Add(totemName);

        if (added)
        {
            collectedTotemsDebug.Add(totemName);

            totemsCollected++;

            Debug.Log($"[PlayerProgress] Totem acquired: {totemName}. Total unique:{collectedTotems.Count}");
        }
        else
        {
            Debug.Log($"[PlayerProgress] Totemalready owned: {totemName}");
        }
    }

    public bool HasTotem(string totemName)
    {
        return collectedTotems.Contains(totemName);
    }
    public bool CanEnterDarkBoss()
    {
        return hasSolarisBlessing && totemsCollected >= totemsRequiredForDarkBoss;
    }

    public bool CanEnterHighBoss()
    {
        return hasSolarisBlessing;
    }
}
