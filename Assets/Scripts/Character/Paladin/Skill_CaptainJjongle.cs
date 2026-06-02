using System.Collections;
using UnityEngine;

public class Skill_CaptainJjongle : Skill
{
    [SerializeField]
    float spawnOffsetX = 3f;

    [SerializeField]
    float unitSpawnInterval = 0.1f;

    [SerializeField]
    float unitTravelDuration = 0.25f;

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

    // 1체인: 파도부대 2명
    public override void Chain1(Vector3 targetPos, float scaleFactor) =>
        StartCoroutine(SpawnUnits(targetPos, scaleFactor, 2));

    // 2체인: 파도부대 4명
    public override void Chain2(Vector3 targetPos, float scaleFactor) =>
        StartCoroutine(SpawnUnits(targetPos, scaleFactor, 4));

    // 3체인: 파도부대 6명
    public override void Chain3(Vector3 targetPos, float scaleFactor) =>
        StartCoroutine(SpawnUnits(targetPos, scaleFactor, 6));

    IEnumerator SpawnUnits(Vector3 targetPos, float scaleFactor, int count)
    {
        if (effectPrefab == null)
            yield break;

        for (int i = 0; i < count; i++)
        {
            float side = (i % 2 == 0) ? -1f : 1f;
            Vector3 spawnPos = targetPos + new Vector3(side * spawnOffsetX, 0f, 0f);
            var go = Instantiate(effectPrefab, spawnPos, Quaternion.identity);
            go.transform.localScale = Vector3.one * scaleFactor;
            StartCoroutine(MoveUnit(go, spawnPos, targetPos));

            yield return new WaitForSeconds(unitSpawnInterval);
        }
    }

    IEnumerator MoveUnit(GameObject go, Vector3 from, Vector3 to)
    {
        float t = 0f;
        while (t < unitTravelDuration)
        {
            t += Time.deltaTime;
            if (go == null)
                yield break;
            go.transform.position = Vector3.Lerp(from, to, t / unitTravelDuration);
            yield return null;
        }
        if (go != null)
            Destroy(go, 0.3f);
    }
}
