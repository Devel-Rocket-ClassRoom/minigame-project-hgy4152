using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Skill_Reon_Excalibur : Skill
{
    [SerializeField]
    float spawnOffset = 12f;

    [SerializeField]
    float moveDuration = 0.15f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Chain1(testPos, 1f);
        }

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

    public override void Chain1(Vector3 targetPos, float scaleFactor)
    {
        targetPos.x -= 1f;
        if (effectPrefab == null)
            return;
        Vector3 startPos = targetPos + new Vector3(-spawnOffset, spawnOffset, 0);
        var go = Instantiate(effectPrefab);
        go.transform.position = startPos;
        go.transform.localScale *= scaleFactor;
        StartCoroutine(MoveTo(go, startPos, targetPos));
    }

    public override void Chain2(Vector3 targetPos, float scaleFactor)
    {
        targetPos.x += 0.5f;
        if (effectPrefab == null)
            return;
        Vector3 startPos = targetPos + new Vector3(spawnOffset, spawnOffset, 0);
        var go = Instantiate(effectPrefab);
        go.transform.position = startPos;
        go.GetComponent<SpriteRenderer>().flipY = true;
        go.transform.localScale *= scaleFactor;
        StartCoroutine(MoveTo(go, startPos, targetPos));
    }

    public override void Chain3(Vector3 targetPos, float scaleFactor)
    {
        targetPos.x -= 0.25f;
        targetPos.y = 0f;
        if (effectPrefab == null)
            return;
        Vector3 startPos = targetPos + new Vector3(0, spawnOffset + 3f, 0);
        var go = Instantiate(effectPrefab, startPos, Quaternion.Euler(0, 0, -135f));
        go.transform.localScale *= scaleFactor * 1.2f;
        StartCoroutine(MoveTo(go, startPos, targetPos));
    }

    IEnumerator MoveTo(GameObject go, Vector3 start, Vector3 target)
    {
        float t = 0f;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            if (go == null)
                yield break;
            float ratio = t / moveDuration;
            go.transform.position = Vector3.Lerp(start, target, ratio * ratio);
            yield return null;
        }
        if (go != null)
        {
            go.transform.position = target;
            Destroy(go, 0.5f);
        }
    }
}
