using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardSwapUI : MonoBehaviour
{
    [SerializeField]
    GameObject panel;

    [SerializeField]
    JokerCardSlotUI[] handSlots;

    [SerializeField]
    Button[] slotButtons;

    [SerializeField]
    Image newCardImage;

    [SerializeField]
    TextMeshProUGUI newCardName;

    System.Action<int> _onSlotPicked;

    void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public void Show(JokerCard[] currentHand, JokerCard newCard, System.Action<int> onSlotPicked)
    {
        _onSlotPicked = onSlotPicked;

        for (int i = 0; i < handSlots.Length; i++)
            handSlots[i].Refresh(i < currentHand.Length ? currentHand[i] : null);

        if (newCardImage != null)
        {
            newCardImage.enabled = newCard != null;
            if (newCard != null)
                newCardImage.sprite = newCard.icon;
        }

        if (newCardName != null && newCard != null)
            newCardName.text = newCard.cardName;

        for (int i = 0; i < slotButtons.Length; i++)
        {
            int idx = i;
            slotButtons[i].onClick.RemoveAllListeners();
            slotButtons[i].onClick.AddListener(() => OnSlotClicked(idx));
        }

        panel.SetActive(true);
    }

    void OnSlotClicked(int idx)
    {
        panel.SetActive(false);
        var cb = _onSlotPicked;
        _onSlotPicked = null;
        cb?.Invoke(idx);
    }
}
