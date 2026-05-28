using UnityEngine;
using UnityEngine.UI;

public class PauseButtonUI : MonoBehaviour
{
    [SerializeField]
    Button pauseButton;

    [SerializeField]
    SettingUI settingUI;

    void Start()
    {
        pauseButton?.onClick.AddListener(Pause);
        settingUI?.onClose.AddListener(Resume);
    }

    void Pause()
    {
        Time.timeScale = 0f;
        settingUI?.Open();
    }

    void Resume()
    {
        Time.timeScale = 1f;
    }
}
