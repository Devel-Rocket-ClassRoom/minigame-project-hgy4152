using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JokerCardSlotUI : MonoBehaviour
{
    Image icon;
    TextMeshProUGUI cardName;

    void Awake()
    {
        icon = GetComponentInChildren<Image>();
        cardName = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void Refresh(JokerCard card)
    {
        if (card == null)
        {
            icon.enabled = false;
            cardName.text = string.Empty;
            return;
        }

        icon.enabled = true;
        cardName.enabled = true;

        icon.sprite = card.icon;
        cardName.text = card.name;
    }
}
