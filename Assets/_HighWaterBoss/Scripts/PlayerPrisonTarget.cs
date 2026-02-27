using UnityEngine;
using System.Collections;

public class PlayerPrisonTarget : MonoBehaviour
{
    [Header("Player Movement script")]
    public MonoBehaviour[] scriptToDisable;

    [Header("Prison Logic")]
    public Rigidbody playerRigidbody;

    public bool prisonActive;
    private RigidbodyConstraints savedconstraints;
    private bool savedKinematic;

    [System.Obsolete]
    public void ApplyPrison(float duration, int damageChunk)
    {
        if (!prisonActive)
        {
            StartCoroutine(PrisonRoutine(duration, damageChunk));
        }
    }

    [System.Obsolete]
    private IEnumerator PrisonRoutine(float duration, int damageChunk)
    {
        prisonActive = true;

        Health playerHealth = GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damageChunk);
        }

        if (scriptToDisable != null)
        {
            foreach (var script in scriptToDisable)
            {
                if (script)
                {
                    script.enabled = false;
                }
            }
        }

        if (playerRigidbody != null)
        {
            savedconstraints = playerRigidbody.constraints;
            savedKinematic = playerRigidbody.isKinematic;

            playerRigidbody.velocity = Vector3.zero;
            playerRigidbody.angularVelocity = Vector3.zero;

            playerRigidbody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        }

        yield return new WaitForSeconds(duration);

        if (playerRigidbody !=null)
        {
            playerRigidbody.constraints = savedconstraints;
            playerRigidbody.isKinematic = savedKinematic;
        }

        if (scriptToDisable != null)
        {
            foreach(var script in scriptToDisable)
            {
                if ( script)
                {
                    script.enabled = true;
                }
            }
        }

        prisonActive = false;
    }
}
