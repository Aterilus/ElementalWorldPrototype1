using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class IceAbilityPack : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject telegraphPrefab;
    public GameObject spikePrefab;
    public GameObject frostNovaPrefab;
    public GameObject prisonPrefab;
    public GameObject blizzardPrefab;

    [Header("Move Windups")]
    public float spikesWindup = 0.9f;
    public float novaWindup = 0.6f;
    public float prisonWindup = 0.9f;

    [Header("Spike Pattern")]
    public int spikesCount = 6;
    public float spikesSpacing = 1.5f;

    [Header("Prison")]
    public float prisonFreezeSeconds = 2.5f;

    [Header("Blizzard")]
    public float blizzardDuration = 10f;

    public void CastIceSpikeLine(Transform caster, Transform target)
    {
        if (!spikePrefab || caster == null || target == null) { return; }
        StartCoroutine(CastSpikesLineAtTarget(caster, target, spikesWindup));
    }

    IEnumerator CastSpikesLineAtTarget(Transform caster, Transform target, float windup)
    {
        Vector3 dir = (target.position - caster.position).normalized;
        Vector3 start = target.position - dir * 2f;

        Vector3[] pts = new Vector3[spikesCount];
        for (int i = 0; i < spikesCount; ++i)
        {
            Vector3 p = start + dir * (i * spikesSpacing);
            p.y = caster.position.y;
            pts[i] = p;
        }

        foreach (var p in pts)
        {
            SpawnTelegraph(p, windup, 1.4f);
        }

        yield return new WaitForSeconds(windup);

        foreach (var p in pts)
        {
            Instantiate(spikePrefab, p, Quaternion.identity);
        }
    }

    public void CastFrostNova(Transform caster)
    {
        if (!frostNovaPrefab || caster == null) { return; }
        StartCoroutine(CastFrostNovaRoutine(caster, novaWindup));
    }

    IEnumerator CastFrostNovaRoutine(Transform caster, float windup)
    {
        if (!frostNovaPrefab) { yield break; }

        SpawnTelegraph(caster.position, windup, 3f);
        yield return new WaitForSeconds(windup);

        Instantiate(frostNovaPrefab, caster.position, Quaternion.identity);
    }

    public void CastPrison(Transform caster, Transform target)
    {
        if (!prisonPrefab || caster == null || target == null) { return; }
        StartCoroutine(CastPrisonRoutine(caster, target, prisonWindup, prisonFreezeSeconds));
    }

    IEnumerator CastPrisonRoutine(Transform caster, Transform target, float windup, float freezeSeconds)
    {
        Vector3 p = target.position;
        //p.y = transform.position.y;

        SpawnTelegraph(p, windup, 2.2f);
        yield return new WaitForSeconds(windup);

        var prison = Instantiate(prisonPrefab, p, Quaternion.identity);
        var fp = prison.GetComponent<FrozenPrison>();
        if (fp != null)
        {
            fp.player = target;
        }

        target.GetComponent<PlayerMovement>()?.Freeze(freezeSeconds);
    }

    public void CastBlizzard()
    {
        if (!blizzardPrefab) { return; }
        StartCoroutine(CastBlizzardRoutine());
    }

    IEnumerator CastBlizzardRoutine()
    {
        var go = Instantiate(blizzardPrefab, Vector3.zero, Quaternion.identity);

        var mgr = go.GetComponent<BlizzardManager>();
        if (mgr != null)
        {
            yield return new WaitForSeconds(2f);
        }
    }

    void SpawnTelegraph(Vector3 pos, float lifetime, float scale)
    {
        if (!telegraphPrefab) { return; }

        var t = Instantiate(telegraphPrefab, pos + Vector3.up * 0.02f, Quaternion.Euler(90, 0, 0));
        t.transform.localScale = new Vector3(scale, scale, scale);

        var tele = t.GetComponent<Telegraph>();
        if (tele != null)
        {
            tele.lifetime = lifetime;
        }
    }
}
