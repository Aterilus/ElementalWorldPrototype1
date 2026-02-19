using UnityEngine;

public class CalamityDeathReporter : MonoBehaviour
{
    public string calamityId;

    public void ReportDeath()
    {
        CalamityProgressTracker.NotifyCalamityKilled(calamityId);
    }
}
