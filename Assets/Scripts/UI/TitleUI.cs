using UnityEngine;

public class TitleUI : MonoBehaviour
{
    void Update()
    {
        if (Input.anyKeyDown || Input.touchCount > 0)
            GameStateMachine.Instance.TransitionTo(GameState.Lobby);
    }
}
