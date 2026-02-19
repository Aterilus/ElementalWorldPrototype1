using UnityEngine;

public class PlayerReadiness : MonoBehaviour
{
    [Min(1)] public int readinessLevel = 1;
    public Stats stats;

    private void Awake()
    {
        if (!stats) { stats = GetComponent<Stats>(); }
    }

    private void Update()
    {
        if (!stats) { return; }

        readinessLevel = 1
            + (stats.evHP / 5)
            + (stats.evAttack / 5)
            + (stats.evDefense / 5)
            + (stats.evSpeed / 5);
    }
}
