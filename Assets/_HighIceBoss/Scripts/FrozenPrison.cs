using UnityEngine;

public class FrozenPrison : MonoBehaviour
{
    public float hp = 25f;
    public float duration = 3f;
    public Transform player;
    public float radius = 1.5f;

    float t;

    private void Start()
    {
        if (player)
        {
            transform.position = player.position;
        }
    }

    private void Update()
    {
        t += Time.deltaTime;
        if (player)
        {
            transform.position = player.position;
        }
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0f)
        {
            Release();
        }
    }

    void Release()
    {
        Destroy(gameObject);
    }
}
