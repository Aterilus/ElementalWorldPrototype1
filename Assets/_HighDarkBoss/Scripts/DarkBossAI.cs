using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DarkBossAI : MonoBehaviour
{
    public enum AbilityId
    {
        None,
        Wind_Cyclone,
        Wind_HurricaneStrike,

        Water_AcidWaves,
        Water_HydroSnipe,
        Water_WaterPrison,

        Lightning_LightningStrike,
        Lightning_ChainBolt,

        Ice_Blizzard,
        Ice_FrostNova,
        Ice_IceSpike,

        Fire_Fireball,
        Fire_FlamePillar,
        Fire_MeteorImpact,

        Earth_SeismicSlam,
        Earth_PillarPrison,
        Earth_BoulderToss,

        Dark_AbyssBrand,
        Dark_Summon,
        Dark_ShadowChains,
        Dark_UbralRush
    }

    [System.Serializable]
    public class ElementPool
    {
        public string elementID;
        public GameObject clonePrefab;
        public List<AbilityId> abilities = new();
    }

    [System.Serializable]
    public class DarkBossMove
    {
        public string moveName;
        public AbilityId abilityId;
        public float cooldown = 6f;
        public float windup = 1.0f;
        public GameObject movePrefab;
        [HideInInspector] public float nextReadyTime;
    }

    [Header("References")]
    public Transform player;
    public Transform castPoint;
    public Health bossHealth;

    [Header("Ability Packs")]
    public EarthAbilityPack earthPack;
    public FireAbilityPack firePack;
    public IceAbilityPack icePack;
    public LightningAbilityPack lightningPack;
    public WaterAbilityPack waterPack;
    public WindAbilityPack windPack;

    [Header("Phase Settings")]
    public float decisionInterval = 0.75f;
    public bool onlyOneCloneAtATime = true;
    public bool sealElementWhenCloneDies = true;
    public bool unlockPhase2WhenNoElementsLeft = true;

    [Header("Phase 1 Kit")]
    public DarkBossMove abyssBrandMove;
    public DarkBossMove summonMove;

    [Header("Phase 2 Kit")]
    public List<DarkBossMove> phase2Signature = new();

    [Header("Borrowed Elemental Pool")]
    public List<ElementPool> borrowedMoves = new();

    [Header("Behavior")]
    public float minRangeToAct = 3f;
    public float maxRangeToAct = 95f;

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

        if (!windPack) { windPack = GetComponent<WindAbilityPack>(); }
        if (!waterPack) { waterPack = GetComponent<WaterAbilityPack>(); }
        if (!lightningPack) { lightningPack = GetComponent<LightningAbilityPack>(); }
        if (!icePack) { icePack = GetComponent<IceAbilityPack>(); }
        if (!firePack) { firePack = GetComponent<FireAbilityPack>(); }
        if (!earthPack) { earthPack = GetComponent<EarthAbilityPack>(); }

        InitMoves(abyssBrandMove);
        InitMoves(summonMove);
        foreach (var move in phase2Signature)
        {
            InitMoves(move);
        }

        StartCoroutine(BrainLoop());
    }

    private void EnterPhase2(string reason)
    {
        isPhase2 = true;
        // You can add any additional logic for entering phase 2 here (e.g., visual changes, new attacks, etc.)
        Debug.Log($"Dark Boss has entered Phase 2: {reason}");
    }

    IEnumerator BrainLoop()
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

    DarkBossMove ChooseMove()
    {
        if (!isPhase2)
        {
            bool canSummon = summonMove != null && summonMove.movePrefab != null && IsReady(summonMove) && GetSummonableElementsCount() > 0;
            
            if (onlyOneCloneAtATime && cloneAlive)
            {
                canSummon = false;
            }

            float r = Random.value;

            if (canSummon && r < 0.30f)
            {
                Debug.Log("Dark boss chose Summon move");
                return summonMove;
            }
            if (abyssBrandMove != null && IsReady(abyssBrandMove) && r < 0.60f)
            {
                return abyssBrandMove;
            }

            AbilityId borrowed = GetRandomBorrowedAbilityIdFromUnsealed();
            if (borrowed == AbilityId.Dark_AbyssBrand) { return null; }

            return new DarkBossMove
            {
                moveName = "Borrowed",
                abilityId = borrowed,
                cooldown = 2.5f,
                movePrefab = null,
                nextReadyTime = 0f

            };
        }
        else
        {
            if (phase2Signature == null || phase2Signature.Count == 0)
            {
                return null;
            }

            if (Random.value < 0.80f)
            {
                return GetRandomReadyFromList(phase2Signature);
            }

            AbilityId borrowed = GetRandomBorrowedAbilityIdFromUnsealed();
            return new DarkBossMove
            {
                moveName = "Borrowed",
                abilityId = borrowed,
                cooldown = 2.5f,
                windup = 0.4f,
                movePrefab = null,
                nextReadyTime = 0f
            };
        }
    }

    IEnumerator CastMove(DarkBossMove chosenMove)
    {
        if (chosenMove == null) { yield break; }
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

        Transform caster = castPoint ? castPoint : transform;

        if (chosenMove.abilityId == AbilityId.Dark_AbyssBrand || chosenMove.abilityId == AbilityId.Dark_Summon ||
            chosenMove.abilityId == AbilityId.Dark_ShadowChains || chosenMove.abilityId == AbilityId.Dark_UbralRush)
        {
            if (chosenMove.movePrefab != null)
            {
                Instantiate(chosenMove.movePrefab, caster.position, caster.rotation);
                Debug.Log("DarkBoss cast dark prefab move: " + chosenMove.abilityId);
            }
            else
            {
                Debug.LogWarning("Darkboss: dark move missing prefab: " + chosenMove.abilityId);
            }
        }
        else
        {
            ExecuteAbilityImmediate(chosenMove.abilityId, caster, player);
        }

        yield return new WaitForSeconds(0.15f);
        isCasting = false;
    }

    DarkBossMove GetRandomReadyFromList(List<DarkBossMove> list)
    {
        for (int i = 0; i < 8; ++i)
        {
            var move = list[Random.Range(0, list.Count)];
            if (move != null && IsReady(move)) { return move; }
        }

        foreach (var move in list)
        {
            if (move != null && IsReady(move)) { return move; }
        }

        return null;
    }

    AbilityId GetRandomBorrowedAbilityIdFromUnsealed()
    {
        List<AbilityId> temp = new();

        foreach (var pool in borrowedMoves)
        {
            if (pool == null) { continue; }
            string id = Normalize(pool.elementID);
            if (string.IsNullOrEmpty(id)) { continue; }
            if (sealedElements.Contains(id)) { continue; }
            if (pool.abilities != null) { temp.AddRange(pool.abilities); }
        }

        if (temp.Count == 0) { return AbilityId.None; }

        return temp[Random.Range(0, temp.Count)];
    }

    bool IsReady(DarkBossMove move) => move != null && Time.time >= move.nextReadyTime;

    void InitMoves(DarkBossMove move)
    {
        if (move == null) { return; }
        move.nextReadyTime = 0f;
    }

    int GetSummonableElementsCount()
    {
        int count = 0;
        foreach (var pool in borrowedMoves)
        {
            if (pool == null) {  continue; }
            string id = Normalize(pool.elementID);
            if (string.IsNullOrEmpty(id)) { continue; }
            if (sealedElements.Contains(id)) { continue; }
            if (pool.clonePrefab == null) { continue; }
            count++;
        }

        return count;
    }

    public ElementPool GetRandomSummonablePool()
    {
        List<ElementPool> valid = new();  

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

    public AbilityId PickOneBorrowedAbilityIdFrom(ElementPool pool)
    {
        if (pool == null || pool.abilities == null || pool.abilities.Count == 0) { return AbilityId.Wind_Cyclone; }

        return pool.abilities[Random.Range(0, pool.abilities.Count)];
    }

    public void OnCloneSpawned(string elementId)
    {
        cloneAlive = true;
        activeCloneElementId = Normalize(elementId);
        Debug.Log("Clone spawned: " + activeCloneElementId);
    }

    public void OnCloneDefeated(string elementId)
    {
        Debug.Log("OnCloneDefeated called for element: " + elementId);
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
        }

        if (unlockPhase2WhenNoElementsLeft && GetSummonableElementsCount() == 0 && !isPhase2)
        {
            isPhase2 = true;
        }
    }

    void ExecuteAbilityImmediate(AbilityId id, Transform caster, Transform target)
    {
        switch (id)
        {
            case AbilityId.Wind_Cyclone:
                windPack?.CastCyclone(caster, target); 
                break;
            case AbilityId.Wind_HurricaneStrike:
                windPack?.CastHurricaneStrike(caster, target);
                break;
            case AbilityId.Water_AcidWaves:
                waterPack?.StartAcidWaves(target);
                break;
            case AbilityId.Water_HydroSnipe:
                waterPack?.CastHydroSnipe(caster, target);
                break;
            case AbilityId.Water_WaterPrison:
                waterPack?.CastWaterPrison(target);
                break;
            case AbilityId.Lightning_ChainBolt:
                lightningPack?.CastCloudChainCombo(caster, target);
                break;
            case AbilityId.Lightning_LightningStrike:
                lightningPack?.CastLightningStrike(target);
                break;
            case AbilityId.Ice_Blizzard:
                icePack?.CastBlizzard();
                break;
            case AbilityId.Ice_FrostNova:
                icePack?.CastFrostNova(caster);
                break;
            case AbilityId.Ice_IceSpike:
                icePack?.CastIceSpikeLine(caster, target);
                break;
            case AbilityId.Fire_Fireball:
                firePack?.CastFireball(caster, target);
                break;
            case AbilityId.Fire_FlamePillar:
                firePack?.CastFirePillars(target);
                break;
            case AbilityId.Fire_MeteorImpact:
                firePack?.CastMeteorBurst();
                break;
            case AbilityId.Earth_BoulderToss:
                earthPack?.CastBoulder(caster, target);
                break;
            case AbilityId.Earth_PillarPrison:
                earthPack?.CastPillarPrison(target);
                break;
            case AbilityId.Earth_SeismicSlam:
                earthPack?.CastSeismicSlam(caster);
                break;
        }
    }

    public void ExecuteAbilityFromCaster(AbilityId id, Transform caster, Transform target, float windup)
    {
        StartCoroutine(ExecuteDelayed(id, caster, target, windup));
    }

    IEnumerator ExecuteDelayed(AbilityId id, Transform caster, Transform target, float windup)
    {
        if (windup > 0f) { yield return new WaitForSeconds(windup); }
        ExecuteAbilityImmediate(id, caster, target);
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