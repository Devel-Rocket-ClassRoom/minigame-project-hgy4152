using System.Collections.Generic;
using UnityEngine;

// 데미지 계산 전담 (GameManager에서 추출). MonoBehaviour 아님 — 순수 계산 + 패시브 통지.
public class DamageCalculator
{
    readonly JokerManager jokerManager;
    readonly CharacterSet characterSet;

    public DamageCalculator(JokerManager jokerManager, CharacterSet characterSet)
    {
        this.jokerManager = jokerManager;
        this.characterSet = characterSet;
    }

    public (HashSet<int> skipJokerIndices, float deckBonus) BuildJokerContext(ChainJudge judge)
    {
        var skipJokerIndices = new HashSet<int>();
        if (judge.skipRightmostJokers > 0 && jokerManager != null)
        {
            int skipped = 0;
            for (
                int k = jokerManager.ActiveHand.Length - 1;
                k >= 0 && skipped < judge.skipRightmostJokers;
                k--
            )
            {
                if (jokerManager.ActiveHand[k] != null)
                {
                    skipJokerIndices.Add(k);
                    skipped++;
                }
            }
        }

        float deckBonus = 1f;
        if (jokerManager != null)
            for (int k = 0; k < jokerManager.ActiveHand.Length; k++)
                if (jokerManager.ActiveHand[k] != null && !skipJokerIndices.Contains(k))
                    deckBonus *= jokerManager.ActiveHand[k].DeckBonus(judge);

        return (skipJokerIndices, deckBonus);
    }

    // boss 배율·시프트 페널티는 CalcSingleGroupDamage에서 ApplyPassive 이전 적용 (Hikari flat 보너스 수치 보존)
    public float CalcPartyBonus(ChainJudge judge, float deckBonus)
    {
        bool protection = false;
        float partyPassiveBonus = 0f;

        if (characterSet != null)
            foreach (var c in characterSet.GetInstances())
            {
                if (c.IsProtectionPassive(judge))
                    protection = true;
                else
                    partyPassiveBonus += c.GetPartyBonus(judge);
            }

        if (protection)
            judge.ClearDebuffs();

        return (1f + partyPassiveBonus) * deckBonus;
    }

    public int CalcSingleGroupDamage(
        ChainGroup group,
        ChainJudge judge,
        HashSet<int> skipJokerIndices,
        float partyBonus
    )
    {
        var character = group.DominantCharacter;

        int jokerGroupBonus = 0;
        if (jokerManager != null)
            for (int k = 0; k < jokerManager.ActiveHand.Length; k++)
                if (jokerManager.ActiveHand[k] != null && !skipJokerIndices.Contains(k))
                    jokerGroupBonus += jokerManager.ActiveHand[k].GetBonus(judge, group);

        int dmg = CalcGroupDamage(group, judge);
        dmg -= judge.bossFlatBonus;
        dmg += jokerGroupBonus;

        float chainBonus = character?.GetChainTypeBonus(judge, group) ?? 0f;
        float classBonus = character?.GetClassTypeBonus(judge, group) ?? 0f;
        foreach (var m in judge.activeModifiers)
        {
            chainBonus -= m.GetChainBonusPenalty(group);
            classBonus -= m.GetClassBonusPenalty(group);
        }
        if (chainBonus != 0f || classBonus != 0f)
            dmg = Mathf.RoundToInt(dmg * (1f + chainBonus + classBonus));

        if (judge.classDiscriminateActive)
        {
            int classBlocks = judge.blockDistribution.GetValueOrDefault(group.DominantClass);
            dmg = Mathf.FloorToInt(
                dmg * Mathf.Max(0f, 1f - judge.classDiscriminatePerBlock * classBlocks)
            );
        }

        // 보스 데미지 배율·시프트 페널티: ApplyPassive 이전 적용 (flat 보너스 패시브 수치 보존)
        dmg = Mathf.FloorToInt(dmg * (2f - judge.bossDamageMultiplier));
        if (!judge.isShiftBlock && judge.nonShiftPenaltyMultiplier != 1f)
            dmg = Mathf.FloorToInt(dmg * judge.nonShiftPenaltyMultiplier);

        // 미이관 패시브 (ApplyPassive 유지: Hikari, AhnMansik)
        dmg = character?.ApplyPassive(judge, group, dmg) ?? dmg;

        int rawDmg = dmg;
        dmg = Mathf.FloorToInt(dmg * partyBonus);

        character?.ApplyDebuffPassive(judge, group);
        characterSet?.NotifyAnyGroupDamage(rawDmg, dmg);
        return dmg;
    }

    public int[] CalcDamages(List<ChainGroup> groups, ChainJudge judge)
    {
        var (skipJokerIndices, deckBonus) = BuildJokerContext(judge);
        float partyBonus = CalcPartyBonus(judge, deckBonus);
        var result = new int[groups.Count];
        for (int i = 0; i < groups.Count; i++)
            result[i] = CalcSingleGroupDamage(groups[i], judge, skipJokerIndices, partyBonus);
        return result;
    }

    int CalcGroupDamage(ChainGroup group, ChainJudge judge)
    {
        int idx = group.Length - 1;
        if (idx < 0 || idx > 2)
            return 0;
        if (judge.chainLevelNullified[idx])
            return 0;
        if (judge.classNullified.Contains(group.DominantClass))
            return 0;
        if (judge.requireAllThreeClasses && judge.classDistribution.Count < 3)
            return 0;

        float baseMul = group.Length switch
        {
            2 => 1.1f,
            3 => 1.2f,
            _ => 1f,
        };
        baseMul *= judge.chainLevelMultiplier[idx];
        int ap = characterSet?.GetDef(group.DominantCharacter)?.attackPower ?? 10;
        return Mathf.FloorToInt(baseMul * ap * group.Length);
    }

    public static int[] SplitDamageWeighted(int total, int chainLength)
    {
        var result = new int[chainLength];
        int weightSum = chainLength * (chainLength + 1) / 2;
        int accumulated = 0;
        for (int i = 0; i < chainLength - 1; i++)
        {
            result[i] = Mathf.RoundToInt(total * (i + 1) / (float)weightSum);
            accumulated += result[i];
        }
        result[chainLength - 1] = total - accumulated;
        return result;
    }
}
