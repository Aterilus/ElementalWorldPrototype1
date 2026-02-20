using UnityEngine;
[CreateAssetMenu(menuName = "ElementalWorld/Calamity Definition")]
public class CalamityDefinition : ScriptableObject
{
    public string calamityName;
    public GameObject prefab;

    [Header("Spawn")]
    public float minDistanceFromPlayer = 60f;
    public float maxDistanceFromPlayer = 250f;

    [Header("Roam")]
    public float roamSpeed = 4f;
    public float chaseSpeed = 6f;
    public float detectRange = 50f;

    [Header("Attack")]
    public float attackRange = 10f;
    public float attackCoolDown = 1f;

    [Header("Global feel (teleport reposition)")]
    public float repositionIfPlayerFaterThan = 400f;
    public float repositionCoolDown = 20f;

    [Header("Progress Gating")]
    public int minLevelToAttack = 5;
    public int minLevelToGoLethal = 10;
}
