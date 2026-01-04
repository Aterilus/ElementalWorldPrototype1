using UnityEngine;
using System.Reflection;

public class PlayerDamageBridge : MonoBehaviour 
{
    [Header("Drag your OLD player health script here")]
    public MonoBehaviour oldPlayerHealthScript;

    [Header("Method names in the OLD player health script")]
    public string takeDamageMethodName = "TakeDamage";
    public float debugDamageMultiplier = 1f;

    public void TakeDamage(float damage)
    {
        if (oldPlayerHealthScript == null) return;

        damage *= debugDamageMultiplier;

        MethodInfo method = oldPlayerHealthScript.GetType().GetMethod(takeDamageMethodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (method == null)
        {
            Debug.LogError($"Method '{takeDamageMethodName}' not found in '{oldPlayerHealthScript.GetType().Name}'.");
            return;
        }

        var p = method.GetParameters();
        if (p.Length == 1)
        {
            if (p[0].ParameterType == typeof(int))
            {
                method.Invoke(oldPlayerHealthScript, new object[] { Mathf.RoundToInt(damage) });
            }
            else
            {
                method.Invoke(oldPlayerHealthScript, new object[] { damage });
            }
        }
    }
}
