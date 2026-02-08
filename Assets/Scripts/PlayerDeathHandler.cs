using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathHandler : MonoBehaviour
{
    public Health playerHealth;
    public float reloadDelay = 1.0f;
    private bool dead;

    private void Awake()
    {
        if (playerHealth == null)
        {
            playerHealth = GetComponentInParent<Health>();
        }
    }

    private void Update()
    {
        if (dead) { return; }
        if (playerHealth != null && playerHealth.IsDead)
        {
            dead = true;
            Debug.Log("Player is dead. Reloading scene...");
            Invoke(nameof(ReloadScene), reloadDelay);
        }
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

