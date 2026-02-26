using System.Collections;
using UnityEngine;

public class PlayerStatusEffects : MonoBehaviour
{
    public PlayerMovement movement;
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

    public void ApplySlow(float multiplier, float duration)
    {
        if (movement == null) { return; }
        StartCoroutine(SlowRoutine(multiplier, duration));
    }

    System.Collections.IEnumerator SlowRoutine(float multiplier, float duration)
    {
        float original = movement.speed;
        movement.speed = original * multiplier;
        yield return new WaitForSeconds(duration);
        movement.speed = original;
    }
}
