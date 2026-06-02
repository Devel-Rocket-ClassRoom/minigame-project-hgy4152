using System.Collections;
using UnityEngine;

public class Skill_Cain : Skill
{
    [SerializeField]
    float spawnHeight = 8f;

    [SerializeField]
    float slamDuration = 0.2f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            Chain1(testPos, 1f);

        if (Input.GetKeyDown(KeyCode.W))
        {
            Chain1(testPos, 1.5f);
            Chain2(testPos, 1.5f);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Chain1(testPos, 2f);
            Chain2(testPos, 2f);
            Chain3(testPos, 2f);
        }
    }

    // 체인별 크기가 다른 강력한 내려찍기
    public override void Chain1(Vector3 targetPos, float scaleFactor) =>
        Slam(targetPos, scaleFactor * 0.8f);

    public override void Chain2(Vector3 targetPos, float scaleFactor) =>
        Slam(targetPos, scaleFactor);

    public override void Chain3(Vector3 targetPos, float scaleFactor) =>
        Slam(targetPos, scaleFactor * 1.3f);

    void Slam(Vector3 targetPos, float scale)
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
        while (t < slamDuration)
        {
            t += Time.deltaTime;
            if (go == null)
                yield break;
            float ratio = t / slamDuration;
            go.transform.position = Vector3.Lerp(start, target, ratio * ratio);
            yield return null;
        }
        if (go != null)
        {
            go.transform.position = target;
            Destroy(go, 0.4f);
        }
    }
}
