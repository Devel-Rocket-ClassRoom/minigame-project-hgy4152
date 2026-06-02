using System.Collections;
using UnityEngine;

public class Skill_AhnMansik : Skill
{
    [SerializeField]
    float spawnHeight = 6f;

    [SerializeField]
    float fallDuration = 0.2f;

    [SerializeField]
    float grenadeInterval = 0.12f;

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

    // 1체인: 수류탄 1개
    public override void Chain1(Vector3 targetPos, float scaleFactor) =>
        StartCoroutine(ThrowGrenades(targetPos, scaleFactor, 1));

    // 2체인: 수류탄 2개
    public override void Chain2(Vector3 targetPos, float scaleFactor) =>
        StartCoroutine(ThrowGrenades(targetPos, scaleFactor, 2));

    // 3체인: 수류탄 3개
    public override void Chain3(Vector3 targetPos, float scaleFactor) =>
        StartCoroutine(ThrowGrenades(targetPos, scaleFactor, 3));

    IEnumerator ThrowGrenades(Vector3 targetPos, float scaleFactor, int count)
    {
        if (effectPrefab == null)
            yield break;

        for (int i = 0; i < count; i++)
        {
            float offsetX = (i - (count - 1) * 0.5f) * 0.4f;
            Vector3 spawnPos = targetPos + new Vector3(offsetX, spawnHeight, 0f);
            var go = Instantiate(effectPrefab, spawnPos, Quaternion.identity);
            go.transform.localScale = Vector3.one * scaleFactor;
            StartCoroutine(Fall(go, spawnPos, targetPos + new Vector3(offsetX, 0f, 0f)));

            if (i < count - 1)
                yield return new WaitForSeconds(grenadeInterval);
        }
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
}
