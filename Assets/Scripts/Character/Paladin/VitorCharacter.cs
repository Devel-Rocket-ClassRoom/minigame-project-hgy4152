using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(PaladinCreator))]
[RequireComponent(typeof(Skill_Victor))]
public class VitorCharacter : Character
{
    [SerializeField]
    float chargeDuration = 0.3f;

    [SerializeField]
    Vector3 chargeStopOffset;

    public override ClassType Type => ClassType.Paladin;
    public override Color classColor => Color.white;
    protected override bool IsSingleCast => true;

    public void OnChargeStartEvent(Vector3 targetPos, System.Action onArrival = null)
    {
        Vector3 dashEnd =
            (
                transform.parent != null
                    ? transform.parent.InverseTransformPoint(targetPos)
                    : targetPos
            ) + chargeStopOffset;
        dashEnd.y = _idlePos.y;

        transform
            .DOLocalMove(dashEnd, chargeDuration)
            .SetEase(Ease.InQuart)
            .OnComplete(() =>
            {
                onArrival?.Invoke();
                transform
                    .DOLocalMove(_idlePos, chargeDuration * 0.5f)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(StartBreathing);
            });
    }

    bool _stageImmunityUsed;
    int _stageDiscardsUsed;
    public override int StackCount => _stageImmunityUsed ? -1 : _stageDiscardsUsed;

    public override void OnStageStart()
    {
        _stageImmunityUsed = false;
        _stageDiscardsUsed = 0;
    }

    // Divine Shield: 자기 클래스 블럭 스테이지 누적 디스카드 20개 도달 시 이번 턴 디버프 전부 무효화 (스테이지당 1회)
    public override bool IsProtectionPassive(ChainJudge judge)
    {
        _stageDiscardsUsed = 0;
        judge.stageDiscardsByClass?.TryGetValue(Type, out _stageDiscardsUsed);
        if (!_stageImmunityUsed && _stageDiscardsUsed >= 20)
        {
            if (!judge.isPreview)
            {
                _stageImmunityUsed = true;
                var prefab = (skill as Skill_Victor)?.passiveEffectPrefab;
                if (prefab != null)
                {
                    var go = Instantiate(prefab, transform.position, Quaternion.identity);
                    Destroy(go, 0.5f);
                }
            }
            return true;
        }
        return false;
    }
}
