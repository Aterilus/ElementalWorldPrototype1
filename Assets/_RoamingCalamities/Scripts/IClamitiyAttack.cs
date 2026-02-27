using UnityEngine;

public interface ICalamityAttack
{
    void Execute(Transform caster, Transform player, bool allowLethal);
}
