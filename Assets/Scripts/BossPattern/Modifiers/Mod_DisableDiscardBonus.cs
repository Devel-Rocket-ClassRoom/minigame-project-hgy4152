using UnityEngine;

[CreateAssetMenu(
    fileName = "Mod_DisableDiscardBonus",
    menuName = "Boss/Modifier/DisableDiscardBonus"
)]
public class Mod_DisableDiscardBonus : Modifier
{
    public override void Apply(ChainJudge judge)
    {
        judge.discardBonusDisabled = true;
    }
}
