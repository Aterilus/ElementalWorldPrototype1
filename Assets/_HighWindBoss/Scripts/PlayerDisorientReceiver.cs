using UnityEngine;
using System.Collections;

public class PlayerDisorientReceiver : MonoBehaviour
{
    public bool IsDisoriented {  get; private set; }
    public bool InvertControls { get; private set; }
    public float MoveMultiplier { get; private set; }

    private Coroutine routine;

    public void ApplyDisorient(float duration, float moveMultiplier, bool invertControls)
    {
        if (routine != null) { StopCoroutine(routine); }
        routine = StartCoroutine(DisorientRoutine(duration, moveMultiplier, invertControls));
    }

    private IEnumerator DisorientRoutine(float duration, float moveMultiplier, bool invertControls)
    {
        IsDisoriented = true;
        MoveMultiplier = Mathf.Clamp(moveMultiplier, 0.2f, 1f);
        InvertControls = invertControls;

        yield return new WaitForSeconds(duration);

        IsDisoriented = false;
        MoveMultiplier = 1f;
        InvertControls = false;
        routine = null;
    }
}
