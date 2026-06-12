using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Skill_CaptainJG : Skill
{
    [SerializeField]
    float spawnOffsetX = 3f;

    [SerializeField]
    float spawnYMin = 1f;

    [SerializeField]
    float spawnYMax = 3f;

    [SerializeField]
    float unitSpawnInterval = 0.1f;

    [SerializeField]
    float unitTravelDuration = 0.25f;

    [SerializeField]
    Sprite[] unitSprites;

    [SerializeField]
    GameObject hitEffectPrefab;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
            Chain1(testPos, 1f);

        if (Input.GetKeyDown(KeyCode.K))
        {
            Chain1(testPos, 1.5f);
            Chain2(testPos, 1.5f);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Chain1(testPos, 2f);
            Chain2(testPos, 2f);
            Chain3(testPos, 2f);
        }
    }

    // 1체인: 파도부대 2명
    public override void Chain1(Vector3 targetPos, float scaleFactor) =>
        SpawnUnitsAsync(targetPos, scaleFactor, 2, this.GetCancellationTokenOnDestroy()).Forget();

    // 2체인: 파도부대 4명
    public override void Chain2(Vector3 targetPos, float scaleFactor) =>
        SpawnUnitsAsync(targetPos, scaleFactor, 4, this.GetCancellationTokenOnDestroy()).Forget();

    // 3체인: 파도부대 6명
    public override void Chain3(Vector3 targetPos, float scaleFactor) =>
        SpawnUnitsAsync(targetPos, scaleFactor, 6, this.GetCancellationTokenOnDestroy()).Forget();

    async UniTaskVoid SpawnUnitsAsync(
        Vector3 targetPos,
        float scaleFactor,
        int count,
        CancellationToken ct
    )
    {
        bool hasSprites = unitSprites != null && unitSprites.Length > 0;
        if (effectPrefab == null && !hasSprites)
            return;

        GameObject hitEffect =
            hitEffectPrefab != null
                ? SpawnEffect(hitEffectPrefab, targetPos, Quaternion.identity)
                : null;

        int[] remaining = { count };

        for (int i = 0; i < count; i++)
        {
            float randX = UnityEngine.Random.Range(-spawnOffsetX, spawnOffsetX);
            float randY = UnityEngine.Random.Range(spawnYMin, spawnYMax);
            Vector3 spawnPos = targetPos + new Vector3(randX, randY, 0f);

            GameObject go;
            bool pooled = effectPrefab != null;
            if (pooled)
            {
                go = SpawnEffect(effectPrefab, spawnPos, Quaternion.identity);
            }
            else
            {
                go = new GameObject("Unit");
                go.transform.position = spawnPos;
                go.AddComponent<SpriteRenderer>();
            }
            if (hasSprites)
            {
                var sr = go.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.sprite = unitSprites[UnityEngine.Random.Range(0, unitSprites.Length)];
            }

            MoveUnitAsync(
                    go,
                    spawnPos,
                    targetPos,
                    pooled,
                    () =>
                    {
                        if (--remaining[0] <= 0 && hitEffect != null)
                            ReleaseEffect(hitEffect);
                    },
                    ct
                )
                .Forget();

            await UniTask.Delay(TimeSpan.FromSeconds(unitSpawnInterval), cancellationToken: ct);
        }
    }

    async UniTaskVoid MoveUnitAsync(
        GameObject go,
        Vector3 from,
        Vector3 to,
        bool pooled,
        Action onDone,
        CancellationToken ct
    )
    {
        float t = 0f;
        while (t < unitTravelDuration)
        {
            t += Time.deltaTime;
            if (go == null)
                break;
            go.transform.position = Vector3.Lerp(from, to, Mathf.Clamp01(t / unitTravelDuration));
            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }
        if (go != null)
        {
            if (pooled)
                ReleaseEffect(go);
            else
                Destroy(go);
        }
        onDone?.Invoke();
    }
}
