using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossSlotUI : MonoBehaviour
{
    public Image portrait;
    public TextMeshProUGUI bossNameLabel;
    public TextMeshProUGUI rarityLabel;

    [SerializeField] GameObject lockOverlay;

    public BossData Def { get; private set; }

    public void SetLocked(bool locked)
    {
        if (lockOverlay != null)
            lockOverlay.SetActive(locked);
    }

    public void Setup(BossData boss)
    {
        Def = boss;

        if (portrait != null && boss.icon != null)
        {
            portrait.sprite = boss.icon;
            portrait.preserveAspect = true;
        }

        if (bossNameLabel != null)
            bossNameLabel.text = Localization.Get(boss.bossName);

        if (rarityLabel != null)
            rarityLabel.text = Localization.Get($"ui_rarity_{boss.rarity.ToString().ToLower()}");
    }
}
