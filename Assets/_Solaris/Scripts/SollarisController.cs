using UnityEngine;

public class SollarisController : MonoBehaviour
{
    [Header("Phase 1 Timing")]
    public float attackDelay = 1.5f;

    private bool isAttacking;

    [Header("References")]
    public Transform playerTransform;
    public Transform[] teleportPoints;

    [ Header("Timing")]
    public float teleportInterval = 3f;

    private float teleportTimer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("TEST: Y pressed - trying to fire dagger");

            var daggers = GetComponent<SolarisDaggers>();
            if (daggers == null)
            {
                Debug.LogError("SolarisDaggers NOT found on this object");
            }
            else
            {
                daggers.TryFire(playerTransform);
            }
        }
        if (playerTransform == null || teleportPoints.Length == 0)
            return;

        //if (!isAttacking)
        //{
        //    return;
        //}
        teleportTimer += Time.deltaTime;

        if (teleportTimer >= teleportInterval)
        {
            StartCoroutine(Phase1Routine());
            teleportTimer = 0f;
        }

        FacePlayer();
    }

    System.Collections.IEnumerator Phase1Routine()
    {
        isAttacking = true;

        TeleportToRandomPoint();

        yield return new WaitForSeconds(attackDelay);

        SolarisFlareTelegraph flareTelegraph = GetComponent<SolarisFlareTelegraph>();
        if (flareTelegraph != null)
        {
            flareTelegraph.TriggerFlare(playerTransform);
        }

        yield return new WaitForSeconds(1.5f);

        isAttacking = false;

        SolarisDaggers daggers = GetComponent<SolarisDaggers>();
        if (daggers != null)
        {
            daggers.TryFire(playerTransform);
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
}
