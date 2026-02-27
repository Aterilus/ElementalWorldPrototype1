using UnityEngine;

public class CloneDeathNotifier : MonoBehaviour
{
    [HideInInspector] public DarkBossAI boss;
    [HideInInspector] public string elementId;

    Health health;
    bool sent;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void Update()
    {
        if (!sent && health != null && health.IsDead)
        {
            sent = true;
            boss?.OnCloneDefeated(elementId);
        }
    }
}
