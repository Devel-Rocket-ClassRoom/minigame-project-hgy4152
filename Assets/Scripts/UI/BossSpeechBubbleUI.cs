using System.Collections;
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

    public IEnumerator Show(string text)
    {
        if (dialogueText != null)
            dialogueText.text = text;
        if (panel != null)
            panel.SetActive(true);
        yield return new WaitForSeconds(displayDuration);
        if (panel != null)
            panel.SetActive(false);
    }
}
