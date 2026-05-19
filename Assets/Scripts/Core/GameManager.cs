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

        var context = new ChainJudge();
        context.IngestGroups(groups);

        // 데미지 함수
        int damage = 0;
        damage += (int)(context.chain1Count * 1.1f);
        damage += (int)(context.chain2Count * 1.2f);
        damage += (int)(context.chain3Count * 1.3f);
        damage += (int)(context.classDistribution[ClassType.Warrior] * 1.3f);
        damage += (int)(context.classDistribution[ClassType.Archer] * 1.3f);
        damage += (int)(context.classDistribution[ClassType.Priest] * 1.3f);

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
