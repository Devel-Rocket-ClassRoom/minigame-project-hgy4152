using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(ArcherCreator))]
[RequireComponent(typeof(Skill_Skadi))]
public class SkadiCharacter : Character
{
    [SerializeField]
    float jumpHeight = 0.5f;

    [SerializeField]
    float jumpUpDuration = 0.333f;

    [SerializeField]
    float holdPerChain = 0.067f;

    [SerializeField]
    float attackAnimDuration = 0.7f;

    public override ClassType Type => ClassType.Archer;
    public override Color classColor => Color.cyan;

    int _frostStacks;
    const int MaxFrostStacks = 5;
    EnemyController _cachedTarget;

    public int FrostStacks => _frostStacks;

    public override void OnStageStart() => _frostStacks = 0;

    public override void PlaySkillEffect(
        int chainCount,
        int[] perHitDamages = null,
        EnemyController target = null
    )
    {
        base.PlaySkillEffect(chainCount, perHitDamages, target);
        _cachedTarget = target;

        float peakY = _idlePos.y + jumpHeight;
        float holdDuration = (_chainCount - 1) * holdPerChain;
        float lastChainT = jumpUpDuration + holdDuration;
        float fallDuration = attackAnimDuration - lastChainT;

        DOTween
            .Sequence()
            .Append(transform.DOLocalMoveY(peakY, jumpUpDuration).SetEase(Ease.OutQuad))
            .AppendInterval(holdDuration)
            .Append(transform.DOLocalMoveY(_idlePos.y, fallDuration).SetEase(Ease.InQuad))
            .OnComplete(StartBreathing);
    }

    public override void OnChainHitEvent()
    {
        base.OnChainHitEvent();

        if (_hitEventIndex > _chainCount || _cachedTarget == null || _frostStacks <= 0)
            return;

        // 타격마다 동상 스택 1 소모 + 적 최대 체력의 5% 추가 피해
        _frostStacks--;
        int bonus = Mathf.RoundToInt(_cachedTarget.MaxHp * 0.05f);
        _cachedTarget.TakeDamage(bonus, classColor);
    }

    // 혹한의 후예: 공격 시 동상 1중첩 추가 (최대 5)
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        _frostStacks = Mathf.Min(_frostStacks + 1, MaxFrostStacks);
        return damage;
    }
}
