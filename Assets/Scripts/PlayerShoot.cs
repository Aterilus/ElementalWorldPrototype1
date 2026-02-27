using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public Transform attackPoint;
    public GameObject projectilePrefab;
    public float fireCoolDown = 0.25f;

    float cd;

    private void Update()
    {
        cd -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && cd <= 0f)
        {
            Shoot();
            cd = fireCoolDown;
        }
    }

    void Shoot()
    {
        var go = Instantiate(projectilePrefab, attackPoint.position, attackPoint.rotation);
        var mover = go.GetComponent<ProjectileMover>();
        mover.Fire(attackPoint.forward);
    }
}
