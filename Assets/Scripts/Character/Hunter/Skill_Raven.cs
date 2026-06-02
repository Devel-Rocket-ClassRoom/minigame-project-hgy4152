using System.Collections;
using UnityEngine;

public class Skill_Raven : Skill
{
    [SerializeField]
    float moveDuration = 0.25f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
            Chain1(testPos, 1f);
        if (Input.GetKeyDown(KeyCode.I))
        {
            Chain1(testPos, 1.5f);
            Chain2(testPos, 1.5f);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Chain1(testPos, 2f);
            Chain2(testPos, 2f);
            Chain3(testPos, 2f);
        }
    }

    public override void Chain1(Vector3 targetPos, float scaleFactor) =>
        Shoot(targetPos, scaleFactor);

    public override void Chain2(Vector3 targetPos, float scaleFactor) =>
        Shoot(targetPos, scaleFactor);

    public override void Chain3(Vector3 targetPos, float scaleFactor) =>
        Shoot(targetPos, scaleFactor);

    void Shoot(Vector3 targetPos, float scale)
    {
        if (effectPrefab == null)
            return;
        Vector3 start = transform.position;
        Vector3 dir = (targetPos - start).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        var go = Instantiate(effectPrefab, start, Quaternion.Euler(0, 0, angle));
        go.transform.localScale = Vector3.one * scale;
        StartCoroutine(MoveTo(go, start, targetPos));
    }

    IEnumerator MoveTo(GameObject go, Vector3 start, Vector3 target)
    {
        float t = 0f;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            if (go == null)
                yield break;
            go.transform.position = Vector3.Lerp(start, target, t / moveDuration);
            yield return null;
        }
        if (go != null)
            Destroy(go);
    }
}
