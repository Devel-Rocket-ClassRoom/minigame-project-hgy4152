using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmDialogUI : MonoBehaviour
{
    [SerializeField]
    GameObject panel;

    [SerializeField]
    TMP_Text messageText;

    [SerializeField]
    Button yesButton;

    [SerializeField]
    Button noButton;

    void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public void Show(string message, Action onYes, Action onNo)
    {
        if (messageText != null)
            messageText.text = message;

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(() =>
        {
            Hide();
            onYes?.Invoke();
        });
        noButton.onClick.AddListener(() =>
        {
            Hide();
            onNo?.Invoke();
        });

        if (panel != null)
            panel.SetActive(true);
    }

    // 예/아니오 버튼 입력을 await로 대기 (UniTaskCompletionSource)
    public UniTask<bool> ShowAsync(string message)
    {
        var tcs = new UniTaskCompletionSource<bool>();
        Show(message, onYes: () => tcs.TrySetResult(true), onNo: () => tcs.TrySetResult(false));
        return tcs.Task;
    }

    void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}
