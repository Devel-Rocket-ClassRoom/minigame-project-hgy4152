using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JokerCardSlotUI : MonoBehaviour
{
    [SerializeField]
    Image icon;

    [SerializeField]
    TextMeshProUGUI cardName;

    public void Refresh(JokerCard card)
    {
        if (card == null)
        {
            icon.enabled = false;
            cardName.text = string.Empty;
            return;
        }

        icon.enabled = true;
        icon.sprite = card.icon;
        cardName.text = card.name;
    }
}
