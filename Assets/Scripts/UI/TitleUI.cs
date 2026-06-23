using Cysharp.Threading.Tasks;
using Firebase.Auth;
using UnityEngine;

public class TitleUI : MonoBehaviour
{
    [SerializeField] GameObject loginPanel;
    [SerializeField] AuthUI authUI;

    bool _touched;
    bool _transitioning;

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

        if (AuthManager.Instance.IsSignedIn)
            TransitionToLobby();
        else if (loginPanel != null)
            loginPanel.SetActive(true);
    }

    void HandleSignInSuccess(FirebaseUser user)
    {
        if (_touched)
            TransitionToLobby();
    }

    void TransitionToLobby()
    {
        if (_transitioning) return;
        _transitioning = true;
        TransitionToLobbyAsync().Forget();
    }

    async UniTaskVoid TransitionToLobbyAsync()
    {
        if (AuthManager.Instance != null)
            await AuthManager.Instance.PendingCloudSync;
        GameStateMachine.Instance.TransitionTo(GameState.Lobby);
    }

    void OnDestroy()
    {
        if (AuthManager.Instance == null) return;
        AuthManager.Instance.OnFirebaseReady -= HandleFirebaseReady;
        AuthManager.Instance.OnSignInSuccess -= HandleSignInSuccess;
    }
}
