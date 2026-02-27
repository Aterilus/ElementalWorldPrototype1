using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
    public float lifeLine = 0.5f;
    private void Start() => Destroy(gameObject, lifeLine);
}
