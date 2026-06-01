using UnityEngine;

public class JokerCardHandUI : MonoBehaviour
{
    JokerCardSlotUI[] slots;
    JokerCardSlotUI _selectedSlot;

    void Awake()
    {
        slots = GetComponentsInChildren<JokerCardSlotUI>();
    }

    public void Refresh(JokerCard[] cards)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                continue;
            slots[i].Refresh(i < cards.Length ? cards[i] : null);
        }
    }

    public void EnterSwapMode()
    {
        _selectedSlot = null;
        foreach (var slot in slots)
            slot.EnterSwapMode(OnSlotSelectedInSwapMode);
    }

    public void ExitSwapMode()
    {
        _selectedSlot = null;
        foreach (var slot in slots)
            slot.ExitSwapMode();
    }

    public int SelectedSlotIndex =>
        _selectedSlot != null ? System.Array.IndexOf(slots, _selectedSlot) : -1;

    void OnSlotSelectedInSwapMode(JokerCardSlotUI slot)
    {
        _selectedSlot = slot;
        foreach (var s in slots)
            s.SetSelected(s == slot);
    }
}
