using UnityEngine;

public class Skill_Victor : Skill
{
    [SerializeField]
    GameObject hitEffectPrefab;

    [Header("=== Passive - 수호의 혼 ===")]
    [SerializeField]
    public GameObject passiveEffectPrefab;

    VitorCharacter _character;

    void Awake() => _character = GetComponent<VitorCharacter>();

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
        Slash(targetPos, scaleFactor);

    public override void Chain2(Vector3 targetPos, float scaleFactor) =>
        Slash(targetPos, scaleFactor);

    public override void Chain3(Vector3 targetPos, float scaleFactor) =>
        Slash(targetPos, scaleFactor);

    void Slash(Vector3 targetPos, float scale)
    {
        _character?.OnChargeStartEvent(
            targetPos,
            () =>
            {
                if (hitEffectPrefab != null)
                {
                    var hit = Instantiate(hitEffectPrefab, targetPos, Quaternion.identity);
                    hit.transform.localScale = Vector3.one * scale;
                    Destroy(hit, 0.5f);
                }
            }
        );
    }
}
