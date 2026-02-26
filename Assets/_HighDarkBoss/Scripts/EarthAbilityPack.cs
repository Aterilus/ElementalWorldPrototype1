using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class EarthAbilityPack : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject warningPrefab;
    public GameObject shockwavePrefab;
    public GameObject pillarPrisonPrefab;
    public GameObject boulderPrefab;

    [Header("Spawn Points")]
    public Transform boulderSpawnPoint;

    [Header("Telegraph")]
    public float slamTelegraph = 1.0f;
    public float prisonTelegraph = 1.0f;
    public float boulderTelegraph = 0.6f;

    [Header("Boulder Throw")]
    public float boulderForce = 18f;
    public float boulderUpward = 4f;

    [Header("Ground Snap")]
    public bool snapSpawnsToGround = true;
    public float rayCastHeight = 30f;
    public LayerMask groundMask = 0;

    public void CastSeismicSlam(Transform caster)
    {
        if (caster == null) { return; }
        StartCoroutine(SeismicSlam(caster));
    }

    public IEnumerator SeismicSlam(Transform caster)
    {
        Vector3 pos = caster.position;
        pos = snapSpawnsToGround ? SnapToGround(pos) : pos;

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

    public void CastPillarPrison(Transform target)
    {
        if (target == null) { return; }
        StartCoroutine(PillarPrison(target));
    }

    public IEnumerator PillarPrison(Transform target)
    {
        Vector3 center = target.position;
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

    public void CastBoulder(Transform caster, Transform target)
    {
        if (caster == null || target == null) { return; }
        StartCoroutine(BoulderToss(caster, target));
    }

    public IEnumerator BoulderToss(Transform caster, Transform target)
    {
        Transform spawn = boulderSpawnPoint != null ? boulderSpawnPoint : caster;

        if (spawn == null || boulderPrefab == null)
        {
            yield break;
        }

        Vector3 warnPos = target.position;
        warnPos = snapSpawnsToGround ? SnapToGround(warnPos) : warnPos;

        if (warningPrefab != null)
        {
            Instantiate(warningPrefab, warnPos, Quaternion.identity);
        }

        yield return new WaitForSeconds(boulderTelegraph);

        GameObject boulder = Instantiate(boulderPrefab, spawn.position, Quaternion.identity);

        Rigidbody body = boulder.GetComponent<Rigidbody>();
        if (body != null)
        {
            Vector3 dir = (target.position - spawn.position);
            dir.y = 0f;
            dir = dir.sqrMagnitude > 0.001f ? spawn.forward : dir.normalized;

            Vector3 force = (dir * boulderForce) + (Vector3.up * boulderUpward);
            body.AddForce(force, ForceMode.VelocityChange);
        }
    }

    public Vector3 SnapToGround(Vector3 pos)
    {
        Ray ray = new Ray(pos + Vector3.up * rayCastHeight, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, rayCastHeight * 2f, groundMask, QueryTriggerInteraction.Ignore))
        {
            pos.y = hit.point.y;
        }
        return pos;
    }
}
