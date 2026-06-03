using System.Collections;
using UnityEngine;

public class Skill_CaptainJG : Skill
{
    [SerializeField]
    float spawnOffsetX = 3f;

    [SerializeField]
    float spawnYMin = 1f;

    [SerializeField]
    float spawnYMax = 3f;

    [SerializeField]
    float unitSpawnInterval = 0.1f;

    [SerializeField]
    float unitTravelDuration = 0.25f;

    [SerializeField]
    Sprite[] unitSprites;

    [SerializeField]
    GameObject hitEffectPrefab;

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
        bool hasSprites = unitSprites != null && unitSprites.Length > 0;
        if (effectPrefab == null && !hasSprites)
            yield break;

        GameObject hitEffect =
            hitEffectPrefab != null
                ? Instantiate(hitEffectPrefab, targetPos, Quaternion.identity)
                : null;

        int[] remaining = { count };

        for (int i = 0; i < count; i++)
        {
            float randX = Random.Range(-spawnOffsetX, spawnOffsetX);
            float randY = Random.Range(spawnYMin, spawnYMax);
            Vector3 spawnPos = targetPos + new Vector3(randX, randY, 0f);

            GameObject go;
            if (effectPrefab != null)
            {
                go = Instantiate(effectPrefab, spawnPos, Quaternion.identity);
            }
            else
            {
                go = new GameObject("Unit");
                go.transform.position = spawnPos;
                go.AddComponent<SpriteRenderer>();
            }
            if (hasSprites)
            {
                var sr = go.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.sprite = unitSprites[Random.Range(0, unitSprites.Length)];
            }

            StartCoroutine(
                MoveUnit(
                    go,
                    spawnPos,
                    targetPos,
                    () =>
                    {
                        if (--remaining[0] <= 0 && hitEffect != null)
                            Destroy(hitEffect);
                    }
                )
            );

            yield return new WaitForSeconds(unitSpawnInterval);
        }
    }

    IEnumerator MoveUnit(GameObject go, Vector3 from, Vector3 to, System.Action onDone)
    {
        float t = 0f;
        while (t < unitTravelDuration)
        {
            t += Time.deltaTime;
            if (go == null)
                break;
            go.transform.position = Vector3.Lerp(from, to, Mathf.Clamp01(t / unitTravelDuration));
            yield return null;
        }
        if (go != null)
            Destroy(go);
        onDone?.Invoke();
    }
}
