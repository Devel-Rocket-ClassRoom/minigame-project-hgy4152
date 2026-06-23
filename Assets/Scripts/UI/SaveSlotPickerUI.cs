using System;
using Cysharp.Threading.Tasks;
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

    // 슬롯 선택을 await로 대기. 취소 시 -1 반환.
    public UniTask<int> ShowAsync(SaveManager mgr, SaveSlotData draft)
    {
        var tcs = new UniTaskCompletionSource<int>();
        Show(mgr, draft, onSlotPicked: slot => tcs.TrySetResult(slot), onCanceled: () => tcs.TrySetResult(-1));
        return tcs.Task;
    }

    void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}
