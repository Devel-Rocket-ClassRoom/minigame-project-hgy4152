using System;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance { get; private set; }
    public FirebaseUser CurrentUser { get; private set; }
    public bool IsSignedIn => CurrentUser != null;

    public event Action<FirebaseUser> OnSignInSuccess;
    public event Action<string> OnSignInFailed;
    public event Action OnSignedOut;

    FirebaseAuth _auth;
    bool _firebaseReady;

    public bool IsFirebaseReady { get; private set; }
    public event Action OnFirebaseReady;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitFirebaseAsync().Forget();
    }

    async UniTaskVoid InitFirebaseAsync()
    {
        var status = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask();
        if (status != DependencyStatus.Available)
        {
            Debug.LogWarning($"[Auth] Firebase 초기화 실패: {status}");
            IsFirebaseReady = true;
            OnFirebaseReady?.Invoke();
            return;
        }
        _auth = FirebaseAuth.DefaultInstance;
        _auth.StateChanged += HandleAuthStateChanged;
        _firebaseReady = true;
        IsFirebaseReady = true;
        OnFirebaseReady?.Invoke();
    }

    public void SignInWithEmail(string email, string password)
    {
        if (!_firebaseReady) return;
        SignInWithEmailAsync(email, password).Forget();
    }

    public void SignUpWithEmail(string email, string password)
    {
        if (!_firebaseReady) return;
        SignUpWithEmailAsync(email, password).Forget();
    }

    public void SignInAnonymously()
    {
        if (!_firebaseReady) return;
        SignInAnonymouslyAsync().Forget();
    }

    public void SignOut()
    {
        _auth?.SignOut();
    }

    async UniTaskVoid SignUpWithEmailAsync(string email, string password)
    {
        try
        {
            var credential = EmailAuthProvider.GetCredential(email, password);
            if (_auth.CurrentUser != null && _auth.CurrentUser.IsAnonymous)
                await _auth.CurrentUser.LinkWithCredentialAsync(credential).AsUniTask();
            else
                await _auth.CreateUserWithEmailAndPasswordAsync(email, password).AsUniTask();
        }
        catch (Exception e)
        {
            await UniTask.SwitchToMainThread();
            var message = e is Firebase.FirebaseException fe
                && (AuthError)fe.ErrorCode == AuthError.EmailAlreadyInUse
                ? "이미 있는 Id입니다"
                : e.Message;
            Debug.LogWarning($"[Auth] 회원가입 실패: {message}");
            OnSignInFailed?.Invoke(message);
        }
    }

    async UniTaskVoid SignInWithEmailAsync(string email, string password)
    {
        try
        {
            await _auth.SignInWithEmailAndPasswordAsync(email, password).AsUniTask();
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Auth] 이메일 로그인 실패: {e.Message}");
            await UniTask.SwitchToMainThread();
            OnSignInFailed?.Invoke(e.Message);
        }
    }

    async UniTaskVoid SignInAnonymouslyAsync()
    {
        try
        {
            await _auth.SignInAnonymouslyAsync().AsUniTask();
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Auth] 익명 로그인 실패: {e.Message}");
            await UniTask.SwitchToMainThread();
            OnSignInFailed?.Invoke(e.Message);
        }
    }

    void HandleAuthStateChanged(object sender, EventArgs e)
    {
        var user = _auth.CurrentUser;
        HandleAuthStateChangedAsync(user).Forget();
    }

    async UniTaskVoid HandleAuthStateChangedAsync(FirebaseUser user)
    {
        await UniTask.SwitchToMainThread();
        CurrentUser = user;
        if (user != null)
        {
            OnSignInSuccess?.Invoke(user);
            var cloud = await RealtimeDbCodexService.PullAsync(user.UserId);
            UnlockManager.MergeFromCloud(cloud);
        }
        else
        {
            OnSignedOut?.Invoke();
        }
    }

    void OnDestroy()
    {
        if (_auth != null)
            _auth.StateChanged -= HandleAuthStateChanged;
    }
}
