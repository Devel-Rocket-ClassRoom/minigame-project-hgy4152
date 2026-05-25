using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    [SerializeField]
    SettingUI settingUI;

    public void OnAdventureClicked() =>
        GameStateMachine.Instance.TransitionTo(GameState.AdventureReady);

    public void OnBossModeClicked() => GameStateMachine.Instance.TransitionTo(GameState.BossReady);

    public void OnSettingClicked()
    {
        if (settingUI != null)
            settingUI.Open();
    }
}
