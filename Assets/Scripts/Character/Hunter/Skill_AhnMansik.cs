using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class Skill_AhnMansik : Skill
{
    [SerializeField]
    Sprite grenadeSprite;

    [SerializeField]
    float throwDuration = 0.4f;

    [SerializeField]
    float arcHeightMin = 2f;

    [SerializeField]
    float arcHeightMax = 5f;

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

    public override void Chain1(Vector3 targetPos, float scaleFactor) => ThrowGrenade(targetPos);

    public override void Chain2(Vector3 targetPos, float scaleFactor) => ThrowGrenade(targetPos);

    public override void Chain3(Vector3 targetPos, float scaleFactor) => ThrowGrenade(targetPos);

    void ThrowGrenade(Vector3 targetPos)
    {
        float arcHeight = Random.Range(arcHeightMin, arcHeightMax);
        var go = new GameObject("Grenade");
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = grenadeSprite;
        go.transform.position = transform.position;
        ThrowAsync(go, transform.position, targetPos, arcHeight, this.GetCancellationTokenOnDestroy())
            .Forget();
    }

    async UniTaskVoid ThrowAsync(
        GameObject go,
        Vector3 start,
        Vector3 target,
        float arcHeight,
        CancellationToken ct
    )
    {
        float t = 0f;
        while (t < throwDuration)
        {
            t += Time.deltaTime;
            if (go == null)
                return;
            float ratio = Mathf.Clamp01(t / throwDuration);
            go.transform.position =
                Vector3.Lerp(start, target, ratio)
                + Vector3.up * (arcHeight * Mathf.Sin(Mathf.PI * ratio));
            await UniTask.Yield(PlayerLoopTiming.Update, ct);
        }
        if (go != null)
        {
            go.transform.position = target;
            Destroy(go);

            if (effectPrefab != null)
            {
                var effect = Instantiate(effectPrefab, target, Quaternion.identity);
                DestroyAfterAnimation(effect, 0.5f);
            }
        }
    }

    void DestroyAfterAnimation(GameObject go, float fallback)
    {
        var anim = go.GetComponentInChildren<Animator>();
        if (anim != null && anim.runtimeAnimatorController != null)
        {
            var clips = anim.runtimeAnimatorController.animationClips;
            if (clips.Length > 0)
            {
                Destroy(go, clips[0].length);
                return;
            }
        }
        Destroy(go, fallback);
    }
}
