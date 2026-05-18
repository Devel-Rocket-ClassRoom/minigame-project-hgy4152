using System.Collections.Generic;

public class GameContext
{
    public int chain1Count;
    public int chain2Count;
    public int chain3Count;
    public Dictionary<ClassType, int> classDistribution = new();

    public void IngestGroups(List<ChainGroup> groups)
    {
        foreach (var g in groups)
        {
            if (g.Length == 1)
                chain1Count++;
            else if (g.Length == 2)
                chain2Count++;
            else if (g.Length == 3)
                chain3Count++;

            // 없는 키면 만들어서 추가하게 끔
            classDistribution[g.DominantClass] =
                classDistribution.GetValueOrDefault(g.DominantClass) + 1;
        }
    }
}
