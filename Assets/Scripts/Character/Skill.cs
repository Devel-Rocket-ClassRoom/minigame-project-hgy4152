using UnityEngine;

public abstract class Skill : MonoBehaviour
{
    [SerializeField]
    protected GameObject effectPrefab;

    [Header("=== 오디오 ===")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip skillCastClip;
    [SerializeField] AudioClip bossHitClip;

    public Transform target;
    public Vector3 testPos => target.position;

    public abstract void Chain1(Vector3 targetPos, float scaleFactor);
    public abstract void Chain2(Vector3 targetPos, float scaleFactor);
    public abstract void Chain3(Vector3 targetPos, float scaleFactor);

    // 풀 기반 투사체/이펙트 스폰·반환 (즉시 반환 패턴 전용 — 시간차 Destroy는 기존 유지)
    protected static GameObject SpawnEffect(GameObject prefab, Vector3 pos, Quaternion rotation)
    {
        var go = GameObjectPool.Get(prefab, null);
        go.transform.SetPositionAndRotation(pos, rotation);
        go.transform.localScale = prefab.transform.localScale; // 재사용 시 스케일 잔존 방지
        return go;
    }

    protected static void ReleaseEffect(GameObject go) => GameObjectPool.Release(go);

    public void PlayCastSound()
    {
        if (audioSource != null && skillCastClip != null)
            audioSource.PlayOneShot(skillCastClip);
    }

    public void PlayBossHitSound()
    {
        if (audioSource != null && bossHitClip != null)
            audioSource.PlayOneShot(bossHitClip);
    }

    public override string ToString() => GetType().Name;
}
