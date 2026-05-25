using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    public TextMeshProUGUI slotLabel;
    public TextMeshProUGUI dateLabel;

    [SerializeField]
    Image[] characterIcons = new Image[3];

    [SerializeField]
    Image[] jokerIcons = new Image[5];

    [SerializeField]
    Button[] characterButtons = new Button[3];

    [SerializeField]
    Button[] jokerButtons = new Button[5];

    public int SlotIndex { get; private set; }
    public bool HasData { get; private set; }

    public void Setup(int slotIndex, SaveSlotData data)
    {
        SlotIndex = slotIndex;
        HasData = true;

        if (slotLabel != null)
            slotLabel.text = $"슬롯 {slotIndex + 1}";

        if (dateLabel != null)
        {
            if (
                !string.IsNullOrEmpty(data.clearedAtIso)
                && DateTime.TryParse(data.clearedAtIso, out var dt)
            )
                dateLabel.text = dt.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
            else
                dateLabel.text = "";
        }

        var reg = TableRegistry.Instance;
        for (int i = 0; i < characterIcons.Length; i++)
        {
            Sprite sp = null;
            if (
                reg != null
                && i < data.characterIds.Length
                && !string.IsNullOrEmpty(data.characterIds[i])
            )
                sp = reg.Character.Get(data.characterIds[i])?.prefab.Icon;
            SetSprite(characterIcons[i], sp);
        }

        for (int i = 0; i < jokerIcons.Length; i++)
        {
            Sprite sp = null;
            if (reg != null && i < data.jokerIds.Length && !string.IsNullOrEmpty(data.jokerIds[i]))
                sp = reg.JokerCard.Get(data.jokerIds[i])?.icon;
            SetSprite(jokerIcons[i], sp);
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
}
