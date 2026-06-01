using System;
using UnityEngine;
using UnityEngine.UI;

public class JokerCardSlotUI : MonoBehaviour
{
    Image icon;

    [SerializeField]
    Button button;

    [SerializeField]
    InfoPopupUI infoPopup;

    [SerializeField]
    Image selectHighlight;

    JokerCard _card;
    bool _swapMode;
    Action<JokerCardSlotUI> _swapClickCallback;

    void Awake()
    {
        icon = GetComponentInChildren<Image>(true);
        if (button == null)
            button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnClicked);
        if (selectHighlight != null)
            selectHighlight.enabled = false;
    }

    public void Refresh(JokerCard card)
    {
        _card = card;

        if (icon == null)
            icon = GetComponentInChildren<Image>(true);

        if (icon == null)
            return;

        if (card == null)
        {
            icon.enabled = false;
            return;
        }

        icon.enabled = true;
        icon.sprite = card.icon;
    }

    public void EnterSwapMode(Action<JokerCardSlotUI> onClicked)
    {
        _swapMode = true;
        _swapClickCallback = onClicked;
        if (selectHighlight != null)
            selectHighlight.enabled = false;
    }

    public void ExitSwapMode()
    {
        _swapMode = false;
        _swapClickCallback = null;
        if (selectHighlight != null)
            selectHighlight.enabled = false;
    }

    public void SetSelected(bool selected)
    {
        if (selectHighlight != null)
            selectHighlight.enabled = selected;
    }

    void OnClicked()
    {
        if (_swapMode)
        {
            _swapClickCallback?.Invoke(this);
            if (_card != null && infoPopup != null)
                infoPopup.ShowJoker(_card);
            return;
        }
        if (_card == null || infoPopup == null)
            return;
        infoPopup.ShowJoker(_card);
    }
}
