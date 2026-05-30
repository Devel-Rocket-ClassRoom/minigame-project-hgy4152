using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    [SerializeField]
    SettingUI settingUI;

    [SerializeField]
    EncyclopediaUI encyclopediaUI;

    [SerializeField]
    LobbyTransitionEffect transitionEffect;

    [SerializeField]
    RectTransform adventureButton;

    [SerializeField]
    RectTransform bossModeButton;

    public void OnAdventureClicked()
    {
        if (transitionEffect != null)
            transitionEffect.Play(adventureButton, GameState.AdventureReady);
        else
            GameStateMachine.Instance.TransitionTo(GameState.AdventureReady);
    }

    public void OnBossModeClicked()
    {
        if (transitionEffect != null)
            transitionEffect.Play(bossModeButton, GameState.BossReady);
        else
            GameStateMachine.Instance.TransitionTo(GameState.BossReady);
    }

    public void OnSettingClicked()
    {
        if (settingUI != null)
            settingUI.Open();
    }

    public void OnEncyclopediaClicked()
    {
        if (encyclopediaUI != null)
            encyclopediaUI.Open();
    }
}
