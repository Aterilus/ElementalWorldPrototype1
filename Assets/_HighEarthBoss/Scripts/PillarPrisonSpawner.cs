using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PillarPrisonSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject pillarPrefab;
    [SerializeField] private GameObject warningPrefab;

    [Header("Shape")]
    [SerializeField] private int pillarCount = 6;
    [SerializeField] private float radius = 3.5f;

    [Header("Timing")]
    [SerializeField] private float telegraphTime = 1.0f;
    [SerializeField] private float activeTime = 2.5f;

    [Header("Grounding")]
    [SerializeField] private bool snapToGround = true;
    [SerializeField] private float raycastHeight = 30f;
    [SerializeField] private LayerMask groundMask = ~0;

    private readonly List<GameObject> pillars = new List<GameObject>();

    public void SpawnAt(Vector3 center)
    {
        StartCoroutine(DoSpawn(center));
    }

    private IEnumerator DoSpawn(Vector3 center)
    {
        Vector3[] points = new Vector3[pillarCount];
        for (int i = 0; i < pillarCount; ++i)
        {
            float angle = (Mathf.PI * 2f) * (i / (float)pillarCount);
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
            points[i] = center + offset;

            if (snapToGround)
            {
                points[i] = SnapToGround(points[i]);
            }

            if (warningPrefab != null)
            {
                Instantiate(warningPrefab, points[i], Quaternion.identity);
            }
        }

        yield return new WaitForSeconds(telegraphTime);

        pillars.Clear();
        for (int i = 0; i < points.Length; ++i)
        {
            GameObject point = Instantiate(pillarPrefab, points[i], Quaternion.identity);
            pillars.Add(point);
        }

        yield return new WaitForSeconds(activeTime);

        for (int i = 0; i < pillars.Count; ++i)
        {
            if (pillars[i] != null)
            {
                Destroy(pillars[i]);
            }
        }

        Destroy(gameObject);
    }

    private Vector3 SnapToGround(Vector3 point)
    {
        Ray ray = new Ray(point + Vector3.up * raycastHeight, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastHeight * 2f, groundMask, QueryTriggerInteraction.Ignore))
        {
            point.y = hit.point.y;
        }
        return point;
    }
}
