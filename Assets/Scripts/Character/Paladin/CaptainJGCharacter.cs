using UnityEngine;

[RequireComponent(typeof(PaladinCreator))]
[RequireComponent(typeof(Skill_CaptainJG))]
public class CaptainJGCharacter : Character
{
    public override ClassType Type => ClassType.Paladin;
    public override Color classColor => Color.white;

    // 파도부대 선장: 3체인 공격 시 무작위 다른 파티원 1체인 발동
    public override void OnAfterGroupPlayed(CharacterSet characterSet, ChainGroup group)
    {
        if (group.DominantClass != Type || group.Length != 3)
            return;

        var others = System.Array.FindAll(characterSet.GetDeployedClassTypes(), t => t != Type);
        if (others.Length == 0)
            return;

        var targetType = others[Random.Range(0, others.Length)];
        characterSet.GetCharacter(targetType)?.PlaySkillEffect(1, null, null);
    }
}
