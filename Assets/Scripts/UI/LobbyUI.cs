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

    [SerializeField]
    GameObject inputBlocker;

    void Start()
    {
        if (AuthManager.Instance != null)
            AuthManager.Instance.OnSessionExpired += OnSessionExpired;

        var user = AuthManager.Instance?.CurrentUser;
        if (user == null)
            Debug.Log("[Lobby] CurrentUser: null");
        else
            Debug.Log($"[Lobby] CurrentUser: uid={user.UserId}, email={user.Email ?? "(none)"}, isAnonymous={user.IsAnonymous}");
    }

    void OnDestroy()
    {
        if (AuthManager.Instance != null)
            AuthManager.Instance.OnSessionExpired -= OnSessionExpired;
    }

    void OnSessionExpired()
    {
        GameStateMachine.Instance.TransitionTo(GameState.Title);
    }

    public void OnAdventureClicked()
    {
        inputBlocker?.SetActive(true);
        if (transitionEffect != null)
            transitionEffect.Play(adventureButton, GameState.AdventureReady);
        else
            GameStateMachine.Instance.TransitionTo(GameState.AdventureReady);
    }

    public void OnBossModeClicked()
    {
        inputBlocker?.SetActive(true);
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

    public void OnLogoutClicked()
    {
        AuthManager.Instance?.SignOut();
        GameStateMachine.Instance.TransitionTo(GameState.Title);
    }
}
