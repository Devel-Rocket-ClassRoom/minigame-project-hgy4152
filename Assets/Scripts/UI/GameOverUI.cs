using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    GameObject panel;

    [SerializeField]
    Button restartButton;

    void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    void Start()
    {
        restartButton.onClick.AddListener(OnRestartClicked);
    }

    public void Show()
    {
        if (panel != null)
            panel.SetActive(true);
    }

    void OnRestartClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
