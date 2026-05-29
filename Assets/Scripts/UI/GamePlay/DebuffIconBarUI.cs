using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DebuffIconBarUI : MonoBehaviour
{
    [SerializeField]
    BossPatternSystem bossPatternSystem;

    [SerializeField]
    Transform iconContainer;

    [SerializeField]
    GameObject debuffIconPrefab;

    [SerializeField]
    DebuffInfoPopupUI debuffInfoPopup;

    void OnEnable()
    {
        bossPatternSystem.OnInjected += Refresh;
    }

    void OnDisable()
    {
        bossPatternSystem.OnInjected -= Refresh;
    }

    void Refresh()
    {
        foreach (Transform child in iconContainer)
            Destroy(child.gameObject);

        var mods = bossPatternSystem.GetActiveModifiers().ToList();
        gameObject.SetActive(mods.Count > 0);

        foreach (var mod in mods)
        {
            var captured = mod;
            var icon = Instantiate(debuffIconPrefab, iconContainer);
            icon.GetComponent<Button>()
                .onClick.AddListener(() =>
                    debuffInfoPopup.Show(bossPatternSystem.GetActiveModifiers())
                );
        }
    }
}
