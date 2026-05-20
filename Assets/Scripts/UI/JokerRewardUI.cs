using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JokerRewardUI : MonoBehaviour
{
    [SerializeField]
    GameObject panel;

    [SerializeField]
    Button[] cardButtons = new Button[3];

    [SerializeField]
    TMP_Text[] cardNameTexts = new TMP_Text[3];

    [SerializeField]
    TMP_Text[] cardDescTexts = new TMP_Text[3];

    [SerializeField]
    JokerCard[] rewardPool;

    [SerializeField]
    JokerManager jokerManager;

    [SerializeField]
    StageManager stageManager;

    [SerializeField]
    GameManager gameManager;

    JokerCard[] _offered = new JokerCard[3];

    void Awake()
    {
        if (panel != null) panel.SetActive(false);
    }

    void Start()
    {
        for (int i = 0; i < cardButtons.Length; i++)
        {
            int idx = i;
            cardButtons[i].onClick.AddListener(() => SelectCard(idx));
        }
    }

    public void Show()
    {
        _offered = PickRandom(3);
        for (int i = 0; i < 3; i++)
        {
            bool valid = i < _offered.Length && _offered[i] != null;
            cardButtons[idx_to_use(i)].gameObject.SetActive(valid);
            if (valid)
            {
                cardNameTexts[i].text = _offered[i].cardName;
                cardDescTexts[i].text = _offered[i].description;
            }
        }
        panel.SetActive(true);
    }

    private int idx_to_use(int i) => i; // Helper to avoid closure issues if needed, but array access is fine

    void SelectCard(int idx)
    {
        if (idx < _offered.Length && _offered[idx] != null)
        {
            int slot = FindEmptySlot();
            if (slot >= 0)
                jokerManager.SetCard(slot, _offered[idx]);
        }
        panel.SetActive(false);
        gameManager.SetPaused(false);
        stageManager.AdvanceToNext();
    }

    int FindEmptySlot()
    {
        var hand = jokerManager.ActiveHand;
        for (int i = 0; i < hand.Length; i++)
            if (hand[i] == null)
                return i;
        return -1;
    }

    JokerCard[] PickRandom(int count)
    {
        if (rewardPool == null || rewardPool.Length == 0)
            return new JokerCard[0];

        var pool = new List<JokerCard>(rewardPool);
        int take = Mathf.Min(count, pool.Count);
        var result = new JokerCard[take];
        for (int i = 0; i < take; i++)
        {
            int r = Random.Range(0, pool.Count);
            result[i] = pool[r];
            pool.RemoveAt(r);
        }
        return result;
    }
}
