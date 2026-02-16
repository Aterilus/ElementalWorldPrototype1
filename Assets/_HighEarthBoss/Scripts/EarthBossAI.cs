using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;

public class EarthBossAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Prefabs")]
    [SerializeField] private GameObject warningPrefab;
    [SerializeField] private GameObject shockwavePrefab;
    [SerializeField] private GameObject pillarPrisonPrefab;
    [SerializeField] private GameObject boulderPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform boulderSpawnPoint;

    [Header("Cooldowns")]
    [SerializeField] private float slamCooldown = 4f;
    [SerializeField] private float prisonCooldown = 7f;
    [SerializeField] private float boukderCooldown = 3f;

    [Header("Telegraph")]
    [SerializeField] private float slamTelegraph = 1.0f;
    [SerializeField] private float prisonTelegraph = 1.0f;
    [SerializeField] private float boulderTelegraph = 0.6f;

    [Header("Boulder Throw")]
    [SerializeField] private float boulderForce = 18f;
    [SerializeField] private float boulderUpward = 4f;

    [Header("Ground Snap")]
    [SerializeField] private bool snapSpawnsToGround = true;
    [SerializeField] private float rayCastHeight = 30f;
    [SerializeField] private LayerMask groundMask = 0;

    private float nextSlam;
    private float nextPrison;
    private float nextBoulder;

    private void Start()
    {
        float time = Time.time + 1f;
        nextSlam = time;
        nextPrison = time + 1f;
        nextBoulder = time + 0.5f;

        StartCoroutine(AttackLoop());
    }

    private IEnumerator AttackLoop()
    {
        while (true)
        {
            if (player == null)
            {
                yield return null;
                continue;
            }

            if (Time.time >= nextBoulder)
            {
                yield return BoulderToss();
                nextBoulder = Time.time + boukderCooldown;
            }
            else if (Time.time >= nextPrison)
            {
                yield return PillarPrison();
                nextPrison = Time.time + prisonCooldown;
            }
            else if (Time.time >= nextSlam)
            {
                yield return SeismicSlam();
                nextSlam = Time.time + slamCooldown;
            }

            yield return null;
        }
    }

    private IEnumerator SeismicSlam()
    {
        Vector3 pos = transform.position;
        pos = snapSpawnsToGround? SnapToGround(pos) : pos;

        if (warningPrefab != null)
        {
            Instantiate(warningPrefab, pos, Quaternion.identity);
        }

        yield return new WaitForSeconds(slamTelegraph);

        if (shockwavePrefab != null)
        {
            Instantiate(shockwavePrefab, pos, Quaternion.identity);
        }
    }

    private IEnumerator PillarPrison()
    {
        Vector3 center = player.position;
        center = snapSpawnsToGround ? SnapToGround(center) : center;

        if (warningPrefab != null)
        {
            Instantiate(warningPrefab, center, Quaternion.identity);
        }

        yield return new WaitForSeconds(prisonTelegraph);

        if (pillarPrisonPrefab != null)
        {
            GameObject obj = Instantiate(pillarPrisonPrefab, center, Quaternion.identity);
            var spawner = obj.GetComponent<PillarPrisonSpawner>();
            if (spawner != null)
            {
                spawner.SpawnAt(center);
            }
        }
    }

    private IEnumerator BoulderToss()
    {
        if (boulderSpawnPoint != null || boulderPrefab == null)
        {
            yield break;
        }

        Vector3 warnPos = player.position;
        warnPos = snapSpawnsToGround ? SnapToGround(warnPos) : warnPos;

        if (warningPrefab != null)
        {
            Instantiate(warningPrefab, warnPos, Quaternion.identity);
        }

        yield return new WaitForSeconds(boulderTelegraph);

        GameObject boulder = Instantiate(boulderPrefab, boulderSpawnPoint.position, Quaternion.identity);

        Rigidbody body = boulder.GetComponent<Rigidbody>();
        if (body != null)
        {
            Vector3 dir = (player.position - boulderSpawnPoint.position);
            dir.y = 0f;
            dir = dir.normalized;

            Vector3 force = (dir * boulderForce) + (Vector3.up * boulderUpward);
            body.AddForce(force, ForceMode.VelocityChange);
        }
    }

    private Vector3 SnapToGround(Vector3 pos)
    {
        Ray ray = new Ray(pos + Vector3.up * rayCastHeight, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, rayCastHeight * 2f, groundMask, QueryTriggerInteraction.Ignore))
        {
            pos.y = hit.point.y;
        }
        return pos;
    }
}
