using DG.Tweening;
using UnityEngine;

public class PaladinCharacter : Character
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

    bool _stageImmunityUsed;

    public override void OnStageStart() => _stageImmunityUsed = false;

    // Divine Shield: 스테이지 누적 디스카드 20개 도달 시 이번 턴 디버프 전부 무효화 (스테이지당 1회)
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        if (!_stageImmunityUsed && judge.stageDiscardsUsed >= 20)
        {
            judge.ClearDebuffs();
            _stageImmunityUsed = true;
            var prefab = (skill as Skill_Victor_HolySlash)?.passiveEffectPrefab;
            if (prefab != null)
            {
                var go = Instantiate(prefab, transform.position, Quaternion.identity);
                Destroy(go, 0.5f);
            }
        }
        return damage;
    }
}
