using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public bool IsDead => currentHealth <= 0f;

    //public event Action<int> OnDamaged;
    public event Action<Health> OnDied;

    private bool died;

    private void Awake()
    {
        if (currentHealth <= 0f) { currentHealth = maxHealth; }
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
        if (died) { return; }
        if (damageAmount <= 0f) { return; }

        Debug.Log($"[Health] TakeDamage called on {gameObject.name}. amount = {damageAmount} BEFORE = {currentHealth}");
        currentHealth -= damageAmount;

        Debug.Log($"[Health] AFTER = {currentHealth}");
        if (currentHealth <= 0f)         
        {
            //currentHealth = 0f;
            Debug.Log($"[Health] {gameObject.name} is now dead.");
            HandleDeath();
        }
        //currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    public void Heal(float healAmount)
    {
        if (died) { return; }
        if (healAmount <= 0f) { return; }

        currentHealth += healAmount;
        if (currentHealth > maxHealth)         
        {
            currentHealth = maxHealth;
        }

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    public float HealthPercent01()
    {
        if (maxHealth <= 0)
        { return 0f; }
        return (float)currentHealth / maxHealth;
    }

    private void HandleDeath()
    {
        var cloneAI = GetComponent<DarkCloneAI>();
        if (cloneAI != null)
        {
            bool hasBoss = (cloneAI.ownerBoss != null);
            Debug.Log($"{name} died. Clone detected. Has ownerBoss? {hasBoss}. Element = {cloneAI.elemetnId}");

            if (hasBoss)
            {
                cloneAI.ownerBoss.OnCloneDefeated(cloneAI.elemetnId);
            }

            Destroy(gameObject);
            return;
        }

        var rep = GetComponent<CalamityDeathReporter1>();
        Debug.Log($"{name} died. Has calamity reporter? {(rep != null)}");

        if (rep != null)
        {
            rep.ReportDeath();
            Destroy(gameObject);
            return;
        }

        Debug.Log($"{name} died. No reporter found. Destroying.");
        Destroy(gameObject);
    }
}
