using System.Collections;
using UnityEngine;

public class Skill_Tirion : Skill
{
    [SerializeField]
    GameObject stormEffectPrefab;

    [SerializeField]
    float spawnHeight = 10f;

    [SerializeField]
    float fallDuration = 0.2f;

    [SerializeField]
    float stormDelay = 0.15f;

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

    // 1·2체인: 망치 낙하
    public override void Chain1(Vector3 targetPos, float scaleFactor) =>
        DropHammer(targetPos, scaleFactor);

    public override void Chain2(Vector3 targetPos, float scaleFactor) =>
        DropHammer(targetPos, scaleFactor);

    // 3체인: 망치 낙하 후 신성폭풍
    public override void Chain3(Vector3 targetPos, float scaleFactor) =>
        StartCoroutine(LightsVerdict(targetPos, scaleFactor));

    void DropHammer(Vector3 targetPos, float scale)
    {
        if (effectPrefab == null)
            return;
        Vector3 startPos = targetPos + new Vector3(0, spawnHeight, 0);
        var go = Instantiate(effectPrefab, startPos, Quaternion.identity);
        go.transform.localScale = Vector3.one * scale;
        StartCoroutine(Fall(go, startPos, targetPos));
    }

    IEnumerator Fall(GameObject go, Vector3 start, Vector3 target)
    {
        float t = 0f;
        while (t < fallDuration)
        {
            t += Time.deltaTime;
            if (go == null)
                yield break;
            float ratio = t / fallDuration;
            go.transform.position = Vector3.Lerp(start, target, ratio * ratio);
            yield return null;
        }
        if (go != null)
        {
            go.transform.position = target;
            Destroy(go, 0.4f);
        }
    }

    IEnumerator LightsVerdict(Vector3 targetPos, float scaleFactor)
    {
        DropHammer(targetPos, scaleFactor);
        yield return new WaitForSeconds(fallDuration + stormDelay);

        if (stormEffectPrefab != null)
        {
            var storm = Instantiate(stormEffectPrefab, targetPos, Quaternion.identity);
            storm.transform.localScale = Vector3.one * scaleFactor * 1.5f;
            Destroy(storm, 0.8f);
        }
    }
}
