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

    void PopulateAll()
    {
        var reg = TableRegistry.Instance;
        if (reg == null)
            return;

        // 캐릭터 탭: ClassType 순서별 panel
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
                foreach (var def in defs)
                    Instantiate(slotPrefab, p.Grid)
                        .Setup(
                            def.prefab.Icon,
                            def.displayName,
                            UnlockManager.IsCharacterUnlocked(def.id)
                        );
            }
        }

        // 조커 탭: Rarity 순서별 panel
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
                foreach (var j in jokers)
                    Instantiate(slotPrefab, p.Grid)
                        .Setup(j.icon, j.cardName, UnlockManager.IsJokerUnlocked(j.id));
            }
        }

        // 몬스터 탭: 일반 panel + 보스 panel
        if (monsterContent != null)
        {
            if (reg.Enemy != null && reg.Enemy.All.Count > 0)
            {
                var p = Instantiate(panelPrefab, monsterContent);
                p.SetHeader("일반");
                foreach (var e in reg.Enemy.All)
                    Instantiate(slotPrefab, p.Grid)
                        .Setup(e.icon, e.enemyName, UnlockManager.IsEnemyUnlocked(e.id));
            }

            if (reg.Boss != null && reg.Boss.All.Count > 0)
            {
                var p = Instantiate(panelPrefab, monsterContent);
                p.SetHeader("보스");
                foreach (var b in reg.Boss.All)
                    Instantiate(slotPrefab, p.Grid)
                        .Setup(b.icon, b.bossName, UnlockManager.IsBossUnlocked(b.id));
            }
        }
    }
}
