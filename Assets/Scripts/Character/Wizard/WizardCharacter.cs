using UnityEngine;

public class WizardCharacter : Character
{
    public override ClassType Type => ClassType.Wizard;
    public override Color classColor => Color.blue;

    public override int ApplyPassive(ChainJudge judge, ChainGroup group, int damage)
    {
        // TODO: Wizard passive
        return damage;
    }
}
