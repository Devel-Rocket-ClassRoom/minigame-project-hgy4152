using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JokerRewardUI : MonoBehaviour
{
    [SerializeField]
    GameObject panel;

    [SerializeField]
    GameObject[] cardButtonObjects = new GameObject[3];

    [SerializeField]
    JokerManager jokerManager;

    [SerializeField]
    GameManager gameManager;

    [SerializeField]
    Button skipButton;

    [SerializeField]
    Button confirmButton;

    [SerializeField]
    GameObject swapPromptPanel;

    [SerializeField]
    Image swapNewCardImage;

    [SerializeField]
    Button swapConfirmButton;

    [SerializeField]
    Button cancelButton;

    [SerializeField]
    JokerCardHandUI jokerHandUI;

    Button[] _buttons;
    Image[] _selectFrames;
    Image[] _cardImages;
    GameObject[] _tooltipPanels;
    TMP_Text[] _nameTexts;
    TMP_Text[] _descTexts;

    JokerCard[] _offered = new JokerCard[3];
    int _selectedIdx = -1;
    JokerCard _pendingSwapCard;

    void Awake()
    {
        _buttons = new Button[cardButtonObjects.Length];
        _selectFrames = new Image[cardButtonObjects.Length];
        _cardImages = new Image[cardButtonObjects.Length];
        _tooltipPanels = new GameObject[cardButtonObjects.Length];
        _nameTexts = new TMP_Text[cardButtonObjects.Length];
        _descTexts = new TMP_Text[cardButtonObjects.Length];

        for (int i = 0; i < cardButtonObjects.Length; i++)
        {
            var root = cardButtonObjects[i].transform;
            _buttons[i] = root.GetComponent<Button>();
            _selectFrames[i] = root.GetComponent<Image>();
            _cardImages[i] = root.GetChild(0).GetComponent<Image>();
            var tp = root.GetChild(1);
            _tooltipPanels[i] = tp.gameObject;
            _nameTexts[i] = tp.GetChild(0).GetComponent<TMP_Text>();
            _descTexts[i] = tp.GetChild(1).GetComponent<TMP_Text>();
            _tooltipPanels[i].SetActive(false);
        }

        if (panel != null)
            panel.SetActive(false);
        if (skipButton != null)
            skipButton.gameObject.SetActive(false);
        if (confirmButton != null)
            confirmButton.gameObject.SetActive(false);
        if (swapPromptPanel != null)
            swapPromptPanel.SetActive(false);
        if (cancelButton != null)
            cancelButton.gameObject.SetActive(false);
    }

    void Start()
    {
        for (int i = 0; i < _buttons.Length; i++)
        {
            int idx = i;
            _buttons[i].onClick.AddListener(() => OnCardClicked(idx));
        }
        skipButton?.onClick.AddListener(Skip);
        confirmButton?.onClick.AddListener(() =>
        {
            if (_selectedIdx >= 0)
                ConfirmPick(_selectedIdx);
        });
        swapConfirmButton?.onClick.AddListener(ConfirmSwap);
        cancelButton?.onClick.AddListener(CancelSwap);
    }

    public void Show()
    {
        _offered = PickRandom(3);
        _selectedIdx = -1;

        for (int i = 0; i < 3; i++)
        {
            bool valid = i < _offered.Length && _offered[i] != null;
            cardButtonObjects[i].SetActive(valid);
            if (_selectFrames[i] != null)
                _selectFrames[i].enabled = false;
            if (valid)
            {
                _cardImages[i].sprite = _offered[i].icon;
                _cardImages[i].preserveAspect = true;
            }
        }

        HideAllTooltips();
        panel.SetActive(true);
        skipButton.gameObject.SetActive(true);
        confirmButton?.gameObject.SetActive(true);
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
        for (int i = 0; i < _selectFrames.Length; i++)
            if (_selectFrames[i] != null)
                _selectFrames[i].enabled = i == idx;

        _nameTexts[idx].text = Localization.Get(_offered[idx].cardName);
        _descTexts[idx].text = Localization.Get(_offered[idx].description);
        _tooltipPanels[idx].SetActive(true);
        for (int i = 0; i < _tooltipPanels.Length; i++)
            if (i != idx) _tooltipPanels[i].SetActive(false);
    }

    void HideAllTooltips()
    {
        foreach (var tp in _tooltipPanels)
            tp.SetActive(false);
    }

    void ConfirmPick(int idx)
    {
        JokerCard card = idx < _offered.Length ? _offered[idx] : null;

        HideAllTooltips();
        for (int i = 0; i < _selectFrames.Length; i++)
            if (_selectFrames[i] != null)
                _selectFrames[i].enabled = false;
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
            _pendingSwapCard = card;
            if (swapNewCardImage != null)
            {
                swapNewCardImage.enabled = true;
                swapNewCardImage.sprite = card.icon;
                swapNewCardImage.preserveAspect = true;
            }
            swapPromptPanel?.SetActive(true);
            cancelButton?.gameObject.SetActive(true);
            jokerHandUI?.EnterSwapMode();
        }
    }

    void CancelSwap()
    {
        jokerHandUI?.ExitSwapMode();
        swapPromptPanel?.SetActive(false);
        cancelButton?.gameObject.SetActive(false);
        _pendingSwapCard = null;

        for (int i = 0; i < 3; i++)
        {
            bool valid = i < _offered.Length && _offered[i] != null;
            cardButtonObjects[i].SetActive(valid);
            if (_selectFrames[i] != null)
                _selectFrames[i].enabled = false;
        }
        HideAllTooltips();
        panel.SetActive(true);
        skipButton.gameObject.SetActive(true);
        confirmButton?.gameObject.SetActive(true);
    }

    void ConfirmSwap()
    {
        if (jokerHandUI == null || jokerHandUI.SelectedSlotIndex < 0)
            return;

        int selectedSlot = jokerHandUI.SelectedSlotIndex;
        jokerHandUI.ExitSwapMode();
        swapPromptPanel?.SetActive(false);
        cancelButton?.gameObject.SetActive(false);

        jokerManager.SetCard(selectedSlot, _pendingSwapCard);
        _pendingSwapCard = null;
        FinishReward();
    }

    public void Skip()
    {
        HideAllTooltips();
        for (int i = 0; i < _selectFrames.Length; i++)
            if (_selectFrames[i] != null)
                _selectFrames[i].enabled = false;
        _selectedIdx = -1;
        panel.SetActive(false);

        FinishReward();
    }

    void FinishReward()
    {
        skipButton.gameObject.SetActive(false);
        confirmButton?.gameObject.SetActive(false);
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
