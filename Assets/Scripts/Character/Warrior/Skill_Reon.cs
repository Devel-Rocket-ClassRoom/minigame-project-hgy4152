using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Skill_Reon : Skill
{
    [SerializeField]
    float spawnOffset = 12f;

    [SerializeField]
    float moveDuration = 0.15f;

    [SerializeField]
    GameObject hitEffectPrefab;

    [Header("=== 캐릭터 이동 ===")]
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

    List<GameObject> _activeEffects = new();

    ReonCharacter _character;

    void Awake() => _character = GetComponent<ReonCharacter>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Chain1(testPos, 1f);
        if (Input.GetKeyDown(KeyCode.W))
            Chain2(testPos, 1.5f);
        if (Input.GetKeyDown(KeyCode.E))
            Chain3(testPos, 2f);
    }

    public override void Chain1(Vector3 targetPos, float scaleFactor) =>
        StartCoroutine(Chain1Routine(targetPos, scaleFactor));

    public override void Chain2(Vector3 targetPos, float scaleFactor) =>
        StartCoroutine(Chain2Routine(targetPos, scaleFactor));

    public override void Chain3(Vector3 targetPos, float scaleFactor) =>
        StartCoroutine(Chain3Routine(targetPos, scaleFactor));

    IEnumerator Chain1Routine(Vector3 targetPos, float scaleFactor)
    {
        Vector3 idlePos = _character.IdlePos;
        Vector3 targetLocal =
            transform.parent != null
                ? transform.parent.InverseTransformPoint(targetPos)
                : targetPos;
        float dx = targetLocal.x - idlePos.x;
        Vector3 cur = transform.localPosition;
        Vector3 peak = new Vector3(cur.x + dx * chain1XFrac, cur.y + chain1Height, cur.z);

        DOTween.Kill(transform);
        Sequence seq = DOTween
            .Sequence()
            .SetTarget(transform)
            .Append(transform.DOLocalMove(peak, chain1Duration * 0.5f).SetEase(Ease.OutQuad));

        Vector3 hitPos = targetPos;
        targetPos.x -= 1f;
        if (effectPrefab != null)
        {
            Vector3 startPos = targetPos + new Vector3(-spawnOffset, spawnOffset, 0);
            var go = Instantiate(effectPrefab);
            go.transform.position = startPos;
            _activeEffects.Add(go);
            StartCoroutine(MoveTo(go, startPos, targetPos, hitPos));
        }

        yield return seq.WaitForCompletion();
        _character.TryReturnAfterChain(DestroyActiveEffects);
    }

    // Chain1 완료 위치에서 chain2 이어서 실행
    IEnumerator Chain2Routine(Vector3 targetPos, float scaleFactor)
    {
        yield return StartCoroutine(Chain1Routine(targetPos, scaleFactor));

        Vector3 idlePos = _character.IdlePos;
        Vector3 targetLocal =
            transform.parent != null
                ? transform.parent.InverseTransformPoint(targetPos)
                : targetPos;
        float dx = targetLocal.x - idlePos.x;
        Vector3 cur = transform.localPosition;
        Vector3 peak = new Vector3(cur.x + dx * chain2XFrac, cur.y + chain2Height, cur.z);

        DOTween.Kill(transform);
        Sequence seq = DOTween
            .Sequence()
            .SetTarget(transform)
            .Append(transform.DOLocalMove(peak, chain2Duration * 0.5f).SetEase(Ease.OutQuad));

        Vector3 hitPos = targetPos;
        targetPos.x += 0.5f;
        if (effectPrefab != null)
        {
            Vector3 startPos = targetPos + new Vector3(spawnOffset, spawnOffset, 0);
            var go = Instantiate(effectPrefab);
            go.transform.position = startPos;
            go.GetComponent<SpriteRenderer>().flipY = true;
            _activeEffects.Add(go);
            StartCoroutine(MoveTo(go, startPos, targetPos, hitPos));
        }

        yield return seq.WaitForCompletion();
        _character.TryReturnAfterChain(DestroyActiveEffects);
    }

    // Chain2 완료 위치에서 슬램 후 idle 복귀
    IEnumerator Chain3Routine(Vector3 targetPos, float scaleFactor)
    {
        yield return StartCoroutine(Chain2Routine(targetPos, scaleFactor));

        Vector3 idlePos = _character.IdlePos;
        Vector3 targetLocal =
            transform.parent != null
                ? transform.parent.InverseTransformPoint(targetPos)
                : targetPos;
        Vector3 cur = transform.localPosition;
        Vector3 peak = new Vector3(cur.x, cur.y + slamHeight, cur.z);
        Vector3 slamPos = new Vector3(targetLocal.x, idlePos.y, idlePos.z);

        Vector3 hitPos = targetPos;
        targetPos.x -= 0.25f;
        targetPos.y = 0f;
        Vector3 startPos3 = targetPos + new Vector3(0, spawnOffset + 3f, 0);

        DOTween.Kill(transform);
        DOTween
            .Sequence()
            .SetTarget(transform)
            .Append(transform.DOLocalMove(peak, slamUpDuration).SetEase(Ease.OutQuad))
            .Append(transform.DOLocalMove(slamPos, slamDownDuration).SetEase(Ease.InQuart))
            .AppendCallback(() =>
            {
                if (effectPrefab == null)
                    return;
                var go3 = Instantiate(effectPrefab, startPos3, Quaternion.Euler(0, 0, -135f));
                go3.transform.localScale *= scaleFactor * 1.2f;
                _activeEffects.Add(go3);
                StartCoroutine(MoveTo(go3, startPos3, targetPos, hitPos));
            })
            .Append(transform.DOLocalMove(idlePos, returnDuration).SetEase(Ease.InOutSine))
            .OnComplete(() =>
            {
                _character.StartBreathing();
                DestroyActiveEffects();
            });
    }

    IEnumerator MoveTo(GameObject go, Vector3 start, Vector3 target, Vector3 hitPos)
    {
        float t = 0f;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            if (go == null)
                yield break;
            float ratio = t / moveDuration;
            go.transform.position = Vector3.Lerp(start, target, ratio * ratio);
            yield return null;
        }
        if (go != null)
        {
            go.transform.position = target;
            SpawnParticleEffect(hitEffectPrefab, hitPos);
        }
    }

    void DestroyActiveEffects()
    {
        foreach (var go in _activeEffects)
            if (go != null)
                Destroy(go);
        _activeEffects.Clear();
    }

    void SpawnParticleEffect(GameObject prefab, Vector3 pos)
    {
        if (prefab == null)
            return;
        var fx = Instantiate(prefab, pos, Quaternion.identity);
        var ps = fx.GetComponent<ParticleSystem>();
        Destroy(fx, ps != null ? ps.main.duration + 1f : 2f);
    }
}
