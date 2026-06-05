using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotInfoPanel : MonoBehaviour
{
    [SerializeField]
    TMP_Text titleText;

    [SerializeField]
    Button backdropButton;

    [SerializeField]
    TMP_Text[] characterTitleTexts = new TMP_Text[3];

    [SerializeField]
    TMP_Text[] characterDescTexts = new TMP_Text[3];

    [SerializeField]
    Image[] characterIcons = new Image[3];

    [SerializeField]
    TMP_Text[] jokerTitleTexts = new TMP_Text[5];

    [SerializeField]
    TMP_Text[] jokerDescTexts = new TMP_Text[5];

    [SerializeField]
    Image[] jokerIcons = new Image[5];

    public System.Action onHide;

    void Awake()
    {
        if (backdropButton != null)
            backdropButton.onClick.AddListener(Hide);
    }

    public void Show(SaveSlotData data, string title)
    {
        gameObject.SetActive(true);
        if (titleText != null)
            titleText.text = title;
        backdropButton?.gameObject.SetActive(true);
        var reg = TableRegistry.Instance;

        for (int i = 0; i < characterIcons.Length; i++)
        {
            CharacterDef def = null;
            if (
                reg != null
                && i < data.characterIds.Length
                && !string.IsNullOrEmpty(data.characterIds[i])
            )
                def = reg.Character.Get(data.characterIds[i]);

            SetIcon(characterIcons[i], def?.prefab.Icon);

            if (i < characterTitleTexts.Length && characterTitleTexts[i] != null)
                characterTitleTexts[i].text = def != null ? Localization.Get(def.displayName) : "";

            if (i < characterDescTexts.Length && characterDescTexts[i] != null)
            {
                if (def != null)
                {
                    var sb = new System.Text.StringBuilder();
                    sb.Append("<color=#7FFF00>");
                    sb.Append(Localization.Get(def.passiveName));
                    sb.Append("</color>\n");
                    sb.Append(Localization.Get(def.description));
                    if (def.blockData != null)
                    {
                        sb.Append("\n\n<color=#FF8C00>");
                        sb.Append(Localization.Get(def.blockData.displayName));
                        sb.Append("</color>\n");
                        sb.Append(Localization.Get(def.blockData.description));
                    }
                    characterDescTexts[i].text = sb.ToString();
                }
                else
                    characterDescTexts[i].text = "";
            }
        }

        for (int i = 0; i < jokerIcons.Length; i++)
        {
            JokerCard card = null;
            if (reg != null && i < data.jokerIds.Length && !string.IsNullOrEmpty(data.jokerIds[i]))
                card = reg.JokerCard.Get(data.jokerIds[i]);

            SetIcon(jokerIcons[i], card?.icon);

            if (i < jokerTitleTexts.Length && jokerTitleTexts[i] != null)
                jokerTitleTexts[i].text = card != null ? Localization.Get(card.cardName) : "";

            if (i < jokerDescTexts.Length && jokerDescTexts[i] != null)
                jokerDescTexts[i].text = card != null ? Localization.Get(card.description) : "";
        }

        Canvas.ForceUpdateCanvases();
    }

    public void Hide()
    {
        backdropButton?.gameObject.SetActive(false);
        gameObject.SetActive(false);
        onHide?.Invoke();
    }

    static void SetIcon(Image img, Sprite sprite)
    {
        if (img == null)
            return;
        img.enabled = sprite != null;
        if (sprite != null)
        {
            img.sprite = sprite;
            img.preserveAspect = true;
        }
    }
}
