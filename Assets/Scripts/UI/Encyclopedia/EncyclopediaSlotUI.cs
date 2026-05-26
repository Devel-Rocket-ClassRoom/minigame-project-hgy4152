using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaSlotUI : MonoBehaviour
{
    [SerializeField]
    Image iconImage;

    [SerializeField]
    TMP_Text nameText;

    public void Setup(Sprite icon, string nameKey)
    {
        if (iconImage != null)
            iconImage.sprite = icon;
        if (nameText != null)
            nameText.text = Localization.Get(nameKey);
    }
}
