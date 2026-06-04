using UnityEngine;

public class Skill_Beatrice : Skill
{
    [SerializeField]
    Vector3 effectPivotOffset;

    BeatriceCharacter _character;

    void Awake() => _character = GetComponent<BeatriceCharacter>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            Chain1(testPos, 0);

        if (Input.GetKeyDown(KeyCode.X))
        {
            Chain1(testPos, 0);
            Chain2(testPos, 0);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Chain1(testPos, 0);
            Chain2(testPos, 0);
            Chain3(testPos, 0);
        }
    }

    public override void Chain1(Vector3 targetPos, float scaleFactor) => Descend(targetPos);

    public override void Chain2(Vector3 targetPos, float scaleFactor) => Descend(targetPos);

    public override void Chain3(Vector3 targetPos, float scaleFactor) => Descend(targetPos);

    void Descend(Vector3 targetPos)
    {
        if (effectPrefab == null)
            return;

        int stacks = _character?.StackCount ?? 0;
        float scale =
            stacks >= 21 ? 2f
            : stacks >= 11 ? 1.5f
            : 1f;

        var go = Instantiate(
            effectPrefab,
            targetPos - effectPivotOffset * scale,
            Quaternion.identity
        );
        go.transform.localScale *= scale;
        Destroy(go, 0.5f);
    }
}
