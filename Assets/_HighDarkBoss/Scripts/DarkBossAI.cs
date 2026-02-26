using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class DarkBossAI : MonoBehaviour
{
    [System.Serializable]
    public class ElementPool
    {
        public string elementID;
        public GameObject clonePrefab;
        public List<DarkBossMove> moves = new();
    }

    [System.Serializable]
    public class DarkBossMove
    {
        public string moveName;
        public GameObject movePrefab;
        public float cooldown = 6f;
        public float windup = 1.0f;
        [HideInInspector] public float nextReadyTime;

        public enum SpawnMode
        {
            AtBoss,
            AtPlayer,
            RandomAroundPlayer,
            RandomInArena
        }

        public SpawnMode spawnMode = SpawnMode.AtBoss;
        public float spawnRadius = 8f;
        public bool groundSnap = true;
    }

    [Header("References")]
    public Transform player;
    public Transform castPoint;
    public Health bossHealth;
    public BossArenaFlow arenaFlow;

    [Header("Phase Rules")]
    public float phase2AtHealthPercent = 0.5f;
    public float decisionInterval = 0.75f;

    [Header("Phase 1 Kit")]
    public DarkBossMove abyssBrandMove;
    public DarkBossMove summonMove;

    [Header("Phase 2 Kit")]
    public List<DarkBossMove> phase2Signature = new List<DarkBossMove>();

    [Header("Borrowed Elemental Pool")]
    public List<ElementPool> borrowedMoves = new List<ElementPool>();

    [Header("Summoning Rules")]
    public bool onlyOneAtATime = true;
    public bool sealElementWhenCloneDies = true;
    public bool unlockPhase2WhenNoElementsLeft = true;

    [Header("Behavior")]
    public float minRangeToAct = 3f;
    public float maxRangeToAct = 35f;
    public float postCastRecovery = 0.2f;


    bool isCasting;
    private HashSet<string> sealedElements = new HashSet<string>();
    private bool isPhase2;

    private bool cloneAlive;
    private string activeCloneElementId = "";
        

    public void Start()
    {
        if (player == null)
        { 
            var p = GameObject.FindGameObjectWithTag("Player"); 
            if (p != null)
            { 
                player = p.transform; 
            }
        }

        InitMoves(abyssBrandMove);
        InitMoves(summonMove);
        foreach (var move in phase2Signature)
        {
            InitMoves(move);
        }
        foreach (var pool in borrowedMoves)
        {
            if (pool == null || pool.moves == null) {  continue; }
            foreach (var move in pool.moves) { InitMoves(move); }
        }

        StartCoroutine(BrainLoop());
    }

    private void Update()
    {
        if (bossHealth != null && bossHealth.IsDead)
        {
            arenaFlow?.OnBossDefeated();
            enabled = false;
            return;
        }

        if (!isPhase2 && bossHealth != null)
        {
            float hp01 = bossHealth.currentHealth / Mathf.Max(1f, bossHealth.maxHealth);
            if (hp01 <= phase2AtHealthPercent)
            {
                EnterPhase2("HP threshold reached");
            }
        }

        if (!isPhase2 && unlockPhase2WhenNoElementsLeft && GetSummonableElementsCount() == 0)
        {
            EnterPhase2("No summonable elements left");
        }
    }

    private void EnterPhase2(string reason)
    {
        isPhase2 = true;
        // You can add any additional logic for entering phase 2 here (e.g., visual changes, new attacks, etc.)
        Debug.Log($"Dark Boss has entered Phase 2: {reason}");
    }

    private IEnumerator BrainLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(decisionInterval);

            if(isCasting || player == null)
            { 
                continue; 
            }

            float dist = Vector3.Distance(transform.position, player.position);
            if (dist < minRangeToAct || dist > maxRangeToAct)
            {
                continue;
            }

            DarkBossMove chosen = ChooseMove();
            if (chosen == null)
            {
                continue;   
            }

            yield return CastMove(chosen);
        }
    }

    private DarkBossMove ChooseMove()
    {
        if (!isPhase2)
        {
            bool canSummon = summonMove != null && summonMove.movePrefab != null && IsReady(summonMove);
            bool hasElements = GetSummonableElementsCount() > 0;

            if (onlyOneAtATime && cloneAlive)
            {
                canSummon = false;
            }

            float r = Random.value;

            if (canSummon && hasElements && r < 0.30f)
            {
                Debug.Log("Dark boss chose Summon move");
                return summonMove;
            }
            if (abyssBrandMove != null && abyssBrandMove.movePrefab != null && IsReady(abyssBrandMove) && r < 0.60f)
            {
                return abyssBrandMove;
            }

            return GetRandomReadyBorrowedFromUnsealed();
        }
        else
        {
            float r = Random.value;
            if (phase2Signature != null && phase2Signature.Count > 0 && r < 0.80f)
            {
                return GetRandomReadyMove(phase2Signature);
            }

            return GetRandomReadyBorrowedFromUnsealed();
        }
    }

    private IEnumerator CastMove(DarkBossMove chosenMove)
    {
        if (chosenMove == null || chosenMove.movePrefab == null) { yield break; }
        if (!IsReady(chosenMove)) {  yield break; }

        isCasting = true;
        chosenMove.nextReadyTime = Time.time + chosenMove.cooldown;

        Vector3 look = player.position - transform.position;
        look.y = 0f;
        if (look.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(look);
        }

        if (chosenMove.windup > 0f)
        {
            yield return new WaitForSeconds(chosenMove.windup);
        }

        Vector3 spawnPos = GetSpawnPosition(chosenMove);
        Quaternion spawnRot = castPoint ? castPoint.rotation : transform.rotation;

        GameObject obj = Instantiate(chosenMove.movePrefab, spawnPos, spawnRot);

        var targetSetter = obj.GetComponent<IAttackTargetReceiver>();
        if(targetSetter != null) { targetSetter.SetTarget(player); }

        yield return new WaitForSeconds(postCastRecovery);
        isCasting = false;
    }

    private DarkBossMove GetRandomReadyBorrowedFromUnsealed()
    {
        List<DarkBossMove> temp = new List<DarkBossMove>();

        foreach (var pool in borrowedMoves)
        {
            if (pool == null) {  continue; }
            string id = Normalize(pool.elementID);
            if (string.IsNullOrEmpty(id)) { continue; }
            if (sealedElements.Contains(id)) { continue; }

            if (pool.moves != null)
            {
                temp.AddRange(pool.moves);
            }
        }

        return GetRandomReadyMove(temp);
    }

    private bool IsReady(DarkBossMove move) => move != null && Time.time >= move.nextReadyTime;

    private DarkBossMove GetRandomReadyMove(List<DarkBossMove> listOfMoves)
    {
        if (listOfMoves == null || listOfMoves.Count == 0) { return null; }

        for (int i = 0; i < 8; i++)
        {
            var move = listOfMoves[Random.Range(0, listOfMoves.Count)];
            if (move != null && move.movePrefab != null && IsReady(move))
            {
                return move;
            }
        }

        foreach (var move in listOfMoves)
        {
            if (move != null && move.movePrefab != null && IsReady(move))
            {
                return move;
            }
        }

        return null;
    }

    private void InitMoves(DarkBossMove move)
    {
        if (move == null) { return; }
        move.nextReadyTime = 0f;
    }

    public int GetSummonableElementsCount()
    {
        int count = 0;
        foreach (var pool in borrowedMoves)
        {
            if (pool == null) {  continue; }
            string id = Normalize(pool.elementID);
            if (string.IsNullOrEmpty(id)) { continue; }
            if (!sealedElements.Contains(id)) { count++; }
        }

        return count;
    }

    public ElementPool GetRandomSummonablePool()
    {
        List<ElementPool> valid = new List<ElementPool>();  

        foreach (var pool in borrowedMoves)
        {
            if (pool == null) { continue; }
            string id = Normalize(pool.elementID);
            if (string.IsNullOrEmpty(id)) {continue; }
            if (sealedElements.Contains(id)) { continue; }
            if (pool.clonePrefab == null) { continue; }
            valid.Add(pool);
        }

        if (valid.Count == 0) {  return null; }
        return valid[Random.Range(0, valid.Count)];
    }

    public DarkBossMove PickOneBorrowedMoveFrom(ElementPool pool)
    {
        if (pool == null || pool.moves == null || pool.moves.Count == 0) { return null; }

        for (int i = 0; i < 10; i++)
        {
            var m = pool.moves[Random.Range(0, pool.moves.Count)];
            if (m != null && m.movePrefab != null) { return m; }
        }

        foreach (var move in pool.moves)
        {
            if (move != null && move.movePrefab != null) { return move; }
        }

        return null;
    }

    private Vector3 GetSpawnPosition(DarkBossMove move)
    {
        Vector3 bossPos = castPoint ? castPoint.position : transform.position;
        Vector3 playerPos = player ? player.position : bossPos;

        Vector3 pos = bossPos;

        switch (move.spawnMode)
        {
            case DarkBossMove.SpawnMode.AtBoss:
                pos = bossPos;
                break;
            case DarkBossMove.SpawnMode.AtPlayer:
                pos = playerPos;
                break;
            case DarkBossMove.SpawnMode.RandomAroundPlayer:
                {
                    Vector3 off = Random.insideUnitCircle * move.spawnRadius;
                    off.y = 0f;
                    pos = playerPos + off;
                }
                break;
            case DarkBossMove.SpawnMode.RandomInArena:
                {
                    Vector3 center = transform.position;
                    Vector3 off = Random.insideUnitCircle * move.spawnRadius;
                    off.y = 0f;
                    pos = center + off;
                }
                break;
        }

        if (move.groundSnap)
        {
            pos.y = 0f;
        }

        return pos;
    }

    public void OnCloneSpawned(string elementId)
    {
        cloneAlive = true;
        activeCloneElementId = Normalize(elementId);
    }

    public void OnCloneDefeated(string elementId)
    {
        string id = Normalize(elementId);
        if (string.IsNullOrEmpty(id)) { return; }

        if (cloneAlive && id == activeCloneElementId)
        {
            cloneAlive = false;
            activeCloneElementId = "";
        }

        if (sealElementWhenCloneDies && !sealedElements.Contains(id))
        {
            sealedElements.Add(id);
            Debug.Log($"Element Sealed: {id}. Borrowed move pool shrunk.");

            if (unlockPhase2WhenNoElementsLeft && GetSummonableElementsCount() == 0 && !isPhase2)
            {
                EnterPhase2("All elements sealed");
            }
        }
    }

    static string Normalize(string s)
    {
        return string.IsNullOrEmpty(s) ? "" : s.ToLower().Trim();
    }
}

public interface IAttackTargetReceiver
{
    void SetTarget(Transform target);
}