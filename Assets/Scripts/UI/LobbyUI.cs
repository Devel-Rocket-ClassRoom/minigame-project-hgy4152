using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    public void OnAdventureClicked() =>
        GameStateMachine.Instance.TransitionTo(GameState.AdventureReady);

    public void OnBossModeClicked() =>
        GameStateMachine.Instance.TransitionTo(GameState.BossReady);
}
