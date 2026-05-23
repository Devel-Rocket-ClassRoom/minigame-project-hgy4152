using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSlotUI : MonoBehaviour
{
    public Image portrait;
    public GameObject highlight;
    public GameObject partyOverlay;
    public TextMeshProUGUI inPartyLbl;
    
    public System.Action<CharacterSlotUI> OnSelected;

    public CharacterDef Def { get; private set; }
    public bool IsInParty { get; private set; }

    public void Setup(CharacterDef def, bool isInParty)
    {
        Def = def;
        IsInParty = isInParty;

        if (portrait != null && def.icon != null)
            portrait.sprite = def.icon;

        if (inPartyLbl != null)
        {
            inPartyLbl.text = "장착중";
            inPartyLbl.gameObject.SetActive(true);
        }

        if (partyOverlay != null)
            partyOverlay.SetActive(isInParty);

        UpdateState(false);
    }

    public void UpdateState(bool isSelected)
    {
        if (highlight != null)
            highlight.SetActive(isSelected);
    }

    public void OnClick()
    {
        OnSelected?.Invoke(this);
    }
}
