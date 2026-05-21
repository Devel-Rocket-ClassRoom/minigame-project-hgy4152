using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JokerCardSlotUI : MonoBehaviour
{
    Image icon;

    void Awake()
    {
        icon = GetComponentInChildren<Image>(true);
    }

    public void Refresh(JokerCard card)
    {
        if (card == null)
        {
            icon.enabled = false;
            return;
        }

        icon.enabled = true;

        icon.sprite = card.icon;
    }
}
