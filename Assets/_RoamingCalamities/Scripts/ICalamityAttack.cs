using UnityEngine;

public interface IClamitiyAttack
{
    void Execute(Transform caster, Transform player, bool allowLethal);
}
