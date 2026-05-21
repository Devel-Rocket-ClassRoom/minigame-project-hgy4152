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
        if (icon == null)
            icon = GetComponentInChildren<Image>(true);

        if (icon == null)
            return;

        if (card == null)
        {
            icon.enabled = false;
            return;
        }

        icon.enabled = true;
        icon.sprite = card.icon;
    }
}
