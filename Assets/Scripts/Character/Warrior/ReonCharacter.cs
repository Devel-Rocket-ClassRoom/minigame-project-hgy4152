using DG.Tweening;
using UnityEngine;

public class ReonCharacter : Character
{
    [Header("=== 체인 이동 ===")]
    [SerializeField]
    float chain1Height = 0.35f;

    [SerializeField]
    float chain1XFrac = 0.12f;

    [SerializeField]
    float chain1Duration = 0.28f;

    [SerializeField]
    float chain2Height = 0.20f;

    [SerializeField]
    float chain2XFrac = 0.30f;

    [SerializeField]
    float chain2Duration = 0.28f;

    [SerializeField]
    float slamHeight = 0.60f;

    [SerializeField]
    float slamUpDuration = 0.20f;

    [SerializeField]
    float slamDownDuration = 0.15f;

    [SerializeField]
    float returnDuration = 0.18f;

    public override ClassType Type => ClassType.Warrior;
    public override Color classColor => Color.red;

    public override void OnChainHitEvent()
    {
        base.OnChainHitEvent();

        if (_hitEventIndex > _chainCount)
            return;

        DOTween.Kill(transform);

        Vector3 cur = transform.localPosition;
        Vector3 targetLocal =
            transform.parent != null
                ? transform.parent.InverseTransformPoint(_targetPos)
                : _targetPos;
        float dx = targetLocal.x - _idlePos.x;

        switch (_hitEventIndex)
        {
            case 1: // 살짝 급각도 대각선 점프 → 제자리 복귀
            {
                Vector3 peak = new Vector3(cur.x + dx * chain1XFrac, cur.y + chain1Height, cur.z);
                DOTween
                    .Sequence()
                    .Append(
                        transform.DOLocalMove(peak, chain1Duration * 0.5f).SetEase(Ease.OutQuad)
                    )
                    .Append(
                        transform.DOLocalMove(_idlePos, chain1Duration * 0.5f).SetEase(Ease.InQuad)
                    )
                    .OnComplete(StartBreathing);
                break;
            }
            case 2: // 완각도 대각선 점프 (높이↓, 전진↑) → 제자리 복귀
            {
                Vector3 peak = new Vector3(cur.x + dx * chain2XFrac, cur.y + chain2Height, cur.z);
                DOTween
                    .Sequence()
                    .Append(
                        transform.DOLocalMove(peak, chain2Duration * 0.5f).SetEase(Ease.OutQuad)
                    )
                    .Append(
                        transform.DOLocalMove(_idlePos, chain2Duration * 0.5f).SetEase(Ease.InQuad)
                    )
                    .OnComplete(StartBreathing);
                break;
            }
            case 3: // 내려찍기: 솟구치기 → 적 위치로 급강하 → 복귀
            {
                Vector3 peak = new Vector3(cur.x, cur.y + slamHeight, cur.z);
                Vector3 slamPos = new Vector3(targetLocal.x, _idlePos.y, _idlePos.z);
                DOTween
                    .Sequence()
                    .Append(transform.DOLocalMove(peak, slamUpDuration).SetEase(Ease.OutQuad))
                    .Append(transform.DOLocalMove(slamPos, slamDownDuration).SetEase(Ease.InQuart))
                    .Append(transform.DOLocalMove(_idlePos, returnDuration).SetEase(Ease.InOutSine))
                    .OnComplete(StartBreathing);
                break;
            }
        }
    }

    // Chain Mastery: 3체인 시 데미지 +30%
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        if (group.Length >= 3)
            return Mathf.RoundToInt(damage * 1.3f);
        return damage;
    }
}
