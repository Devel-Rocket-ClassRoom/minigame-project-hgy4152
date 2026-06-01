using DG.Tweening;
using UnityEngine;

public class LobbyTransitionEffect : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] RectTransform adventureButton;
    [SerializeField] RectTransform bossModeButton;

    [Header("낚싯대 캐릭터")]
    [SerializeField] RectTransform characterRoot;
    [SerializeField] RectTransform fishRodImage;
    [SerializeField] RectTransform hookPoint;
    [SerializeField] RectTransform exclamationMark;

    [Header("레이아웃")]
    [SerializeField] RectTransform buttonOverlay;

    [Header("페이드")]
    [SerializeField] CanvasGroup fadeOverlay;

    [Header("타이밍")]
    [SerializeField] float phase1Duration = 0.6f;
    [SerializeField] float phase2Duration = 1.2f;
    [SerializeField] float phase3Duration = 0.8f;
    [SerializeField] float fadeDuration = 0.5f;

    [Header("연출 파라미터")]
    [SerializeField] float buttonShakeStrength = 14f;
    [SerializeField] float characterShakeAmount = 15f;
    [SerializeField] Vector2 arcPeakOffset = new Vector2(-80f, 250f);
    [SerializeField] Vector2 arcEndOffset = new Vector2(-300f, -200f);
    [SerializeField] float characterFlyTargetX = 0f;
    [SerializeField] float characterArcHeight = 80f;
    [SerializeField] float characterLandingYOffset = 50f;
    [SerializeField] float characterFlyRotation = 60f;

    bool _isPlaying;
    Sequence _sequence;

    public void Play(RectTransform buttonRect, GameState targetState)
    {
        if (_isPlaying) return;
        _isPlaying = true;

        exclamationMark.localScale = Vector3.zero;
        exclamationMark.gameObject.SetActive(true);

        buttonRect.SetParent(buttonOverlay, worldPositionStays: true);

        Vector3 hookWorldPos = hookPoint.position;

        Vector3 charLocalStart = characterRoot.localPosition;
        Vector3 charWorldStart = characterRoot.position;
        int shakeLoops = Mathf.Max(2, Mathf.RoundToInt(phase2Duration / 0.15f) / 2 * 2);

        _sequence = DOTween.Sequence()
            // Phase 1: 버튼 날아가기
            .Append(buttonRect.DOMove(hookWorldPos, phase1Duration).SetEase(Ease.InBack))
            // Phase 2: 낚싯바늘 걸림 (캐릭터+낚싯대 위아래 진동)
            .Append(
                DOTween.Sequence()
                    .Append(buttonRect.DOShakePosition(
                        phase2Duration,
                        new Vector3(buttonShakeStrength, buttonShakeStrength * 0.5f, 0f),
                        vibrato: 20, randomness: 45f, fadeOut: false))
                    .Join(
                        characterRoot.DOLocalMoveY(charLocalStart.y + characterShakeAmount, 0.15f)
                            .SetLoops(shakeLoops, LoopType.Yoyo)
                            .SetEase(Ease.InOutSine))
                    .Insert(0.1f,
                        exclamationMark.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack))
                    .AppendCallback(() =>
                    {
                        characterRoot.localPosition = charLocalStart;
                    })
            )
            // Phase 3: 포물선 날아가기 + 캐릭터 날아가기 + 페이드
            .Append(
                DOTween.Sequence()
                    .Insert(0f, characterRoot.DOMoveX(
                            charWorldStart.x - characterFlyTargetX,
                            phase3Duration)
                        .SetEase(Ease.OutQuad))
                    .Insert(0f, DOTween.To(
                            () => 0f,
                            t => {
                                float arc = characterArcHeight * 4f * t * (1f - t) + characterLandingYOffset * t;
                                var p = characterRoot.position;
                                p.y = charWorldStart.y + arc;
                                characterRoot.position = p;
                            },
                            1f,
                            phase3Duration)
                        .SetEase(Ease.Linear))
                    .Insert(0f, characterRoot.DOLocalRotate(
                            new Vector3(0f, 0f, characterFlyRotation),
                            phase3Duration)
                        .SetEase(Ease.OutQuad))
                    .Insert(0.2f, buttonRect.DOMove(
                            hookWorldPos + new Vector3(arcPeakOffset.x, arcPeakOffset.y, 0f),
                            phase3Duration * 0.4f)
                        .SetEase(Ease.OutQuad))
                    .Insert(0.2f + phase3Duration * 0.4f, buttonRect.DOMove(
                            hookWorldPos + new Vector3(arcEndOffset.x, arcEndOffset.y, 0f),
                            phase3Duration * 0.6f)
                        .SetEase(Ease.InQuad))
                    .Insert(phase3Duration * 0.3f,
                        DOTween.To(() => fadeOverlay.alpha, x => fadeOverlay.alpha = x, 1f, fadeDuration).SetEase(Ease.Linear))
            )
            .AppendInterval(fadeDuration * 0.3f)
            .OnComplete(() =>
            {
                if (this != null)
                    GameStateMachine.Instance.TransitionTo(targetState);
            });
    }

    void OnDestroy()
    {
        _sequence?.Kill();
    }
}
