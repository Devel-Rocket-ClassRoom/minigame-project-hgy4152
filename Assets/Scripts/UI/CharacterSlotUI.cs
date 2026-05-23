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

    public Character Character { get; private set; }
    public bool IsInParty { get; private set; }

    public void Setup(Character character, bool isInParty)
    {
        Character = character;
        IsInParty = isInParty;
        
        if (portrait != null && character.Icon != null)
            portrait.sprite = character.Icon;
            
        if (inPartyLbl != null)
        {
            inPartyLbl.text = "출전 중";
        }
        
        UpdateState(false);
    }

    public void UpdateState(bool isSelected)
    {
        if (highlight != null)
            highlight.SetActive(isSelected);
            
        if (partyOverlay != null)
            partyOverlay.SetActive(isSelected && IsInParty);
    }

    public void OnClick()
    {
        OnSelected?.Invoke(this);
    }
}
