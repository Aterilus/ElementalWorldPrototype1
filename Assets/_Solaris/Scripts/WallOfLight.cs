using UnityEngine;
using System.Collections;

public class WallOfLight : MonoBehaviour
{
    [Header("Refs")]
    public Scene2Health solarisHealth;
    public Collider lightTrigger;
    public GameObject visualRoot;

    [Header("Behavior")]
    public float duration = 3f;
    public float healPerSecond = 20f;
    public float stunDuration = 1.25f;

    bool playerInLight;

    public bool IsActive { get; private set; }
    public bool PlayerCaughtThisCast { get; private set; }

    private void Awake()
    {
        if (lightTrigger != null) lightTrigger.isTrigger = true;
        SetActive(false);
    }

    public void Activate()
    {
        if (IsActive) return;
        StartCoroutine(ActivateRoutine());
    }

    IEnumerator ActivateRoutine()
    {
        IsActive = true;
        PlayerCaughtThisCast = false;
        playerInLight = false;

        SetActive(true);

        float t = 0f;
        while (t < duration)
        {
            if (solarisHealth != null)
            {
                solarisHealth.currentHealth = Mathf.Min(solarisHealth.maxHealth, solarisHealth.currentHealth + healPerSecond * Time.deltaTime);
            }

            if (playerInLight && !PlayerCaughtThisCast)
            {
                PlayerCaughtThisCast = true;
            }

            yield return null;
        }

        SetActive(false);
        IsActive = false;
    }

    public void SetActive(bool on)
    {
        if (visualRoot != null)
        {
            visualRoot.SetActive(on);
        }
        if (lightTrigger != null)
        {
            lightTrigger.enabled = on;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsActive) return;
        if (other.CompareTag("Player"))
        {
            return;
        }

        playerInLight = true;

        var statusEffects = other.GetComponentInParent<PlayerStatusEffects>();
        if (statusEffects != null)
        {
            statusEffects.ApplyStun(stunDuration);
            PlayerCaughtThisCast = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            return;
        }
        playerInLight = false;
    }
}
