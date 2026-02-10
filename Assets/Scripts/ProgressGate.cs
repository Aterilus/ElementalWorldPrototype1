using UnityEngine;
using UnityEngine.SceneManagement;

public class ProgressGate : MonoBehaviour
{
    public enum GateType
    {
        HighBoss,
        DarkBoss
    }

    [Header("Gate Setup")]
    [SerializeField] private GateType gateType = GateType.HighBoss;
    [SerializeField] private string targetSceneName;

    [Header("Optional")]
    [SerializeField] private string requiredTag = "Player";
    [SerializeField] private bool useTrigger = true;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("GATE HIT by: " + other.name);

        if(!useTrigger)
        {
            return;
        }
        if(!other.CompareTag(requiredTag))
        {
            return;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(targetSceneName);

        TryEnter();
    }

    public void TryEnter()
    {
        var progress = PlayerProgress.Instance;
        if (progress == null)
        {
            Debug.LogError("[ProgressGate] PlayerProgress not found. Add it to presistent object.");
            return;
        }

        if (gateType == GateType.HighBoss)
        {
            if (!progress.HasSolarisBlessing)
            {
                Debug.Log("Locked: You must prove yourself worthy to Solaris first.");
                return;
            }
        }
        else if (gateType == GateType.DarkBoss)
        {
            if (!progress.CanEnterDarkBoss())
            {
                Debug.Log($"Locked: Need Solaris blessing + {progress.TotemsRequiredForDarkBoss} Totems. Current: {progress.TotemsCollected}");
                return;
            }
        }

        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("[ProgressGate] Target Scene Name is empty.");
            return;
        }

        SceneManager.LoadScene(targetSceneName);
    }
}
