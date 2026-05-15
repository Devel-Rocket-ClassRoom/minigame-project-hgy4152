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

    IEnumerator DrawPhaseRoutine()
    {
        float endTime = Time.time + PhaseDuration;
        Debug.Log("[DrawPhaseTimer] Draw phase started (20s).");

        while (Time.time < endTime)
        {
            Block drawn = blockManager.DrawBlock();
            if (drawn == null)
            {
                Debug.Log("[DrawPhaseTimer] Hand full (12/12). Stopping draw.");
                break;
            }
            Debug.Log(
                $"[DrawPhaseTimer] Hand: {blockManager.hand.Count}/12 | Remaining: {endTime - Time.time:F1}s"
            );
            yield return new WaitForSeconds(drawInterval);
        }

        _phaseCoroutine = null;
        Debug.Log($"[DrawPhaseTimer] Phase ended. Final hand: {blockManager.hand.Count}/12");
        OnPhaseEnded?.Invoke();
    }
}
