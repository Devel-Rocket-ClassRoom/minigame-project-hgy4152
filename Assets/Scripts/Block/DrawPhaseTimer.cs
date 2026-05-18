using System;
using System.Collections;
using UnityEngine;

public class DrawPhaseTimer : MonoBehaviour
{
    const float PhaseDuration = 20f;

    [SerializeField]
    BlockManager blockManager;

    [SerializeField]
    float drawInterval = 1.5f;

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
        OnPhaseEnded?.Invoke();
    }

    IEnumerator DrawPhaseRoutine()
    {
        _endTime = Time.time + PhaseDuration;
        Debug.Log("[DrawPhaseTimer] Draw phase started (20s).");

        while (Time.time < _endTime)
        {
            blockManager.DrawBlock();
            yield return new WaitForSeconds(drawInterval);
        }

        _phaseCoroutine = null;
        Debug.Log($"[DrawPhaseTimer] Phase ended. Final hand: {blockManager.hand.Count}/12");
        OnPhaseEnded?.Invoke();
    }
}
