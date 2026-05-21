using TMPro;
using UnityEngine;

public class CardSwapUI : MonoBehaviour
{
    [SerializeField]
    GameObject panel;

    [SerializeField]
    BlockManager blockManager;

    [SerializeField]
    TMP_Text promptText;

    void OnEnable()
    {
        blockManager.OnDrawBlocked += Show;
    }

    void OnDisable()
    {
        blockManager.OnDrawBlocked -= Show;
    }

    void Awake()
    {
        if (panel != null) panel.SetActive(false);
    }

    void Show()
    {
        if (panel != null) panel.SetActive(true);
        if (promptText != null) promptText.text = "버릴 카드를 선택하세요";

        foreach (var block in blockManager.hand)
            block.OnDiscardRequested = OnCardSelected;
    }

    void OnCardSelected(Block block)
    {
        if (panel != null) panel.SetActive(false);

        foreach (var b in blockManager.hand)
            b.OnDiscardRequested = blockManager.Discard;

        blockManager.Discard(block);
    }
}
