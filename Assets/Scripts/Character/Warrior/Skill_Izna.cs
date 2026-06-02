using System.Collections;
using UnityEngine;

public class Skill_Izna : Skill
{
    [SerializeField]
    float moveDuration = 0.12f;

    [SerializeField]
    float spawnOffset = 10f;

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

    // 1체인: 횡으로 빠른 참격
    public override void Chain1(Vector3 targetPos, float scaleFactor)
    {
        if (effectPrefab == null)
            return;
        Vector3 startPos = targetPos + new Vector3(-spawnOffset, 0, 0);
        Vector3 endPos = targetPos + new Vector3(spawnOffset * 0.4f, 0, 0);
        var go = Instantiate(effectPrefab, startPos, Quaternion.identity);
        go.transform.localScale = Vector3.one * scaleFactor;
        StartCoroutine(MoveTo(go, startPos, endPos, moveDuration));
    }

    // 2체인: 좌대각 상승 참격
    public override void Chain2(Vector3 targetPos, float scaleFactor)
    {
        if (effectPrefab == null)
            return;
        Vector3 startPos = targetPos + new Vector3(-1f, -spawnOffset * 0.4f, 0);
        Vector3 endPos = targetPos + new Vector3(-spawnOffset * 0.3f, spawnOffset * 0.3f, 0);
        var go = Instantiate(effectPrefab, startPos, Quaternion.Euler(0, 0, 45f));
        go.transform.localScale = Vector3.one * scaleFactor;
        StartCoroutine(MoveTo(go, startPos, endPos, moveDuration));
    }

    // 3체인: 우대각 상승 후 회전 내려찍기
    public override void Chain3(Vector3 targetPos, float scaleFactor)
    {
        if (effectPrefab == null)
            return;
        Vector3 startPos = targetPos + new Vector3(spawnOffset * 0.3f, spawnOffset * 0.5f, 0);
        var go = Instantiate(effectPrefab, startPos, Quaternion.Euler(0, 0, -45f));
        go.transform.localScale = Vector3.one * scaleFactor * 1.3f;
        StartCoroutine(MoveTo(go, startPos, targetPos, moveDuration * 1.2f));
    }

    IEnumerator MoveTo(GameObject go, Vector3 start, Vector3 target, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            if (go == null)
                yield break;
            go.transform.position = Vector3.Lerp(start, target, t / duration);
            yield return null;
        }
        if (go != null)
        {
            go.transform.position = target;
            Destroy(go, 0.3f);
        }
    }
}
