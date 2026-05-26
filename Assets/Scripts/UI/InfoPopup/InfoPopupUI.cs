using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPopupUI : MonoBehaviour
{
    [SerializeField]
    GameObject panel;

    [SerializeField]
    Button backdropButton;

    [SerializeField]
    TMP_Text nameText;

    [SerializeField]
    TMP_Text descText;

    void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
        if (backdropButton != null)
            backdropButton.onClick.AddListener(Close);
    }

    public void ShowCharacter(CharacterDef def)
    {
        if (nameText != null)
            nameText.text = Localization.Get(def.DisplayName);
        if (descText != null)
            descText.text = Localization.Get(((IDisplayable)def).Description);
        if (panel != null)
            panel.SetActive(true);
    }

    public void ShowJoker(JokerCard card)
    {
        if (nameText != null)
            nameText.text = Localization.Get(card.cardName);
        if (descText != null)
            descText.text = Localization.Get(card.description);
        if (panel != null)
            panel.SetActive(true);
    }

    public void ShowEnemy(EnemyData data)
    {
        if (nameText != null)
            nameText.text = Localization.Get(data.enemyName);
        if (descText != null)
            descText.text = Localization.Get(data.description);
        if (panel != null)
            panel.SetActive(true);
    }

    public void Close()
    {
        if (panel != null)
            panel.SetActive(false);
    }
}
