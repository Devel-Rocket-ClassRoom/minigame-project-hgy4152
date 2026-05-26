using UnityEngine;
using UnityEngine.UI;

public class JokerCardSlotUI : MonoBehaviour
{
    Image icon;

    [SerializeField]
    Button button;

    [SerializeField]
    InfoPopupUI infoPopupPrefab;

    JokerCard _card;

    void Awake()
    {
        icon = GetComponentInChildren<Image>(true);
        if (button == null)
            button = GetComponent<Button>();
        if (button != null)
            button.onClick.AddListener(OnClicked);
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

    void OnClicked()
    {
        if (_card == null || infoPopupPrefab == null)
            return;
        InfoPopupUI.ShowJoker(infoPopupPrefab, _card, button.transform as RectTransform);
    }
}
