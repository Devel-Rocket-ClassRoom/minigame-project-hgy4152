using DG.Tweening;
using UnityEngine;

public class CainCharacter : Character
{
    [Header("=== 체인 이동 ===")]
    [SerializeField]
    float windupDuration = 0.12f;

    [SerializeField]
    float jumpHeight = 0.8f;

    [SerializeField]
    float jumpUpDuration = 0.15f;

    [SerializeField]
    float slamDownDuration = 0.12f;

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

        Vector3 peak = new Vector3(targetLocal.x, cur.y + jumpHeight, cur.z);
        Vector3 slamPos = new Vector3(targetLocal.x, _idlePos.y, _idlePos.z);

        // 기를 모으는 시간 → 점프 → 내려찍기 → 복귀
        DOTween
            .Sequence()
            .AppendInterval(windupDuration)
            .Append(transform.DOLocalMove(peak, jumpUpDuration).SetEase(Ease.OutQuad))
            .Append(transform.DOLocalMove(slamPos, slamDownDuration).SetEase(Ease.InQuart))
            .Append(transform.DOLocalMove(_idlePos, returnDuration).SetEase(Ease.InOutSine))
            .OnComplete(StartBreathing);
    }

    // 폭주: 3체인 그룹이 3개 이상일 때 데미지 +50%
    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        if (judge.chain3Count >= 3)
            return Mathf.RoundToInt(damage * 1.5f);
        return damage;
    }
}
