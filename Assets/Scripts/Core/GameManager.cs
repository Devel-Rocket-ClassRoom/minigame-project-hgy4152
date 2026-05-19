using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    DrawPhaseTimer drawPhaseTimer;

    [SerializeField]
    BlockManager blockManager;

    [SerializeField]
    BossController boss;

    [SerializeField]
    JokerManager jokerManager;

    [SerializeField]
    CharacterSet characterSet;

    [SerializeField]
    float perGroupDelay = 0.4f;

    void OnEnable()
    {
        drawPhaseTimer.OnPhaseEnded += Settle;
    }

    void OnDisable()
    {
        drawPhaseTimer.OnPhaseEnded -= Settle;
    }

    void Settle()
    {
        var groups = ChainResolver.ResolveChains(blockManager.hand);

        var judge = new ChainJudge();
        judge.IngestGroups(groups);
        judge.remainingTimeRatio = drawPhaseTimer.RemainingRatio;
        judge.discardRemaining = blockManager.DiscardsRemaining;

        int jokerBonus = 0;
        if (jokerManager != null)
            foreach (var card in jokerManager.ActiveHand)
                if (card != null)
                    jokerBonus += card.GetBonus(judge);

        StartCoroutine(PlayGroupSequence(groups, judge, jokerBonus));
    }

    IEnumerator PlayGroupSequence(List<ChainGroup> groups, ChainJudge judge, int jokerBonus)
    {
        int totalGroups = Mathf.Max(1, groups.Count);
        foreach (var group in groups)
        {
            var character = characterSet?.GetCharacter(group.DominantClass);
            character?.PlayAttack();

            int groupDmg = CalcGroupDamage(group);
            groupDmg += jokerBonus / totalGroups;
            groupDmg = character?.ApplyPassive(judge, groupDmg) ?? groupDmg;

            boss.TakeDamage(groupDmg);
            Debug.Log($"[GameManager] Group {group.DominantClass} x{group.Length}: {groupDmg} dmg");

            blockManager.RemoveGroup(group);
            yield return new WaitForSeconds(perGroupDelay);
        }

        drawPhaseTimer.StartDrawPhase();
    }

    int CalcGroupDamage(ChainGroup group)
    {
        float chainMul = group.Length switch
        {
            1 => 1.1f,
            2 => 1.2f,
            3 => 1.3f,
            _ => 1f,
        };
        return (int)(chainMul * group.Blocks[0].data.attackPower * group.Length);
    }
}
