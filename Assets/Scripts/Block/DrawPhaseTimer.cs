using System;
using System.Collections;
using UnityEngine;

public class DrawPhaseTimer : MonoBehaviour
{
    const float PhaseDuration = 20f;

    [SerializeField]
    float drawInterval = 1.5f;

    BlockManager blockManager;

    void Awake()
    {
        blockManager = GetComponent<BlockManager>();
    }

    public event Action OnPhaseEnded;

    Coroutine _phaseCoroutine;
    float _endTime;

    public float RemainingRatio =>
        _phaseCoroutine != null ? Mathf.Clamp01((_endTime - Time.time) / PhaseDuration) : 0f;

    void Start()
    {
        StartDrawPhase();
    }

    public void StartDrawPhase()
    {
        if (_phaseCoroutine != null)
            StopCoroutine(_phaseCoroutine);
        blockManager.ResetDiscardCount();
        _phaseCoroutine = StartCoroutine(DrawPhaseRoutine());
    }

    public void StopDrawPhase()
    {
        if (_phaseCoroutine == null)
            return;
        StopCoroutine(_phaseCoroutine);
        _phaseCoroutine = null;
    }

    public void PlayHandNow()
    {
        StopDrawPhase();
        EndPhase();
    }

    IEnumerator DrawPhaseRoutine()
    {
        _endTime = Time.time + PhaseDuration;

        while (Time.time < _endTime)
        {
            blockManager.DrawBlock();
            yield return new WaitForSeconds(drawInterval);
        }

        _phaseCoroutine = null;
        EndPhase();
    }

    void EndPhase()
    {
        blockManager.DrawUntilFull(); // 일찍 턴 끝낼 시 부족한 카드 수 자동 채움
        blockManager.DisableDiscard(); // 버리기 비활성화
        Debug.Log($"[DrawPhaseTimer] Phase ended. Final hand: {blockManager.hand.Count}/12");
        OnPhaseEnded?.Invoke();
    }
}
