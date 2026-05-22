using UnityEngine;

public class Skill_Beatrice_Judgement : Skill
{
    [SerializeField]
    Vector3 descendOffset = new Vector3(0, 3f, 0);

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
        var go = Instantiate(effectPrefab, targetPos + descendOffset, Quaternion.identity);
        go.transform.localScale = Vector3.one * scale;
    }
}
