using TMPro;
using UnityEngine;

public class TurnCounterUI : MonoBehaviour
{
    [SerializeField]
    TMP_Text label;

    [SerializeField]
    BossPatternSystem bossPatternSystem;

    [SerializeField]
    int maxTurns = 5;

    void OnEnable()
    {
        bossPatternSystem.OnInjected += Refresh;
    }

    void OnDisable()
    {
        bossPatternSystem.OnInjected -= Refresh;
    }

    void Refresh()
    {
        if (label != null)
            label.text = $"TURN {bossPatternSystem.TurnIndex} / {maxTurns}";
    }
}
