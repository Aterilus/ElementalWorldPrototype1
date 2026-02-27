using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class WindAbilityPack : MonoBehaviour
{
    [Header("Attack Prefabs")]
    public GameObject cyclonePrefab;
    public GameObject hurricanePrefab;

    [Header("Spawn Points")]
    public Transform cycloneSpawn;
    public Transform hurricaneSpawn;

    [Header("Blowback Settings")]
    public float blowbackForce = 15f;
    public float disorientDurration = 3f;
    public float disorientMoveMultiplier = 1f;
    public bool invertControls = true;
    public float blowbackRange = 6f;

    [Header("Hurricane Strikes Settings")]
    public int hurricaneDamage = 8;
    public float hurricaneSpeed = 10f;
    public int hurricaneCount = 5;

    public void CastCyclone(Transform caster, Transform target)
    {
        if (cyclonePrefab == null || caster == null || target == null) { return; }

        Transform spawn = cycloneSpawn != null ? cycloneSpawn : caster;

        Vector3 dir = (target.position - spawn.position);
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) { dir = spawn.forward; }
        dir.Normalize();

        Instantiate(cyclonePrefab, spawn.position, Quaternion.LookRotation(dir));
    }

    public void CastHurricaneStrike(Transform caster, Transform target)
    {
        if (hurricanePrefab == null || caster == null || target == null) { return; }

        StartCoroutine(HurricaneVollyRoutine(caster, target));
    }

    public IEnumerator HurricaneVollyRoutine(Transform caster, Transform target)
    {
        Transform spawn = hurricaneSpawn != null ? hurricaneSpawn : caster;

        Vector3 baseDir = (target.position - spawn.position);
        baseDir.y = 0f;
        if (baseDir.sqrMagnitude < 0.001f) { baseDir = spawn.forward; }
        baseDir.Normalize();

        float spacing = 2.8f;
        float delay = 0.50f;

        for (int i = 0; i < hurricaneCount; ++i)
        {
            float offset = (i - hurricaneCount / 2f) * spacing;

            Vector3 side = Vector3.Cross(Vector3.up, baseDir);
            Vector3 spawnPos = spawn.position + side * offset;

            GameObject obj = Instantiate(hurricanePrefab, spawnPos, Quaternion.LookRotation(baseDir));

            var proj = obj.GetComponent<HurricaneStrikeProjectile>();
            if (proj != null)
            {
                proj.Init(baseDir, hurricaneSpeed, hurricaneDamage, target);
            }

            yield return new WaitForSeconds(delay);
        }
    }

    public void CastBlowback(Transform caster)
    {
        if (caster == null) { return; }

        Collider[] hits = Physics.OverlapSphere(caster.position, blowbackRange);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Player")) { continue; }

            Rigidbody rb = hit.attachedRigidbody;
            Vector3 away = (hit.transform.position - caster.position);
            away.y = 0.25f;
            if (away.sqrMagnitude < 0.001f) { away = caster.forward; }
            away.Normalize();

            if (rb != null)
            {
                rb.AddForce(away * blowbackForce, ForceMode.VelocityChange);
            }

            var disorient = hit.GetComponentInParent<PlayerDisorientReceiver>();
            if (disorient != null)
            {
                disorient.ApplyDisorient(disorientDurration, disorientMoveMultiplier, invertControls);
            }
        }
    }
}
