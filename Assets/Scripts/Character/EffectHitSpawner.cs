using UnityEngine;

public class EffectHitSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject hitEffectPrefab;

    [SerializeField]
    Vector3 spawnOffset = Vector3.zero;

    [SerializeField]
    bool inheritScale = false;

    Skill _skill;

    void Awake() => _skill = GetComponentInParent<Skill>();

    public void HitEffect()
    {
        _skill?.PlayBossHitSound();
        if (hitEffectPrefab == null)
            return;
        var go = Instantiate(
            hitEffectPrefab,
            transform.position + spawnOffset,
            Quaternion.identity
        );
        if (inheritScale)
            go.transform.localScale = transform.localScale;
        var anim = go.GetComponent<Animator>();
        if (anim != null && anim.runtimeAnimatorController != null)
        {
            var clips = anim.runtimeAnimatorController.animationClips;
            if (clips.Length > 0)
            {
                Destroy(go, clips[0].length);
                return;
            }
        }
        Destroy(go);
    }
}
