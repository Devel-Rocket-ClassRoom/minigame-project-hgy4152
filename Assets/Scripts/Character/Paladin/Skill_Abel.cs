using System.Collections;
using UnityEngine;

public class Skill_Abel : Skill
{
    [SerializeField]
    GameObject chargeEffectPrefab;

    [SerializeField]
    float explosionDuration = 0.5f;

    [SerializeField]
    float gatherDuration = 0.35f;

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

    // 1·2체인: 폭발
    public override void Chain1(Vector3 targetPos, float scaleFactor) =>
        Explode(targetPos, scaleFactor);

    public override void Chain2(Vector3 targetPos, float scaleFactor) =>
        Explode(targetPos, scaleFactor);

    // 3체인: 폭발 → 에너지 수렴 → 더 큰 폭발
    public override void Chain3(Vector3 targetPos, float scaleFactor) =>
        StartCoroutine(BigExplode(targetPos, scaleFactor));

    void Explode(Vector3 pos, float scale)
    {
        if (effectPrefab == null)
            return;
        var go = Instantiate(effectPrefab, pos, Quaternion.identity);
        go.transform.localScale = Vector3.one * scale;
        Destroy(go, explosionDuration);
    }

    IEnumerator BigExplode(Vector3 targetPos, float scaleFactor)
    {
        Explode(targetPos, scaleFactor * 0.7f);
        yield return new WaitForSeconds(0.15f);

        // 에너지 수렴 (역재생 연출)
        if (chargeEffectPrefab != null)
        {
            var gather = Instantiate(chargeEffectPrefab, targetPos, Quaternion.identity);
            gather.transform.localScale = Vector3.one * scaleFactor;
            Destroy(gather, gatherDuration);
            yield return new WaitForSeconds(gatherDuration);
        }

        Explode(targetPos, scaleFactor * 1.6f);
    }
}
