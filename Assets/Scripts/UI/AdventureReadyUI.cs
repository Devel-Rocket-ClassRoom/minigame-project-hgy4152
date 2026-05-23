using UnityEngine;

public class AdventureReadyUI : MonoBehaviour
{
    public void OnPlayClicked() => GameStateMachine.Instance.TransitionTo(GameState.Adventure);

    public void OnBackClicked() => GameStateMachine.Instance.TransitionTo(GameState.Lobby);
}
