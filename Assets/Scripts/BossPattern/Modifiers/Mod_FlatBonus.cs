using UnityEngine;

[CreateAssetMenu(menuName = "Boss/Modifier/FlatBonus")]
public class Mod_FlatBonus : Modifier
{
    public int amount;

    public override void Apply(ChainJudge j) => j.bossFlatBonus += amount;
}
