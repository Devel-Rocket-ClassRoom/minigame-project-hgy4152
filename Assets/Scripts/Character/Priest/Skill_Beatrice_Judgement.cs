using UnityEngine;

public class Skill_Beatrice_Judgement : Skill
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Chain1(testPos, 1f);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Chain1(testPos, 1.5f);
            Chain2(testPos, 1.5f);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Chain1(testPos, 2f);
            Chain2(testPos, 2f);
            Chain3(testPos, 2f);
        }
    }

    public override void Chain1(Vector3 targetPos, float scaleFactor) =>
        Descend(targetPos, scaleFactor);

    public override void Chain2(Vector3 targetPos, float scaleFactor) =>
        Descend(targetPos, scaleFactor);

    public override void Chain3(Vector3 targetPos, float scaleFactor) =>
        Descend(targetPos, scaleFactor);

    void Descend(Vector3 targetPos, float scale)
    {
        if (effectPrefab == null)
            return;
        var go = Instantiate(effectPrefab, targetPos + 0.5f * Vector3.up, Quaternion.identity);
        go.transform.localScale = Vector3.one * scale;

        Destroy(go, 0.5f);
    }
}
