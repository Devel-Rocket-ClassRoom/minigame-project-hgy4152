using System.Collections;
using UnityEngine;

public class Skill_Hikari_GustArrow : Skill
{
    [SerializeField]
    float moveDuration = 0.2f;

    public override void Chain1(Vector3 targetPos, float scaleFactor) =>
        FireArrow(targetPos, scaleFactor);

    public override void Chain2(Vector3 targetPos, float scaleFactor) =>
        FireArrow(targetPos, scaleFactor);

    public override void Chain3(Vector3 targetPos, float scaleFactor) =>
        FireArrow(targetPos, scaleFactor);

    void FireArrow(Vector3 targetPos, float scale)
    {
        if (effectPrefab == null)
            return;
        var go = Instantiate(effectPrefab, transform.position, Quaternion.identity);
        go.transform.localScale = Vector3.one * scale;
        StartCoroutine(MoveTo(go, targetPos));
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
            go.transform.position = targetPos;
    }
}
