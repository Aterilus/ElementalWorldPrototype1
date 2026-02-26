using UnityEngine;

public class DarkCopyMinion : MonoBehaviour
{
    public Transform player;
    public GameObject assignedMovePrefab;

    public float castInterval = 4f;
    public float windup = 0.5f;
    public float lifetime = 10f;

    float nextCast;

    private void Start()
    {
        nextCast = Time.time + Random.Range(0.5f, 1.5f);
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if(player == null || assignedMovePrefab == null) {  return; }

        Vector3 look = player.position - transform.position;
        look.y = 0f;
        if (look.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(look);
        }

        if (Time.time >= nextCast)
        {
            nextCast = Time.time + castInterval;
            Invoke(nameof(DoCast), windup);
        }
    }

    private void DoCast()
    {
        if (assignedMovePrefab == null) { return; }

        var obj = Instantiate(assignedMovePrefab, transform.position, transform.rotation);

        var targetSetter = obj.GetComponent<IAttackTargetReceiver>();
        if (targetSetter != null)
        {
            targetSetter.SetTarget(player);
        }
    }
}
