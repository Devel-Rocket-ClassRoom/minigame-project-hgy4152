using UnityEngine;
using UnityEngine.UI;

public class DrawPhaseUI : MonoBehaviour
{
    DrawPhaseTimer drawPhaseTimer;

    [SerializeField]
    Slider timerSlider;

    [SerializeField]
    Button handPlayButton;

    void Awake()
    {
        drawPhaseTimer = GetComponent<DrawPhaseTimer>();
        handPlayButton.onClick.AddListener(drawPhaseTimer.PlayHandNow);
    }

    void Update()
    {
        timerSlider.value = drawPhaseTimer.RemainingRatio;
        handPlayButton.interactable = drawPhaseTimer.IsActive;
    }
}
