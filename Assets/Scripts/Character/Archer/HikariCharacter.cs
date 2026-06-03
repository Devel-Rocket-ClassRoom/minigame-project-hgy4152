using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(ArcherCreator))]
[RequireComponent(typeof(Skill_Hikari))]
public class HikariCharacter : Character
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
    public override Color classColor => Color.yellow;

    public override void PlaySkillEffect(
        int chainCount,
        int[] perHitDamages = null,
        EnemyController target = null
    )
    {
        base.PlaySkillEffect(chainCount, perHitDamages, target);
        StartJump();
    }

    void StartJump()
    {
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

    // Snipe: 적 최대 체력의 5% 추가 데미지
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        return damage + Mathf.RoundToInt(judge.bossMaxHp * 0.05f);
    }
}
