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
    Image[] selectFrames = new Image[3];

    [SerializeField]
    Image[] cardImage = new Image[3];

    [SerializeField]
    RectTransform tooltipPanel;

    [SerializeField]
    TMP_Text tooltipNameText;

    [SerializeField]
    TMP_Text tooltipDescText;

    [SerializeField]
    JokerManager jokerManager;

    [SerializeField]
    GameManager gameManager;

    [SerializeField]
    CardSwapUI cardSwapUI;

    [SerializeField]
    Button skipButton;

    JokerCard[] _offered = new JokerCard[3];
    int _selectedIdx = -1;

    void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
        if (tooltipPanel != null)
            tooltipPanel.gameObject.SetActive(false);
    }

    void Start()
    {
        for (int i = 0; i < cardButtons.Length; i++)
        {
            int idx = i;
            cardButtons[i].onClick.AddListener(() => OnCardClicked(idx));
        }
        skipButton?.onClick.AddListener(Skip);
    }

    public void Show()
    {
        _offered = PickRandom(3);
        _selectedIdx = -1;

        for (int i = 0; i < 3; i++)
        {
            bool valid = i < _offered.Length && _offered[i] != null;
            cardButtons[i].gameObject.SetActive(valid);
            if (selectFrames[i] != null)
                selectFrames[i].enabled = false;
            if (valid)
                cardImage[i].sprite = _offered[i].icon;
        }

        tooltipPanel.gameObject.SetActive(false);
        panel.SetActive(true);
    }

    void OnCardClicked(int idx)
    {
        if (_selectedIdx == idx)
        {
            ConfirmPick(idx);
        }
        else
        {
            _selectedIdx = idx;
            ShowSelection(idx);
        }
    }

    void ShowSelection(int idx)
    {
        for (int i = 0; i < selectFrames.Length; i++)
            if (selectFrames[i] != null)
                selectFrames[i].enabled = i == idx;

        tooltipNameText.text = Localization.Get(_offered[idx].cardName);
        tooltipDescText.text = Localization.Get(_offered[idx].description);
        PositionTooltip(idx);
        tooltipPanel.gameObject.SetActive(true);
    }

    void PositionTooltip(int idx)
    {
        var btnRect = cardButtons[idx].GetComponent<RectTransform>();
        float width = cardImage[idx].GetComponent<RectTransform>().rect.width;
        float centerPos = cardButtons[cardButtons.Length / 2]
            .GetComponent<RectTransform>()
            .localPosition.x;

        bool isRight = btnRect.localPosition.x > centerPos;
        float offsetX = isRight ? width : -width;

        // 버튼 월드 좌표 → 툴팁 패널 부모 로컬 좌표로 변환
        Vector2 btnInParent = tooltipPanel.parent.InverseTransformPoint(btnRect.position);
        tooltipPanel.localPosition = btnInParent + new Vector2(offsetX, 0);
    }

    void ConfirmPick(int idx)
    {
        JokerCard card = idx < _offered.Length ? _offered[idx] : null;

        tooltipPanel.gameObject.SetActive(false);
        for (int i = 0; i < selectFrames.Length; i++)
            if (selectFrames[i] != null)
                selectFrames[i].enabled = false;
        _selectedIdx = -1;
        panel.SetActive(false);

        if (card == null)
        {
            FinishReward();
            return;
        }

        int slot = FindEmptySlot();
        if (slot >= 0)
        {
            jokerManager.SetCard(slot, card);
            FinishReward();
        }
        else
        {
            cardSwapUI.Show(
                jokerManager.ActiveHand,
                card,
                pickedSlot =>
                {
                    jokerManager.SetCard(pickedSlot, card);
                    FinishReward();
                }
            );
        }
    }

    public void Skip()
    {
        tooltipPanel.gameObject.SetActive(false);
        for (int i = 0; i < selectFrames.Length; i++)
            if (selectFrames[i] != null)
                selectFrames[i].enabled = false;
        _selectedIdx = -1;
        panel.SetActive(false);
        FinishReward();
    }

    void FinishReward()
    {
        gameManager.BeginBattle();
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
        var allCards = TableRegistry.Instance?.JokerCard?.All;
        if (allCards == null || allCards.Count == 0)
            return new JokerCard[0];

        var ownedIds = new HashSet<string>();
        foreach (var c in jokerManager.ActiveHand)
            if (c != null)
                ownedIds.Add(c.id);

        var pools = new Dictionary<Rarity, List<JokerCard>>
        {
            { Rarity.Common, new List<JokerCard>() },
            { Rarity.Rare, new List<JokerCard>() },
            { Rarity.Epic, new List<JokerCard>() },
        };
        foreach (var card in allCards)
            if (
                card != null
                && !ownedIds.Contains(card.id)
                && UnlockManager.IsJokerUnlocked(card.id)
            )
                pools[card.rarity].Add(card);

        var available = new List<JokerCard>();
        foreach (var list in pools.Values)
            available.AddRange(list);

        int take = Mathf.Min(count, available.Count);
        var result = new JokerCard[take];
        for (int i = 0; i < take; i++)
        {
            var picked = PickWeighted(pools, available);
            result[i] = picked;
            pools[picked.rarity].Remove(picked);
            available.Remove(picked);
        }
        return result;
    }

    JokerCard PickWeighted(Dictionary<Rarity, List<JokerCard>> pools, List<JokerCard> available)
    {
        int roll = Random.Range(0, 100);
        Rarity target =
            roll < 60 ? Rarity.Common
            : roll < 90 ? Rarity.Rare
            : Rarity.Epic;

        if (pools[target].Count > 0)
        {
            var pool = pools[target];
            return pool[Random.Range(0, pool.Count)];
        }
        return available[Random.Range(0, available.Count)];
    }
}
