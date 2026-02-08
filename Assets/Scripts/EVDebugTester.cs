using UnityEngine;

public class EVDebugTester : MonoBehaviour
{
    public int strength;
    public int vitality;

    private void Update()
    {
        if (PlayerProgress.Instance == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            if (PlayerProgress.Instance.evPoints > 0)
            {
                PlayerProgress.Instance.evPoints--;
                strength++;
                Debug.Log($"Increased strength to {strength}. Remaining EV points: {PlayerProgress.Instance.evPoints}");
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (PlayerProgress.Instance.evPoints > 0)
            {
                PlayerProgress.Instance.evPoints--;
                vitality++;
                Debug.Log($"Increased vitality to {vitality}. Remaining EV points: {PlayerProgress.Instance.evPoints}");
            }
        }
    }

}
