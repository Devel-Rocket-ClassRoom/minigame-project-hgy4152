using System.Collections.Generic;

public interface IGameCommand
{
    void Execute();
    void Undo();
}

// 보스 플레이 핸드 1회를 기록하는 커맨드.
// Execute가 핸드 시작 전 상태(메멘토)를 캡처하고, Undo가 그 시점으로 복원한다.
// 실제 핸드 정산은 게임 루프(GameManager)가 수행한다.
public class HandPlayCommand : IGameCommand
{
    readonly EnemyController boss;
    readonly BlockManager blockManager;
    readonly BossPatternSystem patternSystem;
    readonly CharacterSet characterSet;

    int _bossHp;
    (int used, Dictionary<ClassType, int> byClass) _stageDiscards;
    (int[] chainCounts, Dictionary<ClassType, int> classDist, int[] chainSequence) _patternSnapshot;
    Dictionary<Character, object> _characterStates;

    public HandPlayCommand(
        EnemyController boss,
        BlockManager blockManager,
        BossPatternSystem patternSystem,
        CharacterSet characterSet
    )
    {
        this.boss = boss;
        this.blockManager = blockManager;
        this.patternSystem = patternSystem;
        this.characterSet = characterSet;
    }

    public void Execute()
    {
        _bossHp = boss.CurrentHp;
        _stageDiscards = blockManager.CaptureStageDiscards();
        if (patternSystem != null)
            _patternSnapshot = patternSystem.CaptureSnapshot();

        _characterStates = new Dictionary<Character, object>();
        if (characterSet != null)
            foreach (var c in characterSet.GetInstances())
                _characterStates[c] = c.CaptureState();
    }

    public void Undo()
    {
        boss.RestoreHp(_bossHp);
        blockManager.RestoreStageDiscards(_stageDiscards);
        patternSystem?.RestoreSnapshot(_patternSnapshot);

        if (_characterStates != null)
            foreach (var kv in _characterStates)
                if (kv.Key != null)
                    kv.Key.RestoreState(kv.Value);
    }
}

// 커맨드 스택 — UndoAll이 역순으로 전부 되돌려 구간 시작(첫 핸드 직전) 상태로 복원
public class CommandHistory
{
    readonly Stack<IGameCommand> _stack = new();

    public int Count => _stack.Count;

    public void Push(IGameCommand command)
    {
        command.Execute();
        _stack.Push(command);
    }

    public void UndoAll()
    {
        while (_stack.Count > 0)
            _stack.Pop().Undo();
    }

    public void Clear() => _stack.Clear();
}
