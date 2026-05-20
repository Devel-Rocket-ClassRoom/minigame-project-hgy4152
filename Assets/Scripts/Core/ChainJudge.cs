using System.Collections.Generic;

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
    public int[] prevChainCounts = new int[3];
    public List<object> activeModifiers = new();
    public object bossPattern;

    public void IngestGroups(List<ChainGroup> groups)
    {
        foreach (var g in groups)
        {
            if (previousClass == g.DominantClass)
            {
                // 겹쳐서 나오면 false
                isShiftBlock = false;
            }

            if (g.Length == 1)
                chain1Count++;
            else if (g.Length == 2)
                chain2Count++;
            else if (g.Length == 3)
                chain3Count++;

            classDistribution[g.DominantClass] =
                classDistribution.GetValueOrDefault(g.DominantClass) + 1;

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

    }

}
