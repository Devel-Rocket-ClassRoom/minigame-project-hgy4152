using System;
using System.Threading;
using Cysharp.Threading.Tasks;
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

    CancellationTokenSource _phaseCts;
    float _endTime;

    public bool IsActive => _phaseCts != null;

    public float RemainingRatio =>
        _phaseCts != null ? Mathf.Clamp01((_endTime - Time.time) / _phaseDuration) : 0f;

    public void SetPhaseDuration(float seconds) => _phaseDuration = Mathf.Max(1f, seconds);

    public void ResetPhaseDuration() => _phaseDuration = defaultPhaseDuration;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StopDrawPhase();
    }

    public void StartDrawPhase()
    {
        var ct = RestartPhase();
        blockManager.ResetDiscardCount();
        DrawPhaseAsync(ct).Forget();
    }

    public void StopDrawPhase()
    {
        if (_phaseCts == null)
            return;
        _phaseCts.Cancel();
        _phaseCts.Dispose();
        _phaseCts = null;
    }

    // 보스 플레이 모드: 블록을 즉시 채운 뒤 타이머만 시작
    public void StartDrawPhaseInstant()
    {
        var ct = RestartPhase();
        blockManager.ResetDiscardCount();
        blockManager.DrawInstanceFull();
        TimerOnlyAsync(ct).Forget();
    }

    CancellationToken RestartPhase()
    {
        StopDrawPhase();
        _phaseCts = CancellationTokenSource.CreateLinkedTokenSource(
            this.GetCancellationTokenOnDestroy()
        );
        return _phaseCts.Token;
    }

    // 타이머 자연 만료 시 핸들만 비움 (취소 아님 — IsActive/RemainingRatio가 비활성으로 전환)
    void ClearPhaseHandle()
    {
        _phaseCts?.Dispose();
        _phaseCts = null;
    }

    async UniTaskVoid TimerOnlyAsync(CancellationToken ct)
    {
        _endTime = Time.time + _phaseDuration;
        await UniTask.Delay(TimeSpan.FromSeconds(_phaseDuration), cancellationToken: ct);
        ClearPhaseHandle();
        EndPhase();
    }

    public void PlayHandNow()
    {
        StopDrawPhase();
        FillThenEndAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    async UniTaskVoid DrawPhaseAsync(CancellationToken ct)
    {
        _endTime = Time.time + _phaseDuration;

        while (Time.time < _endTime)
        {
            blockManager.DrawBlock();
            await UniTask.Delay(TimeSpan.FromSeconds(drawInterval), cancellationToken: ct);
        }

        ClearPhaseHandle();
        await FillHandAsync(this.GetCancellationTokenOnDestroy());
        EndPhase();
    }

    async UniTaskVoid FillThenEndAsync(CancellationToken ct)
    {
        await FillHandAsync(ct);
        EndPhase();
    }

    async UniTask FillHandAsync(CancellationToken ct)
    {
        while (!blockManager.IsHandFull)
        {
            blockManager.DrawBlock();
            await UniTask.Delay(TimeSpan.FromSeconds(0.3f), cancellationToken: ct);
        }
        await UniTask.Delay(TimeSpan.FromSeconds(0.5f), cancellationToken: ct);
    }

    void EndPhase()
    {
        blockManager.DisableDiscard();
        OnPhaseEnded?.Invoke();
    }
}
