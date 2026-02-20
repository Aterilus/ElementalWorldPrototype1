using System;
using System.Collections.Generic;
using System.Diagnostics;

public static class CalamityKillTracker
{
    public static event Action OnAllCalamitiesDefeated;

    public static readonly HashSet<string> killedIds = new();

    public static void RegisterKill(string calamityId)
    {
        if(string.IsNullOrEmpty(calamityId)) { return; }

        bool added = killedIds.Add(calamityId);
        if (!added)
        {
            return;
        }

        if (killedIds.Count >= 3)
        {
            OnAllCalamitiesDefeated?.Invoke();
            StoryFlags.DarkBossUnlocked = true;
        }
    }

    public static int Count => killedIds.Count;

    public static void Reset() => killedIds.Clear();
}
