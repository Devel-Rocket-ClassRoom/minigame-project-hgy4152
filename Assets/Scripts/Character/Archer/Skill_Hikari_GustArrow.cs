using System.Collections;
using UnityEngine;

public class Skill_Hikari_GustArrow : Skill
{
    [SerializeField]
    float moveDuration = 0.2f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Chain1(testPos, 1f);
        }

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

    public override void Chain1(Vector3 targetPos, float scaleFactor) =>
        GustArrow(targetPos, scaleFactor);

    public override void Chain2(Vector3 targetPos, float scaleFactor) =>
        GustArrow(targetPos, scaleFactor);

    public override void Chain3(Vector3 targetPos, float scaleFactor) =>
        GustArrow(targetPos, scaleFactor);

    void GustArrow(Vector3 targetPos, float scale)
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

        yield return null;
        Destroy(go);
    }
}
