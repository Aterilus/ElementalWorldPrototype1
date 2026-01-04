using System.Collections;
using UnityEngine;

public class PlayerStatusEffects : MonoBehaviour
{
    public bool IsStunned {  get; private set; }

    Coroutine stunRoutine;

    public void ApplyStun(float duration)
    {
        if (stunRoutine != null)
        {
            StopCoroutine(stunRoutine);
        }
        stunRoutine = StartCoroutine(StunRoutine(duration));
    }

    IEnumerator StunRoutine(float duration)
    {
        IsStunned = true;
        yield return new WaitForSeconds(duration);
        IsStunned = false;
        stunRoutine = null;
    }
}
