using UnityEngine;
using System.Collections.Generic;

public class CalamityProgressTracker : MonoBehaviour
{
    private static HashSet<string> killed = new();

    public static void NotifyCalamityKilled(string id)
    {
        if (string.IsNullOrEmpty(id)) return;
        killed.Add(id);

        if (killed.Count >= 3)
        {
            StoryFlags.DarkBossUnlocked = true;
        }
    }

    public static int KilledCount => killed.Count;
}
