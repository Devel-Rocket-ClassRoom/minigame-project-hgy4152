using UnityEngine;

public class HunterCharacter : Character
{
    public override ClassType Type => ClassType.Hunter;
    public override Color classColor => Color.green;

    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        // TODO: Hunter passive
        return damage;
    }
}
