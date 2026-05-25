using UnityEngine;

public class PaladinCharacter : Character
{
    public override ClassType Type => ClassType.Paladin;
    public override Color classColor => Color.white;

    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        // TODO: Paladin passive
        return damage;
    }
}
