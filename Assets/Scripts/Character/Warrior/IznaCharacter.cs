using DG.Tweening;
using UnityEngine;

public class IznaCharacter : Character
{
    [Header("=== 체인 이동 ===")]
    [SerializeField]
    float chain1DashFrac = 0.55f;

    [SerializeField]
    float chain1Duration = 0.14f;

    [SerializeField]
    float chain2Height = 0.45f;

    [SerializeField]
    float chain2XOffset = -0.35f;

    [SerializeField]
    float chain2Duration = 0.24f;

    [SerializeField]
    float chain3Height = 0.65f;

    [SerializeField]
    float chain3UpDuration = 0.14f;

    [SerializeField]
    float chain3SpinDuration = 0.12f;

    [SerializeField]
    float chain3DownDuration = 0.13f;

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

        switch (_hitEventIndex)
        {
            case 1: // 빠르게 적을 베고 지나감
            {
                float dx = targetLocal.x - _idlePos.x;
                Vector3 dashPos = new Vector3(
                    _idlePos.x + dx * chain1DashFrac,
                    _idlePos.y,
                    _idlePos.z
                );
                DOTween
                    .Sequence()
                    .Append(transform.DOLocalMove(dashPos, chain1Duration).SetEase(Ease.OutQuint))
                    .Append(transform.DOLocalMove(_idlePos, returnDuration).SetEase(Ease.InOutSine))
                    .OnComplete(StartBreathing);
                break;
            }
            case 2: // 좌대각으로 솟구치면서 올려베기
            {
                Vector3 peak = new Vector3(cur.x + chain2XOffset, cur.y + chain2Height, cur.z);
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
            case 3: // 적 x좌표까지 우대각 상승 → 회전 → 내려찍기
            {
                Vector3 risePos = new Vector3(targetLocal.x, cur.y + chain3Height, cur.z);
                Vector3 slamPos = new Vector3(targetLocal.x, _idlePos.y, _idlePos.z);
                DOTween
                    .Sequence()
                    .Append(transform.DOLocalMove(risePos, chain3UpDuration).SetEase(Ease.OutQuad))
                    .Join(
                        transform.DOLocalRotate(
                            new Vector3(0, 0, 360),
                            chain3SpinDuration + chain3UpDuration,
                            RotateMode.FastBeyond360
                        )
                    )
                    .Append(
                        transform.DOLocalMove(slamPos, chain3DownDuration).SetEase(Ease.InQuart)
                    )
                    .Append(transform.DOLocalMove(_idlePos, returnDuration).SetEase(Ease.InOutSine))
                    .OnComplete(StartBreathing);
                break;
            }
        }
    }

    // 연격: 1·2·3체인 모두 있을 때 데미지 +40%
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        if (judge.chain1Count > 0 && judge.chain2Count > 0 && judge.chain3Count > 0)
            return Mathf.RoundToInt(damage * 1.4f);
        return damage;
    }
}
