using UnityEngine;

// 설정 화면의 Model (MVP) — 슬라이더 변경은 메모리에만 반영(더티 플래그)하고
// Save() 호출 시에만 PlayerPrefs에 1회 기록한다. SettingUI는 View 역할만 담당.
public class SettingsModel
{
    const string BGMVolumeKey = "Setting.BGMVolume";
    const string SFXVolumeKey = "Setting.SFXVolume";
    const float DefaultVolume = 0.8f;

    public float BgmVolume { get; private set; }
    public float SfxVolume { get; private set; }
    public bool IsDirty { get; private set; }

    public SettingsModel()
    {
        BgmVolume = PlayerPrefs.GetFloat(BGMVolumeKey, DefaultVolume);
        SfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey, DefaultVolume);
    }

    public void SetBgmVolume(float value)
    {
        if (Mathf.Approximately(BgmVolume, value))
            return;
        BgmVolume = value;
        IsDirty = true;
    }

    public void SetSfxVolume(float value)
    {
        if (Mathf.Approximately(SfxVolume, value))
            return;
        SfxVolume = value;
        IsDirty = true;
    }

    public void Save()
    {
        if (!IsDirty)
            return;
        PlayerPrefs.SetFloat(BGMVolumeKey, BgmVolume);
        PlayerPrefs.SetFloat(SFXVolumeKey, SfxVolume);
        PlayerPrefs.Save();
        IsDirty = false;
    }
}
