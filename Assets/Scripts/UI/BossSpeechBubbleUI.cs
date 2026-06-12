using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class BossSpeechBubbleUI : MonoBehaviour
{
    [SerializeField]
    GameObject panel;

    [SerializeField]
    TMP_Text dialogueText;

    [SerializeField]
    float displayDuration = 2f;

    void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public async UniTask ShowAsync(string text)
    {
        if (dialogueText != null)
            dialogueText.text = text;
        if (panel != null)
            panel.SetActive(true);
        await UniTask.Delay(
            TimeSpan.FromSeconds(displayDuration),
            cancellationToken: this.GetCancellationTokenOnDestroy()
        );
        if (panel != null)
            panel.SetActive(false);
    }
}
