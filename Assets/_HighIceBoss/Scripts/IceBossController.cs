using System.Collections;
using UnityEngine;

public class IceBossController : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Prefabs")]
    public GameObject telegraphPrefab;
    public GameObject spikePrefab;
    public GameObject frostNovaPrefab;
    public GameObject prisonPrefab;
    public GameObject blizzardPrefab;

    [Header("Cooldown Test Mode")]
    public bool testMode = true;
    public float globalDelayBetweenMoves = 0.6f;

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

    int moveIndex;

    void Start()
    {
        if (player == null)
        {
            var plyr = GameObject.FindGameObjectWithTag("Player");
            if (plyr)
            {
                player = player.transform;
            }
        }

        StartCoroutine(TestLoop());
    }

    IEnumerator TestLoop()
    {
            yield return new WaitForSeconds(1f);

            while (true)
            {
                if (player == null)
                {
                    yield return new WaitForSeconds(0.5f);
                    continue;
                }

                switch (moveIndex)
                {
                    case 0:
                        yield return CastSpikesLineAtPlayer(spikesWindup);
                        break;

                    case 1:
                        yield return CastFrostNova(novaWindup);
                        break;

                    //case 2:
                    //    yield return CastPrison(prisonWindup, prisonFreezeSeconds);
                    //    break;
                
                    case 3:
                        yield return CastBlizzard();
                        break;
                }

                moveIndex = (moveIndex + 1) % 4;
                yield return new WaitForSeconds(globalDelayBetweenMoves);
            }
    }

   

    IEnumerator CastSpikesLineAtPlayer(float windup)
    {
        if (!spikePrefab) { yield break; }
        Vector3 dir = (player.position - transform.position).normalized;
        Vector3 start = player.position - dir * 2f;

        Vector3[] pts = new Vector3[spikesCount];
        for (int i = 0; i < spikesCount; ++i)
        {
            Vector3 p = start + dir * (i * spikesSpacing);
            p.y = transform.position.y;
            pts[i] = p;
        }

        foreach(var p in pts)
        {
            SpawnTelegraph(p, windup, 1.4f);
        }

        yield return new WaitForSeconds(windup);

        foreach (var p in pts)
        {
            Instantiate(spikePrefab, p, Quaternion.identity);
        }
    }

    IEnumerator CastFrostNova(float windup)
    {
        if (!frostNovaPrefab) { yield break; }

        SpawnTelegraph(transform.position, windup, 3f);
        yield return new WaitForSeconds(windup);

        Instantiate(frostNovaPrefab,transform.position, Quaternion.identity);
    }
    //IEnumerator CastPrison(float windup, float freezeSeconds)
    //{
    //    if (!frostNovaPrefab)
    //    { 
    //        yield break;
    //    }

    //    Vector3 p = player.position;
    //    //p.y = transform.position.y;

    //    SpawnTelegraph(p, windup, 2.2f);
    //    yield return new WaitForSeconds(windup);

    //    var prison = Instantiate(prisonPrefab, p, Quaternion.identity);
    //    var fp = prison.GetComponent<FrozenPrison>();
    //    if (fp != null)
    //    {
    //        fp.player = player;
    //    }

    //    player.GetComponent<PlayerMovement>()?.Freeze(freezeSeconds);
    //}

    IEnumerator CastBlizzard()
    {
        if (!blizzardPrefab) { yield break; }

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
