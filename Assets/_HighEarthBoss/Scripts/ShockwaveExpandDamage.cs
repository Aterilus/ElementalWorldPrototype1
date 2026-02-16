using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent (typeof(Rigidbody))]
public class ShockwaveExpandDamage : MonoBehaviour
{
    [Header("Expand")]
    [SerializeField] private float expandSpeed = 10f;
    [SerializeField] private float maxRadius = 12f;
    [SerializeField] private float lifetime = 1.2f;

    [Header("Damage")]
    [SerializeField] private int damage = 15;
    [SerializeField] private float hitCooldown = 0.25f;
    [SerializeField] private string targetTag = "Player";

    [Header("Visuals")]
    [SerializeField] private Transform ringVisuals;
    [SerializeField] private float visualThicknessY = 0.05f;

    private SphereCollider col;
    private float timer;

    private readonly Dictionary<Transform, float> nextHitTime = new Dictionary<Transform, float>();

    private void Awake()
    {
        col = GetComponent<SphereCollider>();
        col.isTrigger = true;

        Rigidbody rb = col.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        if (ringVisuals == null)
        {
            Transform t = transform.Find("ShockwaveRing");
            if (t != null) { ringVisuals = t; }
        }
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        timer = Time.deltaTime;
        col.radius = Mathf.Min(maxRadius, col.radius + expandSpeed * Time.deltaTime);

        if (ringVisuals != null)
        {
            float diameter = col.radius * 2f;
            ringVisuals.localScale = new Vector3(diameter, visualThicknessY, diameter);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag(targetTag)) {  return; }

        Transform t = other.transform;
        float now = Time.time;

        if (nextHitTime.TryGetValue(t, out float next) && now < next) { return; }
        nextHitTime[t] = now + hitCooldown;

        Health playerHP = other.gameObject.GetComponentInParent<Health>();
        if (playerHP != null)
        {
            playerHP.TakeDamage(damage);
        }
    }
}
