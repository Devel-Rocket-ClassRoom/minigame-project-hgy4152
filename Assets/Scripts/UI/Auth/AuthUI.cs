using Firebase.Auth;
using TMPro;
using UnityEngine;

public class AuthUI : MonoBehaviour
{
    [SerializeField] TMP_InputField emailInput;
    [SerializeField] TMP_InputField passwordInput;
    [SerializeField] GameObject stateTextBox;
    [SerializeField] TMP_Text stateText;

    enum PendingAction { None, Login, SignUp, Guest }
    PendingAction _pending;

    void OnEnable()
    {
        if (AuthManager.Instance == null) return;
        AuthManager.Instance.OnSignInSuccess += HandleSignInSuccess;
        AuthManager.Instance.OnSignInFailed += HandleSignInFailed;
    }

    void OnDisable()
    {
        if (AuthManager.Instance == null) return;
        AuthManager.Instance.OnSignInSuccess -= HandleSignInSuccess;
        AuthManager.Instance.OnSignInFailed -= HandleSignInFailed;
    }

    public void OnLoginClicked()
    {
        _pending = PendingAction.Login;
        AuthManager.Instance?.SignInWithEmail(emailInput.text, passwordInput.text);
    }

    public void OnSignUpClicked()
    {
        _pending = PendingAction.SignUp;
        AuthManager.Instance?.SignUpWithEmail(emailInput.text, passwordInput.text);
    }

    public void OnGuestClicked()
    {
        _pending = PendingAction.Guest;
        AuthManager.Instance?.SignInAnonymously();
    }

    public void OnLogoutClicked()
    {
        AuthManager.Instance?.SignOut();
    }

    public void OnCancelClicked()
    {
        gameObject.SetActive(false);
    }

    public void OnBackDropClicked()
    {
        if (stateTextBox != null)
            stateTextBox.SetActive(false);
    }

    public void SetText(string message)
    {
        if (stateTextBox == null) return;
        if (stateText != null) stateText.text = message;
        stateTextBox.SetActive(true);
    }

    void HandleSignInSuccess(FirebaseUser user)
    {
        string message = _pending switch
        {
            PendingAction.Login  => "로그인 성공",
            PendingAction.SignUp => AuthManager.Instance.CurrentUser?.IsAnonymous == false
                && user.Email != null ? "계정 연결 성공" : "회원가입 성공",
            PendingAction.Guest  => "게스트로 시작합니다",
            _                    => "로그인 성공",
        };
        _pending = PendingAction.None;
        SetText(message);
    }

    void HandleSignInFailed(string message)
    {
        _pending = PendingAction.None;
        SetText(message);
    }
}
