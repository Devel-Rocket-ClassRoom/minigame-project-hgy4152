using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    const string BGMVolumeKey = "Setting.BGMVolume";
    const string SFXVolumeKey = "Setting.SFXVolume";
    const string BGMExposedParam = "BGMVolume";
    const string SFXExposedParam = "SFXVolume";
    const float DefaultVolume = 0.8f;
    const float MinVolume = 0.0001f;

    [SerializeField]
    GameObject panel;

    [SerializeField]
    AudioMixer mixer;

    [SerializeField]
    Slider bgmSlider;

    [SerializeField]
    Slider sfxSlider;

    [SerializeField]
    Button closeButton;

    [SerializeField]
    Button quitButton;

    void Awake()
    {
        if (panel != null)
            panel.SetActive(false);

        if (bgmSlider != null)
        {
            bgmSlider.minValue = MinVolume;
            bgmSlider.maxValue = 1f;
            bgmSlider.value = PlayerPrefs.GetFloat(BGMVolumeKey, DefaultVolume);
            bgmSlider.onValueChanged.AddListener(SetBgmVolume);
            ApplyBgmVolume(bgmSlider.value);
        }
        if (sfxSlider != null)
        {
            sfxSlider.minValue = MinVolume;
            sfxSlider.maxValue = 1f;
            sfxSlider.value = PlayerPrefs.GetFloat(SFXVolumeKey, DefaultVolume);
            sfxSlider.onValueChanged.AddListener(SetSfxVolume);
            ApplySfxVolume(sfxSlider.value);
        }
        if (closeButton != null)
            closeButton.onClick.AddListener(Close);
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    public void Open()
    {
        if (panel != null)
            panel.SetActive(true);
    }

    public void Close()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    void SetBgmVolume(float v)
    {
        ApplyBgmVolume(v);
        PlayerPrefs.SetFloat(BGMVolumeKey, v);
    }

    void SetSfxVolume(float v)
    {
        ApplySfxVolume(v);
        PlayerPrefs.SetFloat(SFXVolumeKey, v);
    }

    void ApplyBgmVolume(float v)
    {
        if (mixer != null)
            mixer.SetFloat(BGMExposedParam, Mathf.Log10(Mathf.Max(v, MinVolume)) * 20f);
    }

    void ApplySfxVolume(float v)
    {
        if (mixer != null)
            mixer.SetFloat(SFXExposedParam, Mathf.Log10(Mathf.Max(v, MinVolume)) * 20f);
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
