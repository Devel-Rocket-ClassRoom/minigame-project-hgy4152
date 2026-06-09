using UnityEngine;

public class TitleUI : MonoBehaviour
{
    private void Awake()
    {
        Debug.unityLogger.logEnabled = false;
    }
    void Update()
    {
        if (Input.anyKeyDown || Input.touchCount > 0)
            GameStateMachine.Instance.TransitionTo(GameState.Lobby);
    }
}
