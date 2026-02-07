using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public bool IsDead => currentHealth <= 0f;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10f);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        //if (IsDead) return;
        Debug.Log($"[Health] TakeDamage called on {gameObject.name}. amount = {damageAmount} BEFORE = {currentHealth}");
        currentHealth -= damageAmount;

        Debug.Log($"[Health] AFTER = {currentHealth}");
        if (currentHealth < 0f)         
        {
            currentHealth = 0f;
            Debug.Log($"[Health] {gameObject.name} is now dead.");
        }
        //currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    public void Heal(float healAmount)
    {
        if (IsDead) return;
        currentHealth += healAmount;
        if (currentHealth > maxHealth)         
        {
            currentHealth = maxHealth;
        }
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }
}
