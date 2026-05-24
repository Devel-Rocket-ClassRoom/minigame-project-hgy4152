using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    public TextMeshProUGUI slotLabel;
    public TextMeshProUGUI characterSummary;
    public TextMeshProUGUI dateLabel;
    public GameObject emptyOverlay;
    public Image selectionHighlight;

    public System.Action<SaveSlotUI> OnSelected;

    public int SlotIndex { get; private set; }
    public bool HasData { get; private set; }

    public void Setup(int slotIndex, SaveSlotData data)
    {
        SlotIndex = slotIndex;
        HasData = true;

        if (slotLabel != null)
            slotLabel.text = $"슬롯 {slotIndex + 1}";

        if (characterSummary != null)
        {
            var table = TableRegistry.Instance?.Character;
            var names = new System.Text.StringBuilder();
            foreach (var id in data.characterIds)
            {
                if (string.IsNullOrEmpty(id))
                    continue;
                if (table != null && table.TryGet(id, out var def))
                    names.AppendLine(Localization.Get(def.displayName));
                else
                    names.AppendLine(id);
            }
            characterSummary.text = names.ToString().TrimEnd();
        }

        if (dateLabel != null && !string.IsNullOrEmpty(data.clearedAtIso))
        {
            if (DateTime.TryParse(data.clearedAtIso, out var dt))
                dateLabel.text = dt.ToLocalTime().ToString("yyyy-MM-dd HH:mm");
            else
                dateLabel.text = data.clearedAtIso;
        }

        if (emptyOverlay != null)
            emptyOverlay.SetActive(false);
    }

    public void SetupEmpty(int slotIndex)
    {
        SlotIndex = slotIndex;
        HasData = false;

        if (slotLabel != null)
            slotLabel.text = $"슬롯 {slotIndex + 1}";
        if (characterSummary != null)
            characterSummary.text = "";
        if (dateLabel != null)
            dateLabel.text = "";
        if (emptyOverlay != null)
            emptyOverlay.SetActive(true);
    }

    public void SetSelected(bool selected)
    {
        if (selectionHighlight != null)
            selectionHighlight.enabled = selected;
    }

    public void OnClick() => OnSelected?.Invoke(this);
}
