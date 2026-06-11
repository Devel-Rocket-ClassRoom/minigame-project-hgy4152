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
