using UnityEngine;
using System.Collections.Generic;
public class BossArenaTracker : MonoBehaviour
{
    public BossArenaFlow arenaFlow;
    public Health darkBossHealth;

    private readonly List<Health> activeClonesHealth = new List<Health>();
    private bool victoryTriggered;

    private void Update()
    {
        activeClonesHealth.RemoveAll(c => c == null || (c != null && c.IsDead));

        bool bossDead = darkBossHealth != null && darkBossHealth.IsDead;
        bool noClones = activeClonesHealth.Count == 0;

        if (!victoryTriggered && bossDead && noClones)
        {
            victoryTriggered = true;
            arenaFlow?.OnBossDefeated();
        }
    }

    public void RegisterClone(Health cloneHelath)
    {
        if (cloneHelath != null && !activeClonesHealth.Contains(cloneHelath))
        {
            activeClonesHealth.Add(cloneHelath);
        }
    }

    public int ActivteCloneCount() => activeClonesHealth.Count;
}
