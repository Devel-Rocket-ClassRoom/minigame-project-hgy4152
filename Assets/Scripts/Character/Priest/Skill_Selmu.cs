using System.Collections;
using UnityEngine;

public class Skill_Selmu : Skill
{
    [SerializeField]
    float explosionDuration = 0.5f;

    [SerializeField]
    GameObject totemEffectPrefab;

    [SerializeField]
    float totemBackOffset = 1f;

    [SerializeField]
    GameObject consumeEffectPrefab;

    GameObject _activeTotem;

    void Start()
    {
        SpawnTotemBehind();
    }

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
        if (effectPrefab == null)
            return;
        var go = Instantiate(effectPrefab, targetPos, Quaternion.identity);
        go.transform.localScale = Vector3.one * scaleFactor;
        DestroyAfterAnimation(go, explosionDuration);
    }

    void DestroyAfterAnimation(GameObject go, float fallback)
    {
        var anim = go.GetComponentInChildren<Animator>();
        if (anim != null && anim.runtimeAnimatorController != null)
        {
            var clips = anim.runtimeAnimatorController.animationClips;
            if (clips.Length > 0)
            {
                Destroy(go, clips[0].length);
                return;
            }
        }
        Destroy(go, fallback);
    }

    public void SpawnTotemBehind() =>
        SpawnTotem(transform.position + new Vector3(-totemBackOffset, 0f, 0f));

    public void SpawnTotem(Vector3 position)
    {
        if (totemEffectPrefab == null)
            return;
        if (_activeTotem != null)
            Destroy(_activeTotem);
        _activeTotem = Instantiate(totemEffectPrefab, position, Quaternion.identity);
        _activeTotem.SetActive(false);
    }

    public void SetTotemActive(bool active)
    {
        if (_activeTotem != null)
            _activeTotem.SetActive(active);
        SetPassivePSActive(active);
    }

    public void SetPassivePSActive(bool active)
    {
        if (_activeTotem == null)
            return;
        var ps = _activeTotem.GetComponentInChildren<ParticleSystem>();
        if (ps != null)
            ps.gameObject.SetActive(active);
    }

    public void PlayConsumeEffect(Vector3 targetPos, float scaleFactor)
    {
        if (consumeEffectPrefab == null)
            return;
        var go = Instantiate(consumeEffectPrefab, targetPos, Quaternion.identity);
        go.transform.localScale = Vector3.one * scaleFactor;
        DestroyAfterAnimation(go, 0.5f);
    }
}
