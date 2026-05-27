using System;
using System.Collections.Generic;
using UnityEngine;

public class EncyclopediaUI : MonoBehaviour
{
    [SerializeField]
    GameObject panel;

    [SerializeField]
    EncyclopediaSlotUI slotPrefab;

    [SerializeField]
    EncyclopediaPanelUI panelPrefab;

    [Header("Tab Contents (ScrollView Content)")]
    [SerializeField]
    Transform characterContent;

    [SerializeField]
    Transform jokerContent;

    [SerializeField]
    Transform monsterContent;

    [Header("Tab Roots")]
    [SerializeField]
    GameObject characterTab;

    [SerializeField]
    GameObject jokerTab;

    [SerializeField]
    GameObject monsterTab;

    readonly Dictionary<string, EncyclopediaPanelUI> _charPanels = new();
    readonly Dictionary<string, EncyclopediaPanelUI> _jokerPanels = new();
    readonly Dictionary<string, EncyclopediaPanelUI> _monsterPanels = new();

    void Awake()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    void Start()
    {
        PopulateAll();
    }

    public void Open()
    {
        if (panel != null)
            panel.SetActive(true);
        ShowTab(0);
    }

    public void Close()
    {
        if (panel != null)
            panel.SetActive(false);
    }

    public void ShowTab(int index)
    {
        if (characterTab != null)
            characterTab.SetActive(index == 0);
        if (jokerTab != null)
            jokerTab.SetActive(index == 1);
        if (monsterTab != null)
            monsterTab.SetActive(index == 2);
    }

    // Category 버튼 OnClick에서 호출. filter="" 또는 "All" 이면 전체 표시
    public void FilterCharacter(string filter) => ApplyFilter(_charPanels, filter);

    public void FilterJoker(string filter) => ApplyFilter(_jokerPanels, filter);

    public void FilterMonster(string filter) => ApplyFilter(_monsterPanels, filter);

    static void ApplyFilter(Dictionary<string, EncyclopediaPanelUI> panels, string filter)
    {
        bool showAll = string.IsNullOrEmpty(filter) || filter == "All";
        foreach (var kv in panels)
            kv.Value.gameObject.SetActive(showAll || kv.Key == filter);
    }

    void PopulateAll()
    {
        var reg = TableRegistry.Instance;
        if (reg == null)
            return;

        if (characterContent != null && reg.Character != null)
        {
            var byClass = new Dictionary<ClassType, List<CharacterDef>>();
            foreach (var def in reg.Character.All)
            {
                if (def == null)
                    continue;
                if (!byClass.TryGetValue(def.classType, out var list))
                    byClass[def.classType] = list = new List<CharacterDef>();
                list.Add(def);
            }

            foreach (ClassType ct in Enum.GetValues(typeof(ClassType)))
            {
                if (ct == ClassType.None)
                    continue;
                if (!byClass.TryGetValue(ct, out var defs) || defs.Count == 0)
                    continue;

                var p = Instantiate(panelPrefab, characterContent);
                p.SetHeader(ct.ToString());
                _charPanels[ct.ToString()] = p;
                foreach (var def in defs)
                    Instantiate(slotPrefab, p.Grid)
                        .Setup(
                            def.prefab.Icon,
                            def.displayName,
                            UnlockManager.IsCharacterUnlocked(def.id)
                        );
            }
        }

        if (jokerContent != null && reg.JokerCard != null)
        {
            var byRarity = new Dictionary<Rarity, List<JokerCard>>();
            foreach (var j in reg.JokerCard.All)
            {
                if (j == null)
                    continue;
                if (!byRarity.TryGetValue(j.rarity, out var list))
                    byRarity[j.rarity] = list = new List<JokerCard>();
                list.Add(j);
            }

            foreach (Rarity r in Enum.GetValues(typeof(Rarity)))
            {
                if (!byRarity.TryGetValue(r, out var jokers) || jokers.Count == 0)
                    continue;

                var p = Instantiate(panelPrefab, jokerContent);
                p.SetHeader(r.ToString());
                _jokerPanels[r.ToString()] = p;
                foreach (var j in jokers)
                    Instantiate(slotPrefab, p.Grid)
                        .Setup(j.icon, j.cardName, UnlockManager.IsJokerUnlocked(j.id));
            }
        }

        if (monsterContent != null)
        {
            var byType =
                new Dictionary<EnemyType, List<(Sprite icon, string nameKey, bool unlocked)>>();

            if (reg.Enemy != null)
                foreach (var e in reg.Enemy.All)
                {
                    if (e == null)
                        continue;
                    if (!byType.TryGetValue(e.enemyType, out var list))
                        byType[e.enemyType] = list = new List<(Sprite, string, bool)>();
                    list.Add((e.icon, e.enemyName, UnlockManager.IsEnemyUnlocked(e.id)));
                }

            if (reg.Boss != null)
                foreach (var b in reg.Boss.All)
                {
                    if (b == null)
                        continue;
                    if (!byType.TryGetValue(b.enemyType, out var list))
                        byType[b.enemyType] = list = new List<(Sprite, string, bool)>();
                    list.Add((b.icon, b.bossName, UnlockManager.IsBossUnlocked(b.id)));
                }

            foreach (EnemyType et in Enum.GetValues(typeof(EnemyType)))
            {
                if (!byType.TryGetValue(et, out var entries) || entries.Count == 0)
                    continue;
                var p = Instantiate(panelPrefab, monsterContent);
                p.SetHeader(et.ToString());
                _monsterPanels[et.ToString()] = p;
                foreach (var (icon, nameKey, unlocked) in entries)
                    Instantiate(slotPrefab, p.Grid).Setup(icon, nameKey, unlocked);
            }
        }
    }
}
