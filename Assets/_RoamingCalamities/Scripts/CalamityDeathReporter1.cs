using UnityEngine;

public class CalamityDeathReporter1 : MonoBehaviour
{
    public string calamityId;

    public void ReportDeath()
    {
        CalamityKillTracker.RegisterKill(calamityId);
    }
}
