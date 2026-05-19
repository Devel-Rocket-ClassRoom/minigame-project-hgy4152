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
            // 애니메이션 실행
            var character = characterSet?.GetCharacter(group.DominantClass);
            character?.PlayAttack();

            
            int groupDmg = CalcGroupDamage(group);

            // 조커 보너스 부분 나중에 손봐야함
            // 보너스 받은량을 지금 전체 그룹 수로 나눠서 배분중이라 바꿔야함
            groupDmg += jokerBonus / totalGroups;

            // 각자 패시브 실행(널 연산자)
            groupDmg = character?.ApplyPassive(judge, groupDmg) ?? groupDmg; 

            boss.TakeDamage(groupDmg);
            Debug.Log($"[GameManager] Group {group.DominantClass} x{group.Length}: {groupDmg} dmg");

            blockManager.RemoveGroup(group);
            yield return new WaitForSeconds(perGroupDelay);
        }

        drawPhaseTimer.StartDrawPhase();
    }

    // 블럭 자체 데미지 상승 로직
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
