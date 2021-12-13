using UnityEngine;

public class UnitHelper : MonoBehaviour
{
    private UnitAttack _unitAttack;
    private Unit _unit;

    private void Awake()
    {
        _unitAttack = GetComponentInParent<UnitAttack>();
        _unit = GetComponentInParent<Unit>();
    }

    private void TriggerAttackAnimationEvent()
    {
        _unitAttack.TriggerAttack();
    }

    private void TriggerDeathAnimationEvent()
    {
        _unit.TriggerDeath();
    }
}
