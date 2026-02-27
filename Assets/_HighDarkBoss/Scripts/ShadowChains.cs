using UnityEngine;
using System.Collections;

public class ShadowChains : MonoBehaviour
{
    public GameObject telegraphPrefab;

    public float telegraphTime = 0.8f;
    public float radius = 2.0f;

    public float slowMultiplier = 0.35f;
    public float slowDuration = 1.5f;

    Transform player;

    private void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) { player = p.transform; }

        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        if (player == null) { Destroy(player);  yield break; }

        Vector3 pos = player.position;
        pos.y = 0f;

        if (telegraphPrefab)
        {
            var t = Instantiate(telegraphPrefab, pos, Quaternion.identity);
            t.transform.localScale = new Vector3(radius, 1, radius);
            var tv = t.GetComponent<TelegraphVisuals>();
            if (tv) { tv.life = telegraphTime; }
        }

        yield return new WaitForSeconds(telegraphTime);

        float dist = Vector3.Distance(new Vector3(player.position.x, 0f, player.position.z), pos);
        if (dist <= radius * 0.5f)
        {
            var status = player.GetComponentInParent<PlayerStatusEffects>();
            if (status != null)
            {
                status.ApplySlow(slowMultiplier, slowDuration);
            }
        }

        Destroy(gameObject);
    }
}
