using UnityEngine;
using System.Reflection;

public class PlayerHealthBridge : MonoBehaviour 
{
    public MonoBehaviour oldHealthScript;

    public string getHealthMethodName = "GetCurrentHealth";

    public bool IsDead
    {
        get
        {
            if (oldHealthScript == null) return false;

            MethodInfo method = oldHealthScript.GetType().GetMethod(getHealthMethodName);

            if (method == null) return false;

            object result = method.Invoke(oldHealthScript, null);

            if (result is int i) return i <= 0;
            if (result is float f) return f <= 0f;

            return false;
        }
    }
}
