using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Skill_Raven : Skill
{
    [SerializeField]
    float moveDuration = 0.25f;

    [SerializeField]
    Transform muzzle;

    [SerializeField]
    float muzzleFlashOffsetX;

    [SerializeField]
    float bulletSpread = 0.4f;

    [Header("=== 반동 ===")]
    [SerializeField]
    float recoilAngle = 7.261f;

    [SerializeField]
    float recoilDuration = 0.08f;

    [SerializeField]
    float returnDuration = 0.18f;

    [SerializeField]
    GameObject muzzleFlashPrefab;

    [SerializeField]
    GameObject hitEffectPrefab;

    Character _character;

    void Awake() => _character = GetComponent<Character>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
            Chain1(testPos, 1f);
        if (Input.GetKeyDown(KeyCode.I))
        {
            Chain1(testPos, 1.5f);
            Chain2(testPos, 1.5f);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Chain1(testPos, 2f);
            Chain2(testPos, 2f);
            Chain3(testPos, 2f);
        }
    }

    public override void Chain1(Vector3 targetPos, float scaleFactor) =>
        Shoot(targetPos, scaleFactor);

    public override void Chain2(Vector3 targetPos, float scaleFactor)
    {
        Shoot(targetPos + Vector3.up * bulletSpread * 0.5f, scaleFactor);
        Shoot(targetPos - Vector3.up * bulletSpread * 0.5f, scaleFactor);
    }

    public override void Chain3(Vector3 targetPos, float scaleFactor)
    {
        Shoot(targetPos + Vector3.up * bulletSpread, scaleFactor);
        Shoot(targetPos, scaleFactor);
        Shoot(targetPos - Vector3.up * bulletSpread, scaleFactor);
    }

    void Shoot(Vector3 targetPos, float scale)
    {
        if (effectPrefab == null)
            return;

        DOTween.Kill(transform);
        transform.localRotation = Quaternion.identity;
        DOTween
            .Sequence()
            .SetTarget(transform)
            .Append(
                transform
                    .DOLocalRotate(new Vector3(0, 0, recoilAngle), recoilDuration)
                    .SetEase(Ease.OutQuad)
            )
            .Append(transform.DOLocalRotate(Vector3.zero, returnDuration).SetEase(Ease.InOutSine))
            .OnComplete(_character.StartBreathing);

        Vector3 start = muzzle != null ? muzzle.position : transform.position;
        Vector3 dir = (targetPos - start).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        SpawnParticleEffect(muzzleFlashPrefab, start + new Vector3(muzzleFlashOffsetX, 0f, 0f));
        var go = Instantiate(effectPrefab, start, Quaternion.Euler(0, 0, angle));
        go.transform.localScale = Vector3.one * scale;
        StartCoroutine(MoveTo(go, start, targetPos));
    }

    IEnumerator MoveTo(GameObject go, Vector3 start, Vector3 target)
    {
        float t = 0f;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            if (go == null)
                yield break;
            go.transform.position = Vector3.Lerp(start, target, t / moveDuration);
            yield return null;
        }
        if (go != null)
        {
            SpawnParticleEffect(hitEffectPrefab, target);
            Destroy(go);
        }
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
