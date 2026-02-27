using UnityEngine;

public class TelegraphVisuals : MonoBehaviour
{
    public float life = 1.0f;
    public Vector3 startScale = new Vector3(1, 1, 1);
    public Vector3 endScale = new Vector3(1, 1, 1);

    private void Start()
    {
        transform.localScale = startScale;
        Destroy(gameObject, life);
    }

    private void Update()
    {
        float t = Mathf.Clamp01(1f - (life > 0 ? (GetLifeRemaining() / life) : 0f));
        transform.localScale = Vector3.Lerp(startScale, endScale, t);
    }

    private float GetLifeRemaining()
    {
        return Mathf.Max(0f, life - (Time.time - spawnTime));
    }

    float spawnTime;
    private void Awake() => spawnTime = Time.time;
}
