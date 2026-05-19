using UnityEngine;

public class SkillCardHandUI : MonoBehaviour
{
    [SerializeField]
    SkillCardSlotUI[] slots = new SkillCardSlotUI[5];

    public void Refresh(SkillCard[] cards)
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i].Refresh(i < cards.Length ? cards[i] : null);
    }
}
