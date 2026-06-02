using System.Collections;
using UnityEngine;

public class Skill_Skadi_GlacialArrow : Skill
{
    [SerializeField]
    float moveDuration = 0.18f;

    [SerializeField]
    float spiralOffset = 0.4f;

    [SerializeField]
    float bigArrowDelay = 0.12f;

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
        FireArrow(targetPos, scaleFactor, Vector3.zero);

    // 2체인: 2발 나선형 발사
    public override void Chain2(Vector3 targetPos, float scaleFactor)
    {
        FireArrow(targetPos, scaleFactor, new Vector3(0, spiralOffset, 0));
        FireArrow(targetPos, scaleFactor, new Vector3(0, -spiralOffset, 0));
    }

    // 3체인: 나선 2발 + 기를 모아 큰 화살 1발
    public override void Chain3(Vector3 targetPos, float scaleFactor) =>
        StartCoroutine(GlacialArrow(targetPos, scaleFactor));

    void FireArrow(Vector3 targetPos, float scale, Vector3 spawnOffset)
    {
        if (effectPrefab == null)
            return;
        var go = Instantiate(effectPrefab, transform.position + spawnOffset, Quaternion.identity);
        go.transform.localScale = Vector3.one * scale;
        StartCoroutine(MoveTo(go, targetPos));
    }

    IEnumerator GlacialArrow(Vector3 targetPos, float scaleFactor)
    {
        FireArrow(targetPos, scaleFactor, new Vector3(0, spiralOffset, 0));
        FireArrow(targetPos, scaleFactor, new Vector3(0, -spiralOffset, 0));

        yield return new WaitForSeconds(bigArrowDelay);

        // 큰 화살
        if (effectPrefab == null)
            yield break;
        var big = Instantiate(effectPrefab, transform.position, Quaternion.identity);
        big.transform.localScale = Vector3.one * scaleFactor * 1.5f;
        StartCoroutine(MoveTo(big, targetPos));
    }

    IEnumerator MoveTo(GameObject go, Vector3 targetPos)
    {
        Vector3 start = go.transform.position;
        float t = 0f;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            if (go == null)
                yield break;
            go.transform.position = Vector3.Lerp(start, targetPos, t / moveDuration);
            yield return null;
        }
        if (go != null)
        {
            go.transform.position = targetPos;
            Destroy(go, 0.1f);
        }
    }
}
