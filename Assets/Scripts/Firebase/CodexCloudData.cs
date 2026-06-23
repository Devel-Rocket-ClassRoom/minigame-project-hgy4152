using System;
using System.Collections.Generic;

[Serializable]
public class CodexCloudData
{
    public List<string> unlockedCharacterIds;
    public List<string> unlockedJokerIds;
    public List<string> defeatedEnemyIds;
    public List<string> defeatedBossIds;
    public int adventureClearCount;
    public int bossModeClearCount;
    public int chain1Used;
    public int chain2Used;
    public int chain3Used;
    public int blocksDiscarded;
    public List<ClassClearEntryCloud> classClearCounts;
}

[Serializable]
public class ClassClearEntryCloud
{
    public string classType;
    public int count;
}
