using UnityEngine;

public class IceClone : MonoBehaviour
{
    public float lifetime = 10f;
    float t;

    private void Update()
    {
        t += Time.deltaTime;
        if (t >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}
