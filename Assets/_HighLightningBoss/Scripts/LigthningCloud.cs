using UnityEngine;

public class LightningCloud : MonoBehaviour
{
    private LightningBossController boss;

    [Header("lifetime")]
    public float lifetmie = 3.0f;
    private float t;

    public void Init(LightningBossController owner)
    {
        boss = owner;
        t = 0f;
    }

    private void Update()
    {
        t += Time.deltaTime;
        if (t >= lifetmie)
        {
            Despawn();
        }
    }

    public void Despawn()
    {
        Destroy(gameObject);
    }
}
