using Firebase.Auth;
using UnityEngine;

public class TitleUI : MonoBehaviour
{
    [SerializeField] GameObject loginPanel;
    [SerializeField] AuthUI authUI;

    bool _touched;
    bool _transitioning;
    bool _syncReady;

    void Awake()
    {
        //Debug.unityLogger.logEnabled = false;
        if (loginPanel != null)
            loginPanel.SetActive(false);
    }

    void Start()
    {
        if (AuthManager.Instance == null) return;
        AuthManager.Instance.OnFirebaseReady += HandleFirebaseReady;
        AuthManager.Instance.OnSignInSuccess += HandleSignInSuccess;
    }

    void Update()
    {
        if (_touched) return;
        if (Input.anyKeyDown || Input.touchCount > 0)
        {
            _touched = true;
            OnTouched();
        }
    }

    void OnTouched()
    {
        if (AuthManager.Instance == null)
        {
            TransitionToLobby();
            return;
        }

        // Firebase가 아직 준비 안 됐으면 OnFirebaseReady에서 처리
        if (AuthManager.Instance.IsFirebaseReady)
            HandleFirebaseReady();
    }

    void HandleFirebaseReady()
    {
        if (!_touched) return;

        if (_syncReady)
            TransitionToLobby();
        else if (!AuthManager.Instance.IsSignedIn && loginPanel != null)
            loginPanel.SetActive(true);
        // 로그인은 됐지만 sync 미완료 → HandleSignInSuccess에서 처리
    }

    void HandleSignInSuccess(FirebaseUser user)
    {
        _syncReady = true;
        if (_touched)
            TransitionToLobby();
    }

    void TransitionToLobby()
    {
        if (_transitioning) return;
        _transitioning = true;
        GameStateMachine.Instance.TransitionTo(GameState.Lobby);
    }

    void OnDestroy()
    {
        if (AuthManager.Instance == null) return;
        AuthManager.Instance.OnFirebaseReady -= HandleFirebaseReady;
        AuthManager.Instance.OnSignInSuccess -= HandleSignInSuccess;
    }
}
