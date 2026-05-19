using UnityEngine;

public class JokerCardHandUI : MonoBehaviour
{
    JokerCardSlotUI[] slots;

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
}
