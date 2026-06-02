using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(PaladinCreator))]
[RequireComponent(typeof(Skill_Tirion))]
public class TirionCharacter : Character
{
    [SerializeField]
    float chargeDuration = 0.3f;

    [SerializeField]
    Vector3 chargeStopOffset;

    public override ClassType Type => ClassType.Paladin;
    public override Color classColor => Color.white;

    public void OnChargeStartEvent()
    {
        Vector3 dashEnd =
            (
                transform.parent != null
                    ? transform.parent.InverseTransformPoint(_targetPos)
                    : _targetPos
            ) + chargeStopOffset;
        dashEnd.y = _idlePos.y;

        transform
            .DOLocalMove(dashEnd, chargeDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
                transform
                    .DOLocalMove(_idlePos, chargeDuration * 0.5f)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(StartBreathing)
            );
    }

    int _accumulatedBonus;

    public override void OnStageStart() => _accumulatedBonus = 0;

    // 참회하는 빛: 데미지 감소 패널티가 있을 때 손실분의 30%를 누적하여 반환
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        if (judge.bossDamageMultiplier < 1f)
        {
            int lost = Mathf.RoundToInt(damage * (1f - judge.bossDamageMultiplier));
            _accumulatedBonus += Mathf.RoundToInt(lost * 0.3f);
        }
        return damage + _accumulatedBonus;
    }
}
