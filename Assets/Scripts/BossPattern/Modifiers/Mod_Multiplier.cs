using UnityEngine;

[CreateAssetMenu(menuName = "Boss/Modifier/Multiplier")]
public class Mod_Multiplier : Modifier
{
    public float factor = 1f;

    public override void Apply(ChainJudge j) => j.bossDamageMultiplier *= factor;
}
