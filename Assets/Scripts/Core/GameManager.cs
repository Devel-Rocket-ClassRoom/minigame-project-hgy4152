using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    DrawPhaseTimer drawPhaseTimer;

    [SerializeField]
    BlockManager blockManager;

    [SerializeField]
    BossController boss;

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

        // 데미지 함수
        int damage = 0;
        damage += (int)(judge.chain1Count * 1.1f);
        damage += (int)(judge.chain2Count * 1.2f);
        damage += (int)(judge.chain3Count * 1.3f);
        damage += (int)(judge.classDistribution.GetValueOrDefault(ClassType.Warrior) * 1.3f);
        damage += (int)(judge.classDistribution.GetValueOrDefault(ClassType.Archer) * 1.3f);
        damage += (int)(judge.classDistribution.GetValueOrDefault(ClassType.Priest) * 1.3f);

        int atk = 0;
        foreach (var g in groups)
        {
            // 같은 스킬끼리 그룹으로 뭉치기 때문에 전부 같은 값을 가진 블럭들임
            // 그래서 가장 첫번째 인덱스인 0으로 하드 코딩함
            atk += g.Blocks[0].data.attackPower;
        }

        boss.TakeDamage(damage * atk);
        Debug.Log($"[GameManager] Settled: {damage * atk} dmg");
    }
}
