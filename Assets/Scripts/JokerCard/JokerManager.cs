using UnityEngine;

public class JokerManager : MonoBehaviour
{
    [SerializeField]
    JokerCardHandUI handUI;

    [SerializeField]
    JokerCard[] activeHand = new JokerCard[5];

    public JokerCard[] ActiveHand => activeHand;

    void Start()
    {
        handUI.Refresh(activeHand);
    }

    public void SetCard(int slot, JokerCard card)
    {
        if (slot < 0 || slot >= activeHand.Length)
            return;
        activeHand[slot] = card;
        handUI.Refresh(activeHand);
    }

    public void SetHand(JokerCard[] cards)
    {
        for (int i = 0; i < activeHand.Length; i++)
            activeHand[i] = i < cards.Length ? cards[i] : null;
        handUI.Refresh(activeHand);
    }
}
