using UnityEngine;

public class LobbyUI : MonoBehaviour
{
    public void OnAdventureClicked() =>
        GameStateMachine.Instance.TransitionTo(GameState.AdventureReady);

    public void OnBossModeClicked()
    {
        Debug.Log("보스 모드 준비 중");
    }
}
