using UnityEngine;

public class SolarisHitReacts : MonoBehaviour 
{
    public void OnHit()
    {
        Debug.Log("Solaris has been hit!");
        transform.localScale *= 0.98f; // Slightly shrink the object to simulate impact
    }
}
