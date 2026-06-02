using DG.Tweening;
using UnityEngine;

public class FaimonCharacter : Character
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
    public override Color classColor => new Color(1f, 0.4f, 0f);

    int _burnStacks;
    const int MaxBurnStacks = 5;

    public int BurnStacks => _burnStacks;

    public override void OnStageStart() => _burnStacks = 0;

    public override void PlaySkillEffect(
        int chainCount,
        int[] perHitDamages = null,
        EnemyController target = null
    )
    {
        base.PlaySkillEffect(chainCount, perHitDamages, target);

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

    // 볼케닉 소울: 공격 시 화상 1중첩. 5중첩 시 적 최대 체력의 10% 추가 피해 후 초기화
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        _burnStacks = Mathf.Min(_burnStacks + 1, MaxBurnStacks);

        if (_burnStacks >= MaxBurnStacks)
        {
            _burnStacks = 0;
            return damage + Mathf.RoundToInt(judge.bossMaxHp * 0.1f);
        }

        return damage;
    }
}
