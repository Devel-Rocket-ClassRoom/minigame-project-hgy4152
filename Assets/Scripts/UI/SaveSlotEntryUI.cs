using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotEntryUI : MonoBehaviour
{
    [SerializeField]
    TMP_Text slotLabel;

    [SerializeField]
    TMP_Text dateText;

    [SerializeField]
    Image[] characterIcons = new Image[3];

    [SerializeField]
    Image[] jokerIcons = new Image[5];

    [SerializeField]
    GameObject emptyOverlay;

    public void Refresh(int slotIndex, SaveSlotData data, TableRegistry reg)
    {
        if (slotLabel != null)
            slotLabel.text = $"슬롯 {slotIndex + 1}";

        bool isEmpty = data == null;
        if (emptyOverlay != null)
            emptyOverlay.SetActive(isEmpty);

        if (isEmpty)
        {
            if (dateText != null)
                dateText.text = "비어있음";
            ClearIcons(characterIcons);
            ClearIcons(jokerIcons);
            return;
        }

        if (
            dateText != null
            && System.DateTime.TryParse(
                data.clearedAtIso,
                null,
                System.Globalization.DateTimeStyles.RoundtripKind,
                out var dt
            )
        )
            dateText.text = dt.ToLocalTime().ToString("yy/MM/dd HH:mm");

        for (int i = 0; i < characterIcons.Length; i++)
        {
            Sprite sp = null;
            if (i < data.characterIds.Length && !string.IsNullOrEmpty(data.characterIds[i]))
                sp = reg.Character.Get(data.characterIds[i])?.icon;
            SetSprite(i < characterIcons.Length ? characterIcons[i] : null, sp);
        }

        for (int i = 0; i < jokerIcons.Length; i++)
        {
            Sprite sp = null;
            if (i < data.jokerIds.Length && !string.IsNullOrEmpty(data.jokerIds[i]))
                sp = reg.JokerCard.Get(data.jokerIds[i])?.icon;
            SetSprite(i < jokerIcons.Length ? jokerIcons[i] : null, sp);
        }
    }

    static void SetSprite(Image img, Sprite sprite)
    {
        if (img == null)
            return;
        img.enabled = sprite != null;
        if (sprite != null)
            img.sprite = sprite;
    }

    static void ClearIcons(Image[] icons)
    {
        foreach (var img in icons)
            if (img != null)
                img.enabled = false;
    }
}
