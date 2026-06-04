using System.Collections;
using UnityEngine;

public class Skill_Archon : Skill
{
    [SerializeField]
    float hitYOffset = 0f;

    [SerializeField]
    float scaleYCorrection = 0f;

    ArchonCharacter _archon;

    void Awake() => _archon = GetComponent<ArchonCharacter>();

    float ScaleFromStack()
    {
        int stack = _archon != null ? _archon.StackCount : 0;
        if (stack >= 10)
            return 2f;
        if (stack >= 5)
            return 1.5f;
        return 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            Thunder(testPos, 1f);
        if (Input.GetKeyDown(KeyCode.B))
        {
            Thunder(testPos, 1.5f);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Thunder(testPos, 2f);
        }
    }

    public override void Chain1(Vector3 targetPos, float scaleFactor) =>
        Thunder(targetPos, ScaleFromStack());

    public override void Chain2(Vector3 targetPos, float scaleFactor) =>
        Thunder(targetPos, ScaleFromStack());

    public override void Chain3(Vector3 targetPos, float scaleFactor) =>
        Thunder(targetPos, ScaleFromStack());

    void Thunder(Vector3 targetPos, float scale)
    {
        if (effectPrefab == null)
            return;
        targetPos.y += hitYOffset - (scale - 1f) * scaleYCorrection;
        var go = Instantiate(effectPrefab, targetPos, Quaternion.identity);
        go.transform.localScale = Vector3.one * scale;
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
        Destroy(go, 1f);
    }
}
