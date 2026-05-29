using TMPro;
using UnityEngine;

public class HandCountUI : MonoBehaviour
{
    [SerializeField]
    GameManager gameManager;

    [SerializeField]
    TMP_Text countText;

    void OnEnable()
    {
        gameManager.OnHandPlayCountChanged += Refresh;
    }

    void OnDisable()
    {
        gameManager.OnHandPlayCountChanged -= Refresh;
    }

    void Refresh(int current, int max)
    {
        if (countText != null)
            countText.text = $"{current} / {max}";
    }
}
