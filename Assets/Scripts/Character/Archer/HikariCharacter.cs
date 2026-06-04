using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(ArcherCreator))]
[RequireComponent(typeof(Skill_Hikari))]
public class HikariCharacter : Character
{
    [SerializeField]
    float jumpHeight = 0.5f;

    [SerializeField]
    float jumpUpDuration = 0.15f;

    [SerializeField]
    float jumpDownDuration = 0.2f;

    public override ClassType Type => ClassType.Archer;
    public override Color classColor => Color.yellow;

    public void StartJump(System.Action onPeak = null)
    {
        DOTween.Kill(transform);
        DOTween
            .Sequence()
            .SetTarget(transform)
            .Append(
                transform
                    .DOLocalMoveY(_idlePos.y + jumpHeight, jumpUpDuration)
                    .SetEase(Ease.OutQuad)
            )
            .AppendCallback(() => onPeak?.Invoke())
            .Append(transform.DOLocalMoveY(_idlePos.y, jumpDownDuration).SetEase(Ease.InQuad))
            .OnComplete(StartBreathing);
    }

    // Snipe: 적 최대 체력의 5% 추가 데미지
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        return damage + Mathf.RoundToInt(judge.bossMaxHp * 0.05f);
    }
}
