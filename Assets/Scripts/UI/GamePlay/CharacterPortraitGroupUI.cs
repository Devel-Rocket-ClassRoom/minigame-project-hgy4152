using UnityEngine;

public class CharacterPortraitGroupUI : MonoBehaviour
{
    [SerializeField]
    CharacterPortraitUI[] slots;

    [SerializeField]
    CharacterSet characterSet;

    [SerializeField]
    InfoPopupUI infoPopup;

    void Start()
    {
        if (characterSet == null)
            characterSet = FindObjectOfType<CharacterSet>();

        BindSlots();
    }

    void BindSlots()
    {
        var reg = TableRegistry.Instance;
        if (reg == null || reg.Character == null)
        {
            Debug.LogWarning("[CharacterPortraitGroupUI] TableRegistry를 찾을 수 없습니다.");
            return;
        }

        var ids =
            characterSet != null
                ? characterSet.GetCurrentCharacterIds()
                : System.Array.Empty<string>();

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                continue;

            CharacterDef def = null;
            Sprite icon = null;

            if (i < ids.Length && !string.IsNullOrEmpty(ids[i]))
            {
                def = reg.Character.Get(ids[i]);
                if (def != null)
                {
                    var character = characterSet.GetCharacter(def.classType);
                    if (character != null)
                        icon = character.Icon;
                }
            }

            slots[i].Bind(def, icon);

            var slot = slots[i];
            slot.OnClicked -= OnPortraitClicked;
            slot.OnClicked += OnPortraitClicked;
        }
    }

    void OnPortraitClicked(CharacterPortraitUI slot)
    {
        if (infoPopup != null && slot.Def != null)
            infoPopup.ShowCharacter(slot.Def);
    }
}
