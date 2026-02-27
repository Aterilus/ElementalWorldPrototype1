using UnityEngine;
using System.Collections;

public class UmbralRush : MonoBehaviour
{
    public GameObject trailDamagePrefab;

    public float dashDistance = 10f;
    public float dashTime = 0.25f;

    public int trailCount = 5;
    public float trailRadius = 1.2f;
    public int trailDamage = 12;

    Transform player;
    Transform boss;

    private void Start()
    {
        boss = transform;
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) { player = p.transform; }

        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        if (player == null) { Destroy(gameObject); yield break; }

        Transform bossRoot = FindNearestBoss();
        if (bossRoot == null) { Destroy(gameObject); yield break;}

        Vector3 start = bossRoot.position;
        Vector3 dir = (player.position - bossRoot.position);
        dir.y = 0f;
        dir = dir.sqrMagnitude > 0.001f ? dir.normalized : bossRoot.forward;

        Vector3 end = start + dir * dashDistance;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / Mathf.Max(0.01f, dashTime);
            bossRoot.position = Vector3.Lerp(start, end, t);

            yield return null;
        }

        if (trailDamagePrefab)
        {
            for (int i = 0; i < trailCount; ++i)
            {
                float u = (trailCount == 1) ? 1f : (i / (float)(trailCount - 1));
                Vector3 pos = Vector3.Lerp(start, end, u);
                pos.y = 0f;

                var z = Instantiate(trailDamagePrefab, pos, Quaternion.identity);
                z.transform.localScale = Vector3.one * trailRadius;

                var dz = z.GetComponent<DamageZone>();
                if (dz)
                {
                    dz.damage = trailDamage;
                    dz.life = 0.2f;
                }
            }
        }

        Destroy(gameObject);
    }

    private Transform FindNearestBoss()
    {
        var b = GameObject.FindGameObjectWithTag("Enemy");
        return b ? b.transform : null;
    }
}
