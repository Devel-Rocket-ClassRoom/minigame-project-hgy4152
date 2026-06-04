using System.Collections;
using UnityEngine;

public class Skill_Hikari : Skill
{
    [SerializeField]
    float moveDuration = 0.2f;

    [SerializeField]
    float shotInterval = 0.25f;

    [SerializeField]
    GameObject hitEffectPrefab;

    [SerializeField]
    GameObject chain3HitEffectPrefab;

    HikariCharacter _character;

    void Awake() => _character = GetComponent<HikariCharacter>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            Chain1(testPos, 1f);
        if (Input.GetKeyDown(KeyCode.S))
            Chain2(testPos, 1.5f);
        if (Input.GetKeyDown(KeyCode.D))
            Chain3(testPos, 2f);
    }

    public override void Chain1(Vector3 targetPos, float scaleFactor) =>
        StartCoroutine(FireSequence(targetPos, scaleFactor, hitEffectPrefab, 1));

    public override void Chain2(Vector3 targetPos, float scaleFactor) =>
        StartCoroutine(FireSequence(targetPos, scaleFactor, hitEffectPrefab, 2));

    public override void Chain3(Vector3 targetPos, float scaleFactor) =>
        StartCoroutine(FireSequence(targetPos, scaleFactor, chain3HitEffectPrefab, 3));

    IEnumerator FireSequence(Vector3 targetPos, float scale, GameObject hitPrefab, int count)
    {
        for (int i = 0; i < count; i++)
        {
            _character?.StartJump();
            GustArrow(targetPos, scale, hitPrefab);
            if (i < count - 1)
                yield return new WaitForSeconds(shotInterval);
        }
    }

    void GustArrow(Vector3 targetPos, float scale, GameObject hitPrefab)
    {
        if (effectPrefab == null)
            return;
        var go = Instantiate(effectPrefab, transform.position, Quaternion.identity);
        go.transform.localScale = Vector3.one * scale;
        StartCoroutine(MoveTo(go, targetPos, hitPrefab));
    }

    IEnumerator MoveTo(GameObject go, Vector3 targetPos, GameObject hitPrefab)
    {
        Vector3 start = go.transform.position;
        float t = 0f;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            if (go == null)
                yield break;
            go.transform.position = Vector3.Lerp(start, targetPos, t / moveDuration);
            yield return null;
        }
        if (go != null)
        {
            go.transform.position = targetPos;
            SpawnAnimEffect(hitPrefab, targetPos);
            Destroy(go);
        }
    }

    void SpawnAnimEffect(GameObject prefab, Vector3 pos)
    {
        if (prefab == null)
            return;
        var fx = Instantiate(prefab, pos, Quaternion.identity);
        var anim = fx.GetComponent<Animator>();
        if (anim != null && anim.runtimeAnimatorController != null)
        {
            var clips = anim.runtimeAnimatorController.animationClips;
            if (clips.Length > 0)
            {
                Destroy(fx, clips[0].length);
                return;
            }
        }
        Destroy(fx, 1f);
    }
}
