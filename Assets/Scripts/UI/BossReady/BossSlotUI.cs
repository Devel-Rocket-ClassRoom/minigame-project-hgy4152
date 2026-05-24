using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossSlotUI : MonoBehaviour
{
    public Image portrait;
    public TextMeshProUGUI bossNameLabel;
    public TextMeshProUGUI rarityLabel;

    public System.Action<BossSlotUI> OnSelected;

    public EnemyData Def { get; private set; }

    public void Setup(EnemyData boss)
    {
        Def = boss;

        if (portrait != null && boss.icon != null)
            portrait.sprite = boss.icon;

        if (bossNameLabel != null)
            bossNameLabel.text = boss.enemyName;

        if (rarityLabel != null)
            rarityLabel.text = boss.rarity.ToString();
    }

    public void OnClick() => OnSelected?.Invoke(this);
}
