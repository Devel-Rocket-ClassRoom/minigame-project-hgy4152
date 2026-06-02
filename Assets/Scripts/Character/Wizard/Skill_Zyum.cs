using UnityEngine;

public class Skill_Zyum : Skill
{
    [SerializeField]
    float explosionDuration = 0.5f;

    [SerializeField]
    GameObject passiveExplosionPrefab;

    bool _passiveActive;

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

    public override void Chain1(Vector3 targetPos, float scaleFactor) =>
        Explode(targetPos, scaleFactor);

    public override void Chain2(Vector3 targetPos, float scaleFactor) =>
        Explode(targetPos, scaleFactor);

    public override void Chain3(Vector3 targetPos, float scaleFactor) =>
        Explode(targetPos, scaleFactor);

    void Explode(Vector3 targetPos, float scaleFactor)
    {
        if (effectPrefab != null)
        {
            var go = Instantiate(effectPrefab, targetPos, Quaternion.identity);
            go.transform.localScale = Vector3.one * scaleFactor;
            Destroy(go, explosionDuration);
        }

        if (_passiveActive && passiveExplosionPrefab != null)
        {
            var bonus = Instantiate(
                passiveExplosionPrefab,
                targetPos + new Vector3(0f, 0.5f, 0f),
                Quaternion.identity
            );
            bonus.transform.localScale = Vector3.one * scaleFactor * 1.3f;
            Destroy(bonus, explosionDuration);
        }
    }

    public void SetPassiveActive(bool active)
    {
        _passiveActive = active;
    }
}
