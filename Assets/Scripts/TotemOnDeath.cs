using UnityEngine;

public class TotemOnDeath : MonoBehaviour
{
    [SerializeField] private string totemName;
    [SerializeField] private Health health;

    private void Reset()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.OnDied += HandleDied;
        }
    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.OnDied -= HandleDied;
        }
    }

    private void HandleDied(Health deadHealth)
    {
        if (PlayerProgress.Instance == null)
        {
            Debug.LogError("[TotemOnDeath] PlayerProgress not found in scene.");
            return;
        }

        PlayerProgress.Instance.AddTotem(totemName);
    }
}
