using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EncyclopediaSlotUI : MonoBehaviour
{
    [SerializeField]
    Image iconImage;

    [SerializeField]
    TMP_Text nameText;

    [SerializeField]
    Color lockedIconTint = new Color(0.15f, 0.15f, 0.15f, 1f);

    public void Setup(Sprite icon, string nameKey, bool isUnlocked, Action onClicked = null)
    {
        if (iconImage != null)
        {
            iconImage.sprite = icon;
            iconImage.preserveAspect = true;
            iconImage.color = isUnlocked ? Color.white : lockedIconTint;
        }
        if (nameText != null)
            nameText.text = isUnlocked ? Localization.Get(nameKey) : "???";

        var btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.interactable = onClicked != null;
            if (onClicked != null)
                btn.onClick.AddListener(() => onClicked());
        }
    }
}
