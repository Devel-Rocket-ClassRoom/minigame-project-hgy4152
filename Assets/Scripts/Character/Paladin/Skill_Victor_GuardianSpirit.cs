using UnityEngine;

public class Skill_Victor_HolySlash : Skill
{
    [SerializeField]
    float forwardOffset = 2f;

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

    public override void Chain1(Vector3 targetPos, float scaleFactor) => Slash(scaleFactor);

    public override void Chain2(Vector3 targetPos, float scaleFactor) => Slash(scaleFactor);

    public override void Chain3(Vector3 targetPos, float scaleFactor) => Slash(scaleFactor);

    void Slash(float scale)
    {
        if (effectPrefab == null)
            return;
        Vector3 spawnPos = transform.position + new Vector3(forwardOffset, 0, 0);
        var go = Instantiate(effectPrefab, spawnPos, Quaternion.identity);
        go.transform.localScale = Vector3.one * scale;
        Destroy(go, 0.5f);
    }
}
