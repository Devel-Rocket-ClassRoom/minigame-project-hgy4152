using UnityEngine;

public class SceneBGM : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip bgmClip;

    void Start()
    {
        if (audioSource == null || bgmClip == null)
            return;
        audioSource.clip = bgmClip;
        audioSource.loop = true;
        audioSource.Play();
    }
}
