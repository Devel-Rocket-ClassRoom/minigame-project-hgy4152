using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillCardSlotUI : MonoBehaviour
{
    [SerializeField]
    Image icon;

    [SerializeField]
    TextMeshProUGUI cardName;

    public void Refresh(SkillCard card)
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
