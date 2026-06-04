using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyBuffBarUI : MonoBehaviour
{
    [SerializeField]
    BossPatternSystem bossPatternSystem;

    [SerializeField]
    CharacterSet characterSet;

    [SerializeField]
    Transform iconContainer;

    [SerializeField]
    GameObject buffIconPrefab;

    [SerializeField]
    GameObject debuffIconPrefab;

    [SerializeField]
    BuffDebuffInfoPanelUI infoPanel;

    void OnEnable()
    {
        if (bossPatternSystem != null)
            bossPatternSystem.OnInjected += Refresh;
    }

    void OnDisable()
    {
        if (bossPatternSystem != null)
            bossPatternSystem.OnInjected -= Refresh;
    }

    void Start()
    {
        Refresh();
    }

    void Refresh()
    {
        foreach (Transform child in iconContainer)
            Destroy(child.gameObject);

        // 1. 패시브 버프 (항상 표시)
        var reg = TableRegistry.Instance;
        if (characterSet != null && reg != null && reg.Character != null)
        {
            foreach (var id in characterSet.GetCurrentCharacterIds())
            {
                if (string.IsNullOrEmpty(id))
                    continue;
                var def = reg.Character.Get(id);
                if (def == null || def.buffIcon == null)
                    continue;
                var icon = Instantiate(buffIconPrefab, iconContainer);
                icon.GetComponent<Image>().sprite = def.buffIcon;
                icon.GetComponent<Button>().onClick.AddListener(ShowPanel);
            }
        }

        // 2. 보스 모디파이어 디버프
        if (bossPatternSystem != null)
        {
            foreach (var mod in bossPatternSystem.GetActiveModifiers())
            {
                var icon = Instantiate(debuffIconPrefab, iconContainer);
                if (mod.icon != null)
                    icon.GetComponent<Image>().sprite = mod.icon;
                icon.GetComponent<Button>().onClick.AddListener(ShowPanel);
            }
        }
    }

    void ShowPanel()
    {
        if (infoPanel == null)
            return;
        infoPanel.Show(BuildEntries());
    }

    List<BuffDebuffEntry> BuildEntries()
    {
        var entries = new List<BuffDebuffEntry>();
        var reg = TableRegistry.Instance;

        // 패시브
        if (characterSet != null && reg != null && reg.Character != null)
        {
            foreach (var id in characterSet.GetCurrentCharacterIds())
            {
                if (string.IsNullOrEmpty(id))
                    continue;
                var def = reg.Character.Get(id);
                if (def == null || def.buffIcon == null)
                    continue;
                entries.Add(
                    new BuffDebuffEntry
                    {
                        icon = def.buffIcon,
                        name = Localization.Get(def.passiveName),
                        desc = Localization.Get(def.description),
                    }
                );
            }
        }

        // 보스 모디파이어
        if (bossPatternSystem != null)
        {
            foreach (var mod in bossPatternSystem.GetActiveModifiers())
            {
                entries.Add(
                    new BuffDebuffEntry
                    {
                        icon = mod.icon,
                        name = Localization.Get(mod.modName),
                        desc = Localization.Get(mod.description),
                    }
                );
            }
        }

        return entries;
    }
}
