using UnityEngine;

public class BlizzardManager : MonoBehaviour
{
    public float duration = 12f;
    public float tickInterval = 1.0f;
    public int frostStackPerTick = 1;

    float t, tick;

    Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        t += Time.deltaTime;
        tick += Time.deltaTime;

        if (tick >= tickInterval)
        {
            tick = 0f;
            if (player)
            {
                player.GetComponent<FrostbiteStatus>()?.AddStack();
            }
        }

        if (t >= duration)
        {
            Destroy(gameObject);
        }
    }
}
