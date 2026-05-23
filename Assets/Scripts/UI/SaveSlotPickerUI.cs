using System;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotPickerUI : MonoBehaviour
{
    [SerializeField]
    GameObject panel;

    [SerializeField]
    SaveSlotEntryUI previewEntry;

    [SerializeField]
    SaveSlotEntryUI[] slotEntries = new SaveSlotEntryUI[SaveManager.SlotCount];

    [SerializeField]
    Button[] slotButtons = new Button[SaveManager.SlotCount];

    [SerializeField]
    Button cancelButton;

    void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public void Show(
        SaveManager mgr,
        SaveSlotData draft,
        Action<int> onSlotPicked,
        Action onCanceled
    )
    {
        if (panel != null)
            panel.SetActive(true);

        if (previewEntry != null)
            previewEntry.Refresh(-1, draft, TableRegistry.Instance);

        var reg = TableRegistry.Instance;
        for (int i = 0; i < SaveManager.SlotCount; i++)
        {
            int slot = i;
            SaveSlotData data = mgr.TryLoad(slot, out var loaded) ? loaded : null;

            if (i < slotEntries.Length && slotEntries[i] != null)
                slotEntries[i].Refresh(slot, data, reg);

            if (i < slotButtons.Length && slotButtons[i] != null)
            {
                slotButtons[i].onClick.RemoveAllListeners();
                slotButtons[i]
                    .onClick.AddListener(() =>
                    {
                        Hide();
                        onSlotPicked?.Invoke(slot);
                    });
            }
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(() =>
            {
                Hide();
                onCanceled?.Invoke();
            });
        }
    }

    void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}
