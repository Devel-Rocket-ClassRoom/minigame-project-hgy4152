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
    public event Action OnSessionExpired;

    FirebaseAuth _auth;
    bool _firebaseReady;

    public bool IsFirebaseReady { get; private set; }
    public UniTask PendingCloudSync { get; private set; } = UniTask.CompletedTask;
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
            {
                await _auth.CurrentUser.LinkWithCredentialAsync(credential).AsUniTask();
                // Desktop SDK는 LinkWithCredentialAsync 후 익명 토큰을 그대로 유지해서
                // 재시작 시 다시 게스트로 복원됨. 로그아웃 → 이메일 재로그인으로 영속화
                _auth.SignOut();
                await _auth.SignInWithEmailAndPasswordAsync(email, password).AsUniTask();
            }
            else
            {
                await _auth.CreateUserWithEmailAndPasswordAsync(email, password).AsUniTask();
            }
        }
        catch (Exception e)
        {
            await UniTask.SwitchToMainThread();
            FireSignInFailed(e);
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
            await UniTask.SwitchToMainThread();
            FireSignInFailed(e);
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
            await UniTask.SwitchToMainThread();
            FireSignInFailed(e);
        }
    }

    void FireSignInFailed(Exception e)
    {
        var message = ToKoreanMessage(e);
        Debug.LogWarning($"[Auth] 실패: {message}");
        if (e is Firebase.FirebaseException fe && (AuthError)fe.ErrorCode == AuthError.UserTokenExpired)
        {
            _auth?.SignOut();
            OnSessionExpired?.Invoke();
            return;
        }
        OnSignInFailed?.Invoke(message);
    }

    static string ToKoreanMessage(Exception e)
    {
        if (e is not Firebase.FirebaseException fe) return e.Message;
        return (AuthError)fe.ErrorCode switch
        {
            AuthError.EmailAlreadyInUse    => "이미 있는 Id입니다",
            AuthError.InvalidEmail         => "이메일 양식이 틀립니다",
            AuthError.WrongPassword        => "비밀번호가 틀립니다",
            AuthError.UserNotFound         => "존재하지 않는 계정입니다",
            AuthError.WeakPassword         => "비밀번호는 6자 이상이어야 합니다",
            AuthError.NetworkRequestFailed => "네트워크 연결을 확인해 주세요",
            AuthError.UserTokenExpired     => "로그인이 만료됐습니다. 다시 로그인해 주세요",
            _                              => e.Message,
        };
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
            Debug.Log($"[Auth] StateChanged → uid={user.UserId}, email={user.Email ?? "(none)"}, isAnonymous={user.IsAnonymous}");
            PendingCloudSync = SyncCloudAsync(user.UserId).Preserve();
            await PendingCloudSync;
            OnSignInSuccess?.Invoke(user);
        }
        else
        {
            Debug.Log("[Auth] StateChanged → signed out");
            OnSignedOut?.Invoke();
        }
    }

    async UniTask SyncCloudAsync(string userId)
    {
        var cloud = await RealtimeDbCodexService.PullAsync(userId);
        UnlockManager.MergeFromCloud(cloud);
    }

    void OnDestroy()
    {
        if (_auth != null)
            _auth.StateChanged -= HandleAuthStateChanged;
    }
}
