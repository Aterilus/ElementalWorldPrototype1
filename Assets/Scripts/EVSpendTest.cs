using UnityEngine;

public class EVSpendTest : MonoBehaviour
{
    public Stats stats;

    private void Awake()
    {
        if (!stats) { stats = GetComponent<Stats>(); }
    }

    private void Update()
    {
        if (!stats) {  return; }

        if (Input.GetKeyDown(KeyCode.P)) { stats.evPoints += 5; }

        if (Input.GetKeyDown(KeyCode.Alpha1)) { TrySpend(ref stats.evHP); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { TrySpend(ref stats.evAttack); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { TrySpend(ref stats.evDefense); }
        if (Input.GetKeyDown(KeyCode.Alpha4)) { TrySpend(ref stats.evSpeed); }
    }

    private void TrySpend(ref int bucket)
    {
        if (stats.evPoints <= 0) { return; }
        stats.evPoints--;
        bucket++;
        stats.Recalculate();
    }
}
