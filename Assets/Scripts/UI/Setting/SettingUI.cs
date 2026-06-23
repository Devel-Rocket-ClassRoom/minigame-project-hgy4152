using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingUI : MonoBehaviour
{
    const string BGMExposedParam = "BGMVolume";
    const string SFXExposedParam = "SFXVolume";
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

    [SerializeField]
    Button resetDataButton;

    [SerializeField]
    AuthUI authUI;

    public UnityEvent onClose = new UnityEvent();

    SettingsModel _model;

    void Awake()
    {
        _model = new SettingsModel();

        if (panel != null)
            panel.SetActive(false);

        if (bgmSlider != null)
        {
            bgmSlider.minValue = MinVolume;
            bgmSlider.maxValue = 1f;
            bgmSlider.value = _model.BgmVolume;
            bgmSlider.onValueChanged.AddListener(SetBgmVolume);
            ApplyBgmVolume(bgmSlider.value);
        }
        if (sfxSlider != null)
        {
            sfxSlider.minValue = MinVolume;
            sfxSlider.maxValue = 1f;
            sfxSlider.value = _model.SfxVolume;
            sfxSlider.onValueChanged.AddListener(SetSfxVolume);
            ApplySfxVolume(sfxSlider.value);
        }
        if (closeButton != null)
            closeButton.onClick.AddListener(Close);
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
        if (resetDataButton != null)
            resetDataButton.onClick.AddListener(ResetAllData);
    }

    // 닫기 외 경로(씬 전환 등)로 비활성화돼도 변경분을 저장
    void OnDisable() => _model?.Save();

    public void Open()
    {
        if (panel != null)
            panel.SetActive(true);
    }

    public void Close()
    {
        _model.Save(); // 더티 플래그: 변경이 있을 때만 닫는 시점에 1회 기록
        if (panel != null)
            panel.SetActive(false);
        onClose?.Invoke();
    }

    void SetBgmVolume(float v)
    {
        ApplyBgmVolume(v); // 청각 피드백은 즉시, 저장은 Model에 위임
        _model.SetBgmVolume(v);
    }

    void SetSfxVolume(float v)
    {
        ApplySfxVolume(v);
        _model.SetSfxVolume(v);
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

    public void OnAccountLinkClicked()
    {
        if (authUI != null)
            authUI.gameObject.SetActive(true);
    }

    void ResetAllData()
    {
        UnlockManager.ResetAll();
        SaveManager.DeleteAll();
        Debug.Log("[Debug] 세이브 데이터 초기화 완료");
    }

    void QuitGame()
    {
        if (
            GameStateMachine.Instance != null
            && GameStateMachine.Instance.CurrentState != GameState.Lobby
        )
        {
            Time.timeScale = 1f;
            GameStateMachine.Instance.TransitionToLobby();
            return;
        }
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
