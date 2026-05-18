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
        int damage = 0;
        foreach (var g in groups)
            damage += g.Length * g.Blocks[0].data.attackPower;

        boss.TakeDamage(damage);
        Debug.Log($"[GameManager] Settled: {damage} dmg from {groups.Count} groups");
    }
}
