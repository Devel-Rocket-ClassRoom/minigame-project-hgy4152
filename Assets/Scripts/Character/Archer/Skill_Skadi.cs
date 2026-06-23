using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Skill_Skadi : Skill
{
    [SerializeField]
    float moveDuration = 0.18f;

    [SerializeField]
    float spiralOffset = 0.4f;

    [SerializeField]
    float bigArrowDelay = 0.12f;

    [SerializeField]
    GameObject impactEffectPrefab;

    [SerializeField]
    float impactOffsetY = 0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            Chain1(testPos, 1f);

        if (Input.GetKeyDown(KeyCode.S))
        {
            Chain1(testPos, 1.5f);
            Chain2(testPos, 1.5f);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Chain1(testPos, 2f);
            Chain2(testPos, 2f);
            Chain3(testPos, 2f);
        }
    }

    // 1체인: 1발 직선 발사
    public override void Chain1(Vector3 targetPos, float scaleFactor) =>
        FireArrow(targetPos, Vector3.zero);

    // 2체인: 2발 나선형 발사
    public override void Chain2(Vector3 targetPos, float scaleFactor)
    {
        FireArrow(targetPos, new Vector3(0, spiralOffset, 0));
        FireArrow(targetPos, new Vector3(0, -spiralOffset, 0));
    }

    // 3체인: 나선 2발 + 기를 모아 큰 화살 1발
    public override void Chain3(Vector3 targetPos, float scaleFactor) =>
        GlacialArrowAsync(targetPos, scaleFactor, this.GetCancellationTokenOnDestroy()).Forget();

    void FireArrow(Vector3 targetPos, Vector3 spawnOffset)
    {
        if (effectPrefab == null)
            return;
        var go = SpawnEffect(effectPrefab, transform.position + spawnOffset, Quaternion.identity);
        MoveToAsync(go, targetPos, this.GetCancellationTokenOnDestroy()).Forget();
    }

    async UniTaskVoid GlacialArrowAsync(Vector3 targetPos, float scaleFactor, CancellationToken ct)
    {
        FireArrow(targetPos, new Vector3(0, spiralOffset, 0));
        FireArrow(targetPos, new Vector3(0, -spiralOffset, 0));

        await UniTask.Delay(TimeSpan.FromSeconds(bigArrowDelay), cancellationToken: ct);

        if (impactEffectPrefab == null)
            return;
        var impact = Instantiate(
            impactEffectPrefab,
            targetPos + new Vector3(0, impactOffsetY, 0),
            Quaternion.identity
        );
        impact.transform.localScale = Vector3.one * scaleFactor;
        var animator = impact.GetComponent<Animator>();
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            var clips = animator.runtimeAnimatorController.animationClips;
            if (clips.Length > 0)
                Destroy(impact, clips[0].length);
        }
    }

    async UniTaskVoid MoveToAsync(GameObject go, Vector3 targetPos, CancellationToken ct)
    {
        Vector3 start = go.transform.position;
        float t = 0f;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            if (go == null)
                return;
            go.transform.position = Vector3.Lerp(start, targetPos, t / moveDuration);
            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }
        if (go != null)
        {
            go.transform.position = targetPos;
            await UniTask.Delay(TimeSpan.FromSeconds(0.1f), cancellationToken: ct);
            if (go != null)
                ReleaseEffect(go);
        }
    }
}
