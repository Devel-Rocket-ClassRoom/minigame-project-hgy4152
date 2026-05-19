using UnityEngine;

public class JokerCardHandUI : MonoBehaviour
{
    [SerializeField]
    JokerCardSlotUI[] slots = new JokerCardSlotUI[5];

    public void Refresh(JokerCard[] cards)
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i].Refresh(i < cards.Length ? cards[i] : null);
    }
}
