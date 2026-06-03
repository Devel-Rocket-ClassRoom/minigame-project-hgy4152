using System.Collections;
using UnityEngine;

public class Skill_Zyum : Skill
{
    [SerializeField]
    float explosionDuration = 0.5f;

    [SerializeField]
    GameObject passiveExplosionPrefab;

    [SerializeField]
    Transform weaponTransform;

    [SerializeField]
    float weaponXOffset = 1.5f;

    [SerializeField]
    float weaponMoveDuration = 0.3f;

    bool _passiveActive;
    Vector3 _weaponOriginPos;

    void Start()
    {
        if (weaponTransform != null)
            _weaponOriginPos = weaponTransform.position;
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

        if (Input.GetKeyDown(KeyCode.P))
        {
            _passiveActive = !_passiveActive;
            Debug.Log($"[Zyum] 패시브 {(_passiveActive ? "ON" : "OFF")}");
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
            if (go.transform.childCount >= 2)
            {
                var first = go.transform.GetChild(0).GetComponent<SpriteRenderer>();
                var second = go.transform.GetChild(1).GetComponent<SpriteRenderer>();
                if (first != null && second != null)
                    first.sortingOrder = second.sortingOrder + 1;
            }
            DestroyAfterAnimation(go, explosionDuration);
        }

        if (_passiveActive && weaponTransform != null)
            StartCoroutine(MoveWeaponAndExplode(targetPos, scaleFactor));
    }

    IEnumerator MoveWeaponAndExplode(Vector3 targetPos, float scaleFactor)
    {
        Vector3 arrivalPos = targetPos + new Vector3(-weaponXOffset, 0f, 0f);
        Vector3 originPos = _weaponOriginPos;

        float t = 0f;
        while (t < weaponMoveDuration)
        {
            t += Time.deltaTime;
            if (weaponTransform == null)
                yield break;
            weaponTransform.position = Vector3.Lerp(originPos, arrivalPos, t / weaponMoveDuration);
            yield return null;
        }

        weaponTransform.position = arrivalPos;

        float animDuration = explosionDuration;
        if (passiveExplosionPrefab != null)
        {
            var bonus = Instantiate(passiveExplosionPrefab, targetPos, Quaternion.identity);
            bonus.transform.localScale = Vector3.one * scaleFactor * 1.3f;
            var anim = bonus.GetComponentInChildren<Animator>();
            if (anim != null && anim.runtimeAnimatorController != null)
            {
                var clips = anim.runtimeAnimatorController.animationClips;
                if (clips.Length > 0)
                    animDuration = clips[0].length;
            }
            Destroy(bonus, animDuration);
        }

        yield return new WaitForSeconds(animDuration);

        t = 0f;
        while (t < weaponMoveDuration)
        {
            t += Time.deltaTime;
            if (weaponTransform == null)
                yield break;
            weaponTransform.position = Vector3.Lerp(arrivalPos, originPos, t / weaponMoveDuration);
            yield return null;
        }

        if (weaponTransform != null)
            weaponTransform.position = originPos;
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

    public void SetPassiveActive(bool active)
    {
        _passiveActive = active;
    }
}
