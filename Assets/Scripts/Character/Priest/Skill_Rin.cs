using System.Collections;
using UnityEngine;

public class Skill_Rin : Skill
{
    [SerializeField]
    float talismanInterval = 0.1f;

    [SerializeField]
    float talismanTravelDuration = 0.3f;

    [SerializeField]
    float partyEffectDuration = 0.8f;

    [SerializeField]
    GameObject partyEffectPrefab;

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

    // 1체인: 부적 1개 발사
    public override void Chain1(Vector3 targetPos, float scaleFactor) =>
        StartCoroutine(ThrowTalismans(targetPos, scaleFactor, 1));

    // 2체인: 부적 2개 발사
    public override void Chain2(Vector3 targetPos, float scaleFactor) =>
        StartCoroutine(ThrowTalismans(targetPos, scaleFactor, 2));

    // 3체인: 파티 진영에 이펙트
    public override void Chain3(Vector3 targetPos, float scaleFactor)
    {
        if (partyEffectPrefab == null)
            return;
        var go = Instantiate(partyEffectPrefab, transform.position, Quaternion.identity);
        go.transform.localScale = Vector3.one * scaleFactor;
        Destroy(go, partyEffectDuration);
    }

    IEnumerator ThrowTalismans(Vector3 targetPos, float scaleFactor, int count)
    {
        if (effectPrefab == null)
            yield break;

        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(effectPrefab, transform.position, Quaternion.identity);
            go.transform.localScale = Vector3.one * scaleFactor;
            StartCoroutine(MoveTo(go, transform.position, targetPos));
            if (i < count - 1)
                yield return new WaitForSeconds(talismanInterval);
        }
    }

    IEnumerator MoveTo(GameObject go, Vector3 from, Vector3 to)
    {
        float t = 0f;
        while (t < talismanTravelDuration)
        {
            t += Time.deltaTime;
            if (go == null)
                yield break;
            float ratio = t / talismanTravelDuration;
            go.transform.position = Vector3.Lerp(from, to, ratio * ratio);
            yield return null;
        }
        if (go != null)
            Destroy(go, 0.2f);
    }
}
