using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class AuthUI : MonoBehaviour
{
    [SerializeField] TMP_InputField emailInput;
    [SerializeField] TMP_InputField passwordInput;
    [SerializeField] TMP_Text errorText;

    void OnEnable()
    {
        if (AuthManager.Instance == null) return;
        AuthManager.Instance.OnSignInFailed += ShowError;
    }

    void OnDisable()
    {
        if (AuthManager.Instance == null) return;
        AuthManager.Instance.OnSignInFailed -= ShowError;
    }

    public void OnLoginClicked()
    {
        ClearError();
        AuthManager.Instance?.SignInWithEmail(emailInput.text, passwordInput.text);
    }

    public void OnSignUpClicked()
    {
        ClearError();
        AuthManager.Instance?.SignUpWithEmail(emailInput.text, passwordInput.text);
    }

    public void OnGuestClicked()
    {
        ClearError();
        AuthManager.Instance?.SignInAnonymously();
    }

    public void OnLogoutClicked()
    {
        ClearError();
        AuthManager.Instance?.SignOut();
    }

    public void OnCancelClicked()
    {
        gameObject.SetActive(false);
    }

    void ShowError(string message)
    {
        if (errorText == null) return;
        errorText.text = message;
        HideErrorAfterDelayAsync().Forget();
    }

    async UniTaskVoid HideErrorAfterDelayAsync()
    {
        await UniTask.Delay(3000, cancellationToken: this.GetCancellationTokenOnDestroy());
        if (errorText != null)
            errorText.text = string.Empty;
    }

    void ClearError()
    {
        if (errorText != null)
            errorText.text = string.Empty;
    }
}
