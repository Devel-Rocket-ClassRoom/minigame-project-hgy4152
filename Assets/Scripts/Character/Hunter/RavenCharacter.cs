using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(HunterCreator))]
[RequireComponent(typeof(Skill_Raven))]
public class RavenCharacter : Character
{
    [SerializeField]
    float recoilAngle = 7.261f;

    [SerializeField]
    float recoilDuration = 0.08f;

    [SerializeField]
    float returnDuration = 0.18f;

    public override ClassType Type => ClassType.Hunter;
    public override Color classColor => Color.green;

    int _stageGroupCounter;

    public override void OnStageStart() => _stageGroupCounter = 0;

    public override void OnChainHitEvent()
    {
        base.OnChainHitEvent();

        if (_hitEventIndex > _chainCount)
            return;

        DOTween.Kill(transform);
        DOTween
            .Sequence()
            .Append(
                transform
                    .DOLocalRotate(new Vector3(0, 0, recoilAngle), recoilDuration)
                    .SetEase(Ease.OutQuad)
            )
            .Append(transform.DOLocalRotate(Vector3.zero, returnDuration).SetEase(Ease.InOutSine))
            .OnComplete(StartBreathing);
    }

    // Predator: 스테이지 누적 본인 그룹 3번째마다 데미지 +50%
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        _stageGroupCounter++;
        if (_stageGroupCounter % 3 == 0)
            return Mathf.RoundToInt(damage * 1.5f);
        return damage;
    }
}
