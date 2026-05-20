using UnityEngine;

public class ModeClearUI : MonoBehaviour
{
    [SerializeField]
    GameObject panel;

    void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public void Show()
    {
        if (panel != null)
            panel.SetActive(true);
    }
}
