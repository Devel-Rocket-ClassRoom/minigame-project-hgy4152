using DG.Tweening;
using UnityEngine;

public class AbelCharacter : Character
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

    // 고행의 발걸음: 활성 디버프 1개당 데미지 +10%
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        int debuffCount = judge.activeModifiers.Count;
        if (debuffCount <= 0)
            return damage;
        return Mathf.RoundToInt(damage * (1f + debuffCount * 0.1f));
    }
}
