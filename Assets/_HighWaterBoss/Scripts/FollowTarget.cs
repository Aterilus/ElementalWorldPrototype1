using UnityEngine;

public class FollowTarget : MonoBehaviour 
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 1f, 0f);

    private void LateUpdate()
    {
        if (target == null) {  return; }

        transform.position = target.position + offset;
    }
}
