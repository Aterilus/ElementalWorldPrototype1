using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class PlayerPrisonTarget : MonoBehaviour
{
    [Header("Player Movement script")]
    public MonoBehaviour movementScriptToDisable;

    [Header("Avoidable Prison")]
    public float dodgeWindiwSeconds = 0.25f;
    public float dodgeSpeedThreshold = 7.5f;
    public Rigidbody playerRigidbody;

    private bool recentlyDodged;

    [System.Obsolete]
    public bool TryDodgePrison()
    {
        float speed = 0f;

        if (playerRigidbody != null) { speed = playerRigidbody.velocity.magnitude; }

        bool dodged = speed >= dodgeSpeedThreshold;
        if (dodged)
        {
            if (!recentlyDodged) { StartCoroutine(DodgeLock()); }
            return true;
        }
        return false;
    }

    public void ApplyPrison(float duration, int damageChunk)
    {
        StartCoroutine(PrisonRoutine(duration, damageChunk));
    }

    private IEnumerator PrisonRoutine(float duration, int damageChunk)
    {
        Health playerHealth = GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageChunk);
        }

        if (movementScriptToDisable != null)
        {
            movementScriptToDisable.enabled = false;
        }

        yield return new WaitForSeconds(duration);

        if (movementScriptToDisable != null)
        {
            movementScriptToDisable.enabled = true;
        }
    }

    private IEnumerator DodgeLock()
    {
        recentlyDodged = true;
        yield return new WaitForSeconds(dodgeWindiwSeconds);
        recentlyDodged = false;
    }
}
