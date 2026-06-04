using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSlotUI : MonoBehaviour
{
    public Image portrait;
    public GameObject maskOverlay;
    public GameObject deployButton;
    public GameObject deployedLabel;
    public GameObject removeButton;

    public System.Action<CharacterSlotUI> OnSelected;
    public System.Action<CharacterSlotUI> OnDeployClicked;
    public System.Action<CharacterSlotUI> OnRemoveClicked;

    public CharacterDef Def { get; private set; }
    public bool IsInParty { get; private set; }

    public void Setup(CharacterDef def, bool isInParty)
    {
        Def = def;
        IsInParty = isInParty;

        if (portrait != null && def.prefab != null && def.prefab.Icon != null)
        {
            portrait.sprite = def.prefab.Icon;
            portrait.preserveAspect = true;
        }

        UpdateVisuals(false);
    }

    public void SetInParty(bool inParty)
    {
        IsInParty = inParty;
        UpdateVisuals(false);
    }

    public void UpdateVisuals(bool isSelected)
    {
        bool showMask = IsInParty || isSelected;
        bool showDeploy = isSelected && !IsInParty;
        bool showDeployed = IsInParty && !isSelected;
        bool showRemove = IsInParty && isSelected;

        if (maskOverlay != null)
            maskOverlay.SetActive(showMask);
        if (deployButton != null)
            deployButton.SetActive(showDeploy);
        if (deployedLabel != null)
            deployedLabel.SetActive(showDeployed);
        if (removeButton != null)
            removeButton.SetActive(showRemove);
    }

    public void OnClick() => OnSelected?.Invoke(this);

    public void OnDeployClick() => OnDeployClicked?.Invoke(this);

    public void OnRemoveClick() => OnRemoveClicked?.Invoke(this);
}
