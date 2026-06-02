using System.Collections.Generic;
using System.Linq;

public class ChainJudge
{
    public int chain1Count;
    public int chain2Count;
    public int chain3Count;
    public Dictionary<ClassType, int> classDistribution = new();

    public bool isShiftBlock = true;
    private ClassType previousClass = ClassType.None;

    public float remainingTimeRatio;
    public int turnIndex;
    public int discardRemaining;
    public int discardUsed;
    public int stageDiscardsUsed;
    public int bossMaxHp;
    public int[] prevChainCounts = new int[3];
    public int bossFlatBonus;
    public float bossDamageMultiplier = 1f;
    public List<Modifier> activeModifiers = new();
    public BossPattern bossPattern;

    // 보스 패턴 확장 필드
    public float[] chainLevelMultiplier = { 1f, 1f, 1f }; // 1/2/3체인 배율
    public bool[] chainLevelNullified = new bool[3]; // 체인 길이별 무효화
    public HashSet<ClassType> classNullified = new(); // 특정 직업 그룹 무효화
    public bool discardBonusDisabled; // 디스카드 보너스 무효
    public bool requireAllThreeClasses; // 3종 직업 필수
    public int skipRightmostJokers; // 우측 조커 N장 무효
    public float nonShiftPenaltyMultiplier = 1f; // 비시프트 시 데미지 배율
    public bool classDiscriminateActive; // 자상공격 활성화
    public float classDiscriminatePerBlock = 0.1f; // 자상공격 블록당 감소율
    public Dictionary<ClassType, int> blockDistribution = new(); // 직업별 블록 수
    public Dictionary<ClassType, int> prevClassDistribution = new(); // 이전 턴 직업 분포
    public int[] chainSequence; // 이번 턴 그룹 길이 배열 (등장 순서)
    public int[] prevChainSequence; // 이전 턴 그룹 길이 배열
    public IReadOnlyDictionary<ClassType, int> discardsByClass; // 이번 턴 클래스별 디스카드 수
    public Dictionary<ClassType, int[]> chainCountByClass = new(); // [0]=1체인 [1]=2체인 [2]=3체인

    public void ClearDebuffs()
    {
        bossFlatBonus = 0;
        bossDamageMultiplier = 1f;
        chainLevelMultiplier = new float[] { 1f, 1f, 1f };
        chainLevelNullified = new bool[3];
        classNullified.Clear();
        discardBonusDisabled = false;
        requireAllThreeClasses = false;
        skipRightmostJokers = 0;
        nonShiftPenaltyMultiplier = 1f;
        classDiscriminateActive = false;
    }

    public void IngestGroups(List<ChainGroup> groups)
    {
        chainSequence = groups.Select(g => g.Length).ToArray();
        foreach (var g in groups)
        {
            if (previousClass == g.DominantClass)
                isShiftBlock = false;

            if (g.Length == 1)
                chain1Count++;
            else if (g.Length == 2)
                chain2Count++;
            else if (g.Length == 3)
                chain3Count++;

            classDistribution[g.DominantClass] =
                classDistribution.GetValueOrDefault(g.DominantClass) + 1;

            IngestChainCountByClass(g);
            previousClass = g.DominantClass;
        }
    }

    public void IngestGroup(ChainGroup g)
    {
        if (g.Length == 1)
            chain1Count++;
        else if (g.Length == 2)
            chain2Count++;
        else if (g.Length == 3)
            chain3Count++;

        classDistribution[g.DominantClass] =
            classDistribution.GetValueOrDefault(g.DominantClass) + 1;

        IngestChainCountByClass(g);
    }

    void IngestChainCountByClass(ChainGroup g)
    {
        if (!chainCountByClass.ContainsKey(g.DominantClass))
            chainCountByClass[g.DominantClass] = new int[3];
        chainCountByClass[g.DominantClass][g.Length - 1]++;
    }

    public void IngestHand(System.Collections.Generic.List<Block> hand)
    {
        foreach (var block in hand)
        {
            if (block == null || block.data == null)
                continue;
            var cls = block.data.ownerClass;
            blockDistribution[cls] = blockDistribution.GetValueOrDefault(cls) + 1;
        }
    }
}
