using System;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(WarriorCreator))]
[RequireComponent(typeof(Skill_Reon))]
public class ReonCharacter : Character
{
    [SerializeField]
    float returnDuration = 0.18f;

    public override ClassType Type => ClassType.Warrior;
    public override Color classColor => Color.red;

    // Chain1/2 이동 후 마지막 체인이면 복귀
    public void TryReturnAfterChain(Action onComplete = null)
    {
        if (_hitEventIndex < _chainCount)
            return;
        DOTween
            .Sequence()
            .SetTarget(transform)
            .Append(transform.DOLocalMove(_idlePos, returnDuration).SetEase(Ease.InOutSine))
            .OnComplete(() =>
            {
                StartBreathing();
                onComplete?.Invoke();
            });
    }

    // Chain Mastery: 3체인 시 데미지 +30%
    public override float GetChainTypeBonus(ChainJudge judge, ChainGroup group) =>
        group.Length >= 3 ? 0.3f : 0f;
}
