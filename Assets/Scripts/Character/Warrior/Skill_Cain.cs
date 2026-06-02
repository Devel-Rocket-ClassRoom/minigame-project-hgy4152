using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Skill_Cain : Skill
{
    [SerializeField]
    float windupDuration = 0.12f;

    [SerializeField]
    float arcHeight = 1.5f;

    [SerializeField]
    float slamDuration = 0.3f;

    [SerializeField]
    float returnDuration = 0.18f;

    [SerializeField]
    [Range(0.1f, 0.9f)]
    float arcPeakRatio = 0.35f;

    [SerializeField]
    float effectAnimSpeed = 1f;

    [SerializeField]
    float effectYOffset = 0f;

    Character _character;
    GameObject _currentEffect;
    Coroutine _slamCoroutine;

    void Awake() => _character = GetComponent<Character>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Chain1(testPos, 1f);

        if (Input.GetKeyDown(KeyCode.W))
            Chain1(testPos, 1.5f);

        if (Input.GetKeyDown(KeyCode.E))
            Chain1(testPos, 2f);
    }

    public override void Chain1(Vector3 targetPos, float scaleFactor) =>
        Slam(targetPos, scaleFactor);

    public override void Chain2(Vector3 targetPos, float scaleFactor) =>
        Slam(targetPos, scaleFactor);

    public override void Chain3(Vector3 targetPos, float scaleFactor) =>
        Slam(targetPos, scaleFactor);

    void Slam(Vector3 targetPos, float scale)
    {
        if (_slamCoroutine != null)
            StopCoroutine(_slamCoroutine);
        DOTween.Kill(transform);
        _slamCoroutine = StartCoroutine(SlamRoutine(targetPos, scale));
    }

    IEnumerator SlamRoutine(Vector3 targetPos, float scale)
    {
        yield return new WaitForSeconds(windupDuration);

        Vector3 startLocal = transform.localPosition;
        Vector3 targetLocal =
            transform.parent != null
                ? transform.parent.InverseTransformPoint(targetPos)
                : targetPos;

        float t = 0f;
        while (t < slamDuration)
        {
            t += Time.deltaTime;
            float r = Mathf.Clamp01(t / slamDuration);
            float x = Mathf.Lerp(startLocal.x, targetLocal.x, r);
            float y = Mathf.Lerp(startLocal.y, targetLocal.y, r) + 4f * arcHeight * r * (1f - r);
            transform.localPosition = new Vector3(x, y, startLocal.z);
            yield return null;
        }
        transform.localPosition = new Vector3(targetLocal.x, targetLocal.y, startLocal.z);

        SpawnImpactEffect(targetPos, scale);

        _slamCoroutine = null;
        transform
            .DOLocalMove(_character.IdlePos, returnDuration)
            .SetTarget(transform)
            .SetEase(Ease.InOutSine)
            .OnComplete(_character.StartBreathing);
    }

    void SpawnImpactEffect(Vector3 pos, float scale)
    {
        if (_currentEffect != null)
            Destroy(_currentEffect);

        if (effectPrefab == null)
            return;

        var go = Instantiate(effectPrefab, pos + Vector3.up * effectYOffset, Quaternion.identity);
        Vector3 s = go.transform.localScale;
        go.transform.localScale = new Vector3(s.x * scale, s.y, s.z);
        _currentEffect = go;

        var anim = go.GetComponent<Animator>();
        if (anim != null)
        {
            anim.speed = effectAnimSpeed;
            var clips = anim.runtimeAnimatorController?.animationClips;
            if (clips != null && clips.Length > 0)
                Destroy(go, clips[0].length / effectAnimSpeed);
        }
    }
}
