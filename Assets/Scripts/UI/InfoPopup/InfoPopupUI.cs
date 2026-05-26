using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPopupUI : MonoBehaviour
{
    [SerializeField]
    RectTransform contentPanel;

    [SerializeField]
    Button backdropButton;

    [SerializeField]
    TMP_Text nameText;

    [SerializeField]
    TMP_Text descText;

    void Awake()
    {
        if (backdropButton != null)
            backdropButton.onClick.AddListener(() => Destroy(gameObject));
    }

    public void Init(string displayName, string desc, RectTransform anchor)
    {
        if (nameText != null)
            nameText.text = displayName;
        if (descText != null)
            descText.text = desc;

        if (contentPanel != null)
            contentPanel.localPosition = contentPanel.parent.InverseTransformPoint(anchor.position);
    }

    public static InfoPopupUI ShowCharacter(
        InfoPopupUI prefab,
        CharacterDef def,
        RectTransform anchor
    )
    {
        return Spawn(
            prefab,
            anchor,
            Localization.Get(def.DisplayName),
            Localization.Get(((IDisplayable)def).Description)
        );
    }

    public static InfoPopupUI ShowJoker(InfoPopupUI prefab, JokerCard card, RectTransform anchor)
    {
        return Spawn(
            prefab,
            anchor,
            Localization.Get(card.cardName),
            Localization.Get(card.description)
        );
    }

    public static InfoPopupUI ShowEnemy(InfoPopupUI prefab, EnemyData data, RectTransform anchor)
    {
        return Spawn(
            prefab,
            anchor,
            Localization.Get(data.enemyName),
            Localization.Get(data.description)
        );
    }

    static InfoPopupUI Spawn(
        InfoPopupUI prefab,
        RectTransform anchor,
        string displayName,
        string desc
    )
    {
        if (prefab == null)
            return null;
        var canvas = anchor.GetComponentInParent<Canvas>();
        if (canvas == null)
            return null;
        var instance = Instantiate(prefab, canvas.transform);
        instance.Init(displayName, desc, anchor);
        return instance;
    }
}
