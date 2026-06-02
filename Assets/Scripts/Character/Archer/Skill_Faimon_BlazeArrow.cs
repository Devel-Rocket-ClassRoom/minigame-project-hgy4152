using System.Collections;
using UnityEngine;

public class Skill_Faimon_BlazeArrow : Skill
{
    [SerializeField]
    GameObject bigEffectPrefab;

    [SerializeField]
    float moveDuration = 0.18f;

    [SerializeField]
    float bigArrowDelay = 0.15f;

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

    // 1·2체인: 불화살 발사
    public override void Chain1(Vector3 targetPos, float scaleFactor) =>
        FireArrow(targetPos, scaleFactor);

    public override void Chain2(Vector3 targetPos, float scaleFactor) =>
        FireArrow(targetPos, scaleFactor);

    // 3체인: 불화살 + 점프 후 새 형상의 큰 불 발사
    public override void Chain3(Vector3 targetPos, float scaleFactor) =>
        StartCoroutine(BlazeArrow(targetPos, scaleFactor));

    void FireArrow(Vector3 targetPos, float scale)
    {
        if (effectPrefab == null)
            return;
        var go = Instantiate(effectPrefab, transform.position, Quaternion.identity);
        go.transform.localScale = Vector3.one * scale;
        StartCoroutine(MoveTo(go, targetPos));
    }

    IEnumerator BlazeArrow(Vector3 targetPos, float scaleFactor)
    {
        FireArrow(targetPos, scaleFactor);

        yield return new WaitForSeconds(bigArrowDelay);

        if (bigEffectPrefab != null)
        {
            var big = Instantiate(bigEffectPrefab, transform.position, Quaternion.identity);
            big.transform.localScale = Vector3.one * scaleFactor * 1.5f;
            StartCoroutine(MoveTo(big, targetPos));
        }
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
