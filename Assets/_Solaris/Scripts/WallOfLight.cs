using UnityEngine;
using System.Collections;

public class WallOfLight : MonoBehaviour
{
    [Header("Refs")]
    public float healPerSecond = 5f;
    public bool isActive;
    public GameObject visualRoot;

    private Scene2Health solarisHealth;


    private void Awake()
    {
        solarisHealth = GetComponent<Scene2Health>();
    }

    public void Activate()
    {
        isActive = true;
        visualRoot.SetActive(true);
    }

    public void Deactivate()
    {
        isActive = false;
        visualRoot.SetActive(false);
    }

    private void Update()
    {
        if (!isActive) return;

        if (solarisHealth != null)
        {
            solarisHealth.Heal(healPerSecond * Time.deltaTime);
        }
    }
}