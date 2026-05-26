using System;
using UnityEngine;

public class EncyclopediaUI : MonoBehaviour
{
    [SerializeField]
    GameObject panel;

    [SerializeField]
    EncyclopediaSlotUI slotPrefab;

    [Header("Tab Contents")]
    [SerializeField]
    Transform characterGrid;

    [SerializeField]
    Transform jokerGrid;

    [SerializeField]
    Transform monsterGrid;

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

        if (characterGrid != null && reg.Character != null)
            foreach (var def in reg.Character.All)
                Instantiate(slotPrefab, characterGrid).Setup(def.prefab.Icon, def.displayName);

        if (jokerGrid != null && reg.JokerCard != null)
            foreach (var j in reg.JokerCard.All)
                Instantiate(slotPrefab, jokerGrid).Setup(j.icon, j.cardName);

        if (monsterGrid != null)
        {
            if (reg.Enemy != null)
                foreach (var e in reg.Enemy.All)
                    Instantiate(slotPrefab, monsterGrid).Setup(e.icon, e.enemyName);

            if (reg.Boss != null)
                foreach (var b in reg.Boss.All)
                    Instantiate(slotPrefab, monsterGrid).Setup(b.icon, b.bossName);
        }
    }
}
