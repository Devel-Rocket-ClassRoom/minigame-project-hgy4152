using System.Collections;
using UnityEngine;

public class Skill_AhnMansik : Skill
{
    [SerializeField]
    Sprite grenadeSprite;

    [SerializeField]
    float throwDuration = 0.4f;

    [SerializeField]
    float grenadeInterval = 0.12f;

    [SerializeField]
    float arcHeightMin = 2f;

    [SerializeField]
    float arcHeightMax = 5f;

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

    // 1체인: 수류탄 1개
    public override void Chain1(Vector3 targetPos, float scaleFactor) =>
        StartCoroutine(ThrowGrenades(targetPos, 1));

    // 2체인: 수류탄 2개
    public override void Chain2(Vector3 targetPos, float scaleFactor) =>
        StartCoroutine(ThrowGrenades(targetPos, 2));

    // 3체인: 수류탄 3개
    public override void Chain3(Vector3 targetPos, float scaleFactor) =>
        StartCoroutine(ThrowGrenades(targetPos, 3));

    IEnumerator ThrowGrenades(Vector3 targetPos, int count)
    {
        float[] arcHeights = new float[count];
        arcHeights[0] = Random.Range(arcHeightMin, arcHeightMax);
        for (int i = 1; i < count; i++)
        {
            float h;
            int attempts = 0;
            do
            {
                h = Random.Range(arcHeightMin, arcHeightMax);
                attempts++;
            } while (Mathf.Abs(h - arcHeights[i - 1]) < 0.5f && attempts < 100);
            arcHeights[i] = h;
        }

        for (int i = 0; i < count; i++)
        {
            float offsetX = (i - (count - 1) * 0.5f) * 0.4f;
            Vector3 landPos = targetPos + new Vector3(offsetX, 0f, 0f);

            var go = new GameObject("Grenade");
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = grenadeSprite;
            go.transform.position = transform.position;

            StartCoroutine(Throw(go, transform.position, landPos, arcHeights[i]));

            if (i < count - 1)
                yield return new WaitForSeconds(grenadeInterval);
        }
    }

    IEnumerator Throw(GameObject go, Vector3 start, Vector3 target, float arcHeight)
    {
        float t = 0f;
        while (t < throwDuration)
        {
            t += Time.deltaTime;
            if (go == null)
                yield break;
            float ratio = Mathf.Clamp01(t / throwDuration);
            go.transform.position =
                Vector3.Lerp(start, target, ratio)
                + Vector3.up * (arcHeight * Mathf.Sin(Mathf.PI * ratio));
            yield return null;
        }
        if (go != null)
        {
            go.transform.position = target;
            Destroy(go);

            if (effectPrefab != null)
            {
                var effect = Instantiate(effectPrefab, target, Quaternion.identity);
                DestroyAfterAnimation(effect, 0.5f);
            }
        }
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
}
