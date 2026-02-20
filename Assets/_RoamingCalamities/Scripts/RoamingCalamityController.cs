using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;

public class RoamingCalamityController : MonoBehaviour 
{
    private CalamityDefinition def;
    private Transform player;
    private LayerMask groundMask;

    private Vector3 targetPoint;
    private float nextWanderPick;
    private float nextAttackTime;
    private float nextRepositionTime;

    private CharacterController characterController;

    private enum State { Wander, Chase, Attack }
    private State state;

    public void Initialize(CalamityDefinition definition, Transform playerRef, LayerMask ground)
    {
        def = definition;
        player = playerRef;
        groundMask = ground;
        characterController = GetComponent<CharacterController>();

        PickNewTarget();
        state = State.Wander;
    }

    private void Update()
    {
        if (def == null || player == null || CalamityWorldBounds.Instance == null) { return; }

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > def.repositionIfPlayerFaterThan && Time.time >= nextRepositionTime)
        {
            nextRepositionTime = Time.time;
            Reposition();
            return;
        }

        if (dist <= def.attackRange) { state = State.Attack; }
        else if (dist <= def.detectRange) { state = State.Chase; }
        else { state = State.Wander; }

        if (state == State.Wander) { Wander(); }
        else if (state == State.Chase) { Chase(); }
        else { Attack(); }
    }

    private void Wander()
    {
        if (Time.time >= nextWanderPick || Vector3.Distance(transform.position, targetPoint) < 2f)
        {
            PickNewTarget();
        }

        MoveTowards(targetPoint, def.roamSpeed);
    }

    private void Chase()
    {
        MoveTowards(player.position, def.chaseSpeed);
    }

    private void Attack()
    {
        Debug.Log($"{name} ATTACK state. Dist to player: {Vector3.Distance(transform.position, player.position)}");
        Face(player.position);

        var progress = player.GetComponent<PlayerReadiness>();
        int lvl = progress ? progress.readinessLevel : 1;

        bool canAttack = lvl >= def.minLevelToAttack;
        bool allowLethal = lvl >= def.minLevelToGoLethal;

        var all = GetComponents<MonoBehaviour>();
        foreach (var t in all)
        {
            if (t is ICalamityAttack atk)
            {
                atk.Execute(transform, player, allowLethal);
                return;
            }
        }

        Debug.LogWarning($"{name} has no attack component implementing ICalamityAttack.");
    }

    private void PickNewTarget()
    {
        nextWanderPick = Time.time + Random.Range(2f, 5f);

        Vector3 p = CalamityWorldBounds.Instance.GetRandomPointHigh();
        if (Physics.Raycast(p, Vector3.down, out RaycastHit hit, 1000f, groundMask))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = transform.position;
        }
    }

    private void MoveTowards(Vector3 targetPoint, float speed)
    {
        Vector3 dir = (targetPoint - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.01f) { return; }

        dir.Normalize();
        Face(transform.position + dir);

        Vector3 move = dir * speed * Time.deltaTime;
        
        move.y = -9.81f * Time.deltaTime;
        if (characterController != null)
        {
            characterController.Move(move);
        }
        else
        {
            transform.position += move;
        }
    }

    private void Face(Vector3 lookPos)
    {
        Vector3 dir = (lookPos - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) { return ; }
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), 10f * Time.deltaTime);
    }

    private void Reposition()
    {
        Vector3 p = CalamityWorldBounds.Instance.GetRandomPointHigh();
        if (Physics.Raycast(p, Vector3.down, out RaycastHit hit, 1000f, groundMask))
        {
            transform.position = hit.point;
            PickNewTarget();
        }
    }
}
