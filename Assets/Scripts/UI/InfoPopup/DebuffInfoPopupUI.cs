using System.Collections.Generic;
using TMPro;
using UnityEngine;

// InfoPopupUI의 Content 안에 위치. 슬라이드/백드롭은 InfoPopupUI가 담당.
public class DebuffInfoPopupUI : MonoBehaviour
{
    [SerializeField]
    Transform entryContainer;

    [SerializeField]
    GameObject modifierEntryPrefab;

    public void Populate(IEnumerable<Modifier> modifiers)
    {
        foreach (Transform child in entryContainer)
            Destroy(child.gameObject);

        foreach (var mod in modifiers)
        {
            var entry = Instantiate(modifierEntryPrefab, entryContainer);
            var texts = entry.GetComponentsInChildren<TMP_Text>();
            if (texts.Length >= 1)
                texts[0].text = Localization.Get(mod.modName);
            if (texts.Length >= 2)
                texts[1].text = Localization.Get(mod.description);
        }
    }
}
