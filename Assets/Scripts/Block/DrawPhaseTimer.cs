using System;
using System.Collections;
using UnityEngine;

public class DrawPhaseTimer : MonoBehaviour
{
    [SerializeField]
    float defaultPhaseDuration = 20f;

    [SerializeField]
    float drawInterval = 1.5f;

    float _phaseDuration;
    BlockManager blockManager;

    void Awake()
    {
        blockManager = GetComponent<BlockManager>();
        _phaseDuration = defaultPhaseDuration;
    }

    public event Action OnPhaseEnded;

    Coroutine _phaseCoroutine;
    float _endTime;

    public bool IsActive => _phaseCoroutine != null;

    public float RemainingRatio =>
        _phaseCoroutine != null ? Mathf.Clamp01((_endTime - Time.time) / _phaseDuration) : 0f;

    public void SetPhaseDuration(float seconds) => _phaseDuration = Mathf.Max(1f, seconds);

    public void ResetPhaseDuration() => _phaseDuration = defaultPhaseDuration;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StopDrawPhase();
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
        StartCoroutine(FillThenEnd());
    }

    IEnumerator DrawPhaseRoutine()
    {
        _endTime = Time.time + _phaseDuration;

        while (Time.time < _endTime)
        {
            blockManager.DrawBlock();
            yield return new WaitForSeconds(drawInterval);
        }

        _phaseCoroutine = null;
        yield return StartCoroutine(FillHandRoutine());
        EndPhase();
    }

    IEnumerator FillThenEnd()
    {
        yield return StartCoroutine(FillHandRoutine());
        EndPhase();
    }

    IEnumerator FillHandRoutine()
    {
        while (!blockManager.IsHandFull)
        {
            blockManager.DrawBlock();
            yield return new WaitForSeconds(0.3f);
        }
        yield return new WaitForSeconds(0.5f);
    }

    void EndPhase()
    {
        blockManager.DisableDiscard();
        OnPhaseEnded?.Invoke();
    }
}
