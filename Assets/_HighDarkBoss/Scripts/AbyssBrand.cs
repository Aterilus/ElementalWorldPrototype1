using UnityEngine;
using System.Collections;

public class AbyssBrand : MonoBehaviour
{
    public GameObject telegraphPrefab;
    public GameObject explosionPrefab;

    public float telegraphTime = 1.0f;
    public float radius = 2.5f;
    public int damage = 20;

    Transform player;

    private void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) { player = p.transform; }

        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        if (player == null) { Destroy(gameObject); yield break; }

        Vector3 targetPos = player.position;
        targetPos.y = 0f;

        if (telegraphPrefab)
        {
            var t = Instantiate(telegraphPrefab, targetPos, Quaternion.identity);
            t.transform.localScale = new Vector3(radius, 1, radius);
            var tv = t.GetComponent<TelegraphVisuals>();
            if (tv) { tv.life = telegraphTime; }
        }

        yield return new WaitForSeconds(telegraphTime);

        if (explosionPrefab)
        {
            var e = Instantiate(explosionPrefab, targetPos, Quaternion.identity);
            e.transform.localScale = Vector3.one * radius;
            var dz = e.GetComponent<DamageZone>();
            if (dz)
            {
                dz.damage = damage;
                dz.life = 0.25f;
            }
        }

        Destroy(gameObject);
    }
}
