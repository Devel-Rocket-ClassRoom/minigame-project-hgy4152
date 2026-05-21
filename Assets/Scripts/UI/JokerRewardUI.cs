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
    JokerCard[] rewardPool;

    [SerializeField]
    JokerManager jokerManager;

    [SerializeField]
    GameManager gameManager;

    [SerializeField]
    CardSwapUI cardSwapUI;

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

        tooltipNameText.text = _offered[idx].cardName;
        tooltipDescText.text = _offered[idx].description;
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
