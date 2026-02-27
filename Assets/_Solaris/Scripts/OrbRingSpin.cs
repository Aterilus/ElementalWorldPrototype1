using UnityEngine;

public class OrbRingSpin : MonoBehaviour
{
    public float spinSpeed = 20f;

    private void Update()
    {
        transform.Rotate(0f, spinSpeed * Time.deltaTime, 0f, Space.Self);
    }
}
