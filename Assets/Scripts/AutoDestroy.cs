using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float life = 1.5f;
    private void Start() => Destroy(gameObject, life);
}
