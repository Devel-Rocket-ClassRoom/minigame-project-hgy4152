using System.Collections;
using UnityEngine;

public class Skill_Archon_ThunderBolt : Skill
{
    [SerializeField]
    float dropHeight = 8f;

    [SerializeField]
    float dropDuration = 0.2f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            Chain1(testPos, 1f);
        if (Input.GetKeyDown(KeyCode.B))
        {
            Chain1(testPos, 1.5f);
            Chain2(testPos, 1.5f);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Chain1(testPos, 2f);
            Chain2(testPos, 2f);
            Chain3(testPos, 2f);
        }
    }

    public override void Chain1(Vector3 targetPos, float scaleFactor) =>
        Thunder(targetPos, scaleFactor);

    public override void Chain2(Vector3 targetPos, float scaleFactor) =>
        Thunder(targetPos, scaleFactor);

    public override void Chain3(Vector3 targetPos, float scaleFactor) =>
        Thunder(targetPos, scaleFactor);

    void Thunder(Vector3 targetPos, float scale)
    {
        if (effectPrefab == null)
            return;
        Vector3 start = targetPos + new Vector3(0, dropHeight, 0);
        var go = Instantiate(effectPrefab, start, Quaternion.identity);
        go.transform.localScale = Vector3.one * scale;
        StartCoroutine(Drop(go, start, targetPos));
    }

    IEnumerator Drop(GameObject go, Vector3 start, Vector3 target)
    {
        float t = 0f;
        while (t < dropDuration)
        {
            t += Time.deltaTime;
            if (go == null)
                yield break;
            go.transform.position = Vector3.Lerp(start, target, t / dropDuration);
            yield return null;
        }
        if (go != null)
        {
            go.transform.position = target;
            Destroy(go, 0.5f);
        }
    }
}
