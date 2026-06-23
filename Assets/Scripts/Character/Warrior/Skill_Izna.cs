using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Skill_Izna : Skill
{
    [SerializeField]
    float moveDuration = 0.12f;

    [SerializeField]
    Vector3 effectTargetOffset = new Vector3(1f, 0f, 0f);

    [SerializeField]
    float chain2EffectAngleOffset = 0f;

    [SerializeField]
    GameObject chain3EffectPrefab;

    [SerializeField]
    GameObject chain3HitEffect1;

    [SerializeField]
    float chain3HitEffect1Speed = 1f;

    [SerializeField]
    GameObject chain3HitEffect2;

    [SerializeField]
    float chain3HitEffect2Speed = 1f;

    [SerializeField]
    float chain3HitEffect2Delay = 0.5f;

    [Header("=== 캐릭터 이동 ===")]
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

    Character _character;

    void Awake() => _character = GetComponent<Character>();

    static readonly float[] TestTimings = { 0.15f, 0.35f, 0.55f };

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            TestChainAsync(1, this.GetCancellationTokenOnDestroy()).Forget();

        if (Input.GetKeyDown(KeyCode.W))
            TestChainAsync(2, this.GetCancellationTokenOnDestroy()).Forget();

        if (Input.GetKeyDown(KeyCode.E))
            TestChainAsync(3, this.GetCancellationTokenOnDestroy()).Forget();
    }

    async UniTaskVoid TestChainAsync(int chainCount, CancellationToken ct)
    {
        float scaleFactor = chainCount switch
        {
            1 => 1f,
            2 => 1.5f,
            _ => 2f,
        };
        float prev = 0f;
        for (int i = 0; i < chainCount; i++)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(TestTimings[i] - prev), cancellationToken: ct);
            prev = TestTimings[i];
            switch (i + 1)
            {
                case 1:
                    Chain1(testPos, scaleFactor);
                    break;
                case 2:
                    Chain2(testPos, scaleFactor);
                    break;
                case 3:
                    Chain3(testPos, scaleFactor);
                    break;
            }
        }
    }

    // 1체인: 횡으로 빠른 참격
    public override void Chain1(Vector3 targetPos, float scaleFactor)
    {
        if (effectPrefab != null)
        {
            float dist = Vector3.Distance(transform.position, targetPos);
            var go = Instantiate(effectPrefab, targetPos + effectTargetOffset, Quaternion.identity);
            go.transform.localScale = Vector3.one * dist;
            Destroy(go, moveDuration + 0.3f);
        }
        DOTween.Kill(transform);
        Vector3 idlePos = _character.IdlePos;
        Vector3 targetLocal =
            transform.parent != null
                ? transform.parent.InverseTransformPoint(targetPos)
                : targetPos;
        float dx = targetLocal.x - idlePos.x;
        Vector3 dashPos = new Vector3(idlePos.x + dx * chain1DashFrac, idlePos.y, idlePos.z);
        DOTween
            .Sequence()
            .SetTarget(transform)
            .Append(transform.DOLocalMove(dashPos, chain1Duration).SetEase(Ease.OutQuint))
            .Append(transform.DOLocalMove(idlePos, returnDuration).SetEase(Ease.InOutSine))
            .OnComplete(_character.StartBreathing);
    }

    // 2체인: 좌대각 상승 참격
    public override void Chain2(Vector3 targetPos, float scaleFactor)
    {
        DOTween.Kill(transform);
        transform.localEulerAngles = new Vector3(0, -180, 15);
        if (effectPrefab != null)
        {
            float dist = Vector3.Distance(transform.position, targetPos);
            Vector3 dir = (targetPos - transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + chain2EffectAngleOffset;
            var go = Instantiate(
                effectPrefab,
                transform.position + effectTargetOffset,
                Quaternion.Euler(0, 0, angle)
            );
            go.transform.localScale = Vector3.one * dist;
            var ps = go.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.startRotation = angle * Mathf.Deg2Rad;
            }
            Destroy(go, moveDuration + 0.3f);
        }
        Vector3 idlePos2 = _character.IdlePos;
        Vector3 cur = transform.localPosition;
        Vector3 peak = new Vector3(cur.x + chain2XOffset, cur.y + chain2Height, cur.z);
        DOTween
            .Sequence()
            .SetTarget(transform)
            .Append(transform.DOLocalMove(peak, chain2Duration * 0.5f).SetEase(Ease.OutQuad))
            .Append(transform.DOLocalMove(idlePos2, chain2Duration * 0.5f).SetEase(Ease.InQuad))
            .Join(transform.DOLocalRotate(Vector3.zero, chain2Duration * 0.5f).SetEase(Ease.InQuad))
            .OnComplete(_character.StartBreathing);
    }

    // 3체인: 우대각 상승 후 회전 내려찍기
    public override void Chain3(Vector3 targetPos, float scaleFactor)
    {
        DOTween.Kill(transform);
        transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z);
        Vector3 idlePos3 = _character.IdlePos;
        Vector3 cur3 = transform.localPosition;
        Vector3 targetLocal3 =
            transform.parent != null
                ? transform.parent.InverseTransformPoint(targetPos)
                : targetPos;
        Vector3 risePos = new Vector3(targetLocal3.x, cur3.y + chain3Height, cur3.z);
        Vector3 slamPos = new Vector3(targetLocal3.x, idlePos3.y, idlePos3.z);
        var seq = DOTween
            .Sequence()
            .SetTarget(transform)
            .Append(transform.DOLocalMove(risePos, chain3UpDuration).SetEase(Ease.OutQuad))
            .AppendCallback(() =>
            {
                if (chain3EffectPrefab != null)
                {
                    var go = Instantiate(
                        chain3EffectPrefab,
                        transform.position,
                        Quaternion.identity
                    );
                    Destroy(go, chain3SpinDuration + chain3DownDuration);
                }
            })
            .AppendInterval(chain3SpinDuration)
            .AppendCallback(() =>
            {
                if (chain3HitEffect1 != null)
                {
                    Vector3 effect1Pos = new Vector3(
                        targetPos.x,
                        (targetPos.y + transform.position.y) * 0.5f,
                        targetPos.z
                    );
                    SpawnAnimEffect(chain3HitEffect1, effect1Pos, chain3HitEffect1Speed);
                }
                if (chain3HitEffect2 != null)
                    SpawnDelayedAsync(
                            chain3HitEffect2,
                            targetPos,
                            chain3HitEffect2Delay,
                            chain3HitEffect2Speed,
                            this.GetCancellationTokenOnDestroy()
                        )
                        .Forget();
            })
            .Append(transform.DOLocalMove(slamPos, chain3DownDuration).SetEase(Ease.InQuart))
            .Join(transform.DOLocalRotate(Vector3.zero, chain3DownDuration))
            .Append(transform.DOLocalMove(idlePos3, returnDuration).SetEase(Ease.InOutSine))
            .OnComplete(_character.StartBreathing);
        seq.Insert(
            chain3UpDuration,
            transform.DOLocalRotate(
                new Vector3(0, 0, 360),
                chain3SpinDuration,
                RotateMode.FastBeyond360
            )
        );
    }

    void SpawnAnimEffect(GameObject prefab, Vector3 pos, float speed = 1f)
    {
        var go = Instantiate(prefab, pos, Quaternion.identity);
        var anim = go.GetComponent<Animator>();
        if (anim != null && anim.runtimeAnimatorController != null)
        {
            anim.speed = speed;
            var clips = anim.runtimeAnimatorController.animationClips;
            if (clips.Length > 0)
                Destroy(go, clips[0].length / speed);
        }
    }

    async UniTaskVoid SpawnDelayedAsync(
        GameObject prefab,
        Vector3 pos,
        float delay,
        float speed,
        CancellationToken ct
    )
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: ct);
        SpawnAnimEffect(prefab, pos, speed);
    }
}
