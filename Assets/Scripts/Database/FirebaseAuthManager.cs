using System;
using System.Collections;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Database
{
  public class FirebaseAuthManager : MonoBehaviour
  {
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;

    private FirebaseAuth _auth;
    private FirebaseUser _user;

    private DatabaseReference _databaseReference;

    [Space]
    [Header("Login")]
    public TMP_InputField emailLoginField;

    public TMP_InputField passwordLoginField;

    [Space]
    [Header("Registration")]
    public TMP_InputField nameRegisterField;

    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField confirmPasswordRegisterField;
    public int defaultMoney;
    public int win;
    public int totalGameCount;
    public static FirebaseAuthManager Instance { get; private set; }

    private void Start()
    {
      StartCoroutine(CheckAndFixDependenciesAsync());
    }

    private void Awake()
    {
      if (Instance == null)
      {
        Instance = this;
      }
      else
      {
        Destroy(gameObject);
      }
    }

    private IEnumerator CheckAndFixDependenciesAsync()
    {
      Task<DependencyStatus> dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();
      yield return new WaitUntil(() => dependencyTask.IsCompleted);
      dependencyStatus = dependencyTask.Result;
      if (dependencyStatus == DependencyStatus.Available)
      {
        InitializeFirebase();
        yield return new WaitForEndOfFrame();
        StartCoroutine(CheckForAutoLogin());
      }
      else
      {
        Debug.LogError("Could not resolve all firebase dependencies: " + dependencyStatus);
      }
    }

    private IEnumerator CheckForAutoLogin()
    {
      if (_user != null)
      {
        Task reloadUserTask = _user.ReloadAsync();
        yield return new WaitUntil(() => reloadUserTask.IsCompleted);
        AutoLogin();
      }
      else
      {
        WelcomeUIManager.instance.OpenLoginPanel();
      }
    }

    private void AutoLogin()
    {
      if (_user != null)
      {
        SceneManager.LoadScene("Scenes/MainMenuScene");
      }
      else
      {
        WelcomeUIManager.instance.OpenLoginPanel();
      }
    }

    private void InitializeFirebase()
    {
      _auth = FirebaseAuth.DefaultInstance;
      _databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

      _auth.StateChanged += AuthStateChanged;
      AuthStateChanged(this, null);
    }

    private void AuthStateChanged(object sender, EventArgs eventArgs)
    {
      if (_auth.CurrentUser != _user)
      {
        bool signedIn = _user != _auth.CurrentUser && _auth.CurrentUser != null;

        if (!signedIn && _user != null)
        {
          Debug.Log("Signed out " + _user.UserId);
          SceneManager.LoadScene("Scenes/WelcomeScene");
        }

        _user = _auth.CurrentUser;

        if (signedIn)
        {
          Debug.Log("Signed in " + _user.UserId);
        }
      }
    }

    public void OnLogOut()
    {
      if (_auth != null && _user != null)
      {
        _auth.SignOut();
      }
    }

    public void OnLogIn()
    {
      StartCoroutine(LoginAsync(emailLoginField.text, passwordLoginField.text));
    }

    private IEnumerator LoginAsync(string email, string password)
    {
      Task<AuthResult> loginTask = _auth.SignInWithEmailAndPasswordAsync(email, password);

      yield return new WaitUntil(() => loginTask.IsCompleted);

      if (loginTask.Exception != null)
      {
        Debug.LogError(loginTask.Exception);

        FirebaseException firebaseException = loginTask.Exception.GetBaseException()
          as FirebaseException;
        System.Diagnostics.Debug.Assert(firebaseException != null,
          nameof(firebaseException) + " != null");
        AuthError authError = (AuthError)firebaseException.ErrorCode;


        string failedMessage = "Login Failed! Because ";

        switch (authError)
        {
          case AuthError.InvalidEmail:
            failedMessage += "Email is invalid";
            break;
          case AuthError.WrongPassword:
            failedMessage += "Wrong Password";
            break;
          case AuthError.MissingEmail:
            failedMessage += "Email is missing";
            break;
          case AuthError.MissingPassword:
            failedMessage += "Password is missing";
            break;
          default:
            failedMessage = "Login Failed";
            break;
        }

        Debug.Log(failedMessage);
      }
      else
      {
        _user = loginTask.Result.User;

        Debug.LogFormat("{0} You Are Successfully Logged In", _user.DisplayName);
        SceneManager.LoadScene("Scenes/MainMenuScene");
      }
    }

    public void Register()
    {
      StartCoroutine(RegisterAsync(nameRegisterField.text, emailRegisterField.text,
        passwordRegisterField.text, confirmPasswordRegisterField.text));
    }

    private void CheckState(AuthError authError)
    {
      string failedMessage = " Process is Failed! Because ";
      switch (authError)
      {
        case AuthError.InvalidEmail:
          failedMessage += "email is invalid";
          break;
        case AuthError.WrongPassword:
          failedMessage += "wrong Password";
          break;
        case AuthError.MissingEmail:
          failedMessage += "email is missing";
          break;
        case AuthError.MissingPassword:
          failedMessage += "password is missing";
          break;
        default:
          failedMessage = "registration is failed";
          break;
      }

      Debug.Log(failedMessage);
    }

    private IEnumerator RegisterAsync(string fullName, string email, string password,
      string confirmationPassword)
    {
      if (fullName == "")
      {
        Debug.LogError("User Name is empty");
      }
      else if (email == "")
      {
        Debug.LogError("Email field is empty");
      }
      else if (password != confirmationPassword)
      {
        Debug.LogError("Password does not match");
      }
      else
      {
        Task<AuthResult> registerTask = _auth.CreateUserWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.Exception != null)
        {
          Debug.LogError(registerTask.Exception);

          FirebaseException firebaseException = registerTask.Exception.GetBaseException()
            as FirebaseException;
          System.Diagnostics.Debug.Assert(firebaseException != null,
            nameof(firebaseException) + " != null");
          AuthError authError = (AuthError)firebaseException.ErrorCode;
          CheckState(authError);
        }
        else
        {
          _user = registerTask.Result.User;

          UserProfile userProfile = new UserProfile {DisplayName = fullName};

          Task updateProfileTask = _user.UpdateUserProfileAsync(userProfile);

          yield return new WaitUntil(() => updateProfileTask.IsCompleted);

          if (updateProfileTask.Exception != null)
          {
            _user.DeleteAsync();

            Debug.LogError(updateProfileTask.Exception);

            FirebaseException firebaseException = updateProfileTask.Exception.GetBaseException()
              as FirebaseException;
            System.Diagnostics.Debug.Assert(firebaseException != null,
              nameof(firebaseException) + " != null");
            AuthError authError = (AuthError)firebaseException.ErrorCode;
            CheckState(authError);
          }
          else
          {
            string userId = _user.UserId;
            WriteLeaderboardData(userId, fullName, defaultMoney);
            WriteUserData(userId, fullName, defaultMoney, win, totalGameCount);
            Debug.Log("Registered Successfully. Welcome " + _user.DisplayName);
            WelcomeUIManager.instance.OpenLoginPanel();
          }
        }
      }
    }

    private void WriteUserData(string userId, string fullName, int money, int wins, int totalCount)
    {
      UserData userData = new UserData()
      {
        playerName = fullName,
        money = money,
        id = userId,
        win = wins,
        totalGameCount = totalCount,
        currentExp = 0,
        currentLevel = 0
      };

      string json = JsonUtility.ToJson(userData);
      _databaseReference.Child("userData").Child(userId).SetRawJsonValueAsync(json);
    }

    private void WriteLeaderboardData(string userId, string fullName, int money)
    {
      LeaderboardData leaderboardData = new LeaderboardData
      {
        playerName = fullName,
        money = money,
        id = userId
      };

      string json = JsonUtility.ToJson(leaderboardData);
      _databaseReference.Child("leaderboard").Child(userId).SetRawJsonValueAsync(json);
    }

    public async Task<string> GetMoney()
    {
      if (_auth.CurrentUser != null)
      {
        string userId = _auth.CurrentUser.UserId;

        DataSnapshot snapshot = await _databaseReference.Child("userData")
          .Child(userId).Child("money").GetValueAsync();

        if (snapshot.Exists)
        {
          return snapshot.Value.ToString();
        }
        else
        {
          return "Data not found for the user";
        }
      }
      else
      {
        return "User not authenticated";
      }
    }

    public async Task<string> GetFullName()
    {
      if (_auth.CurrentUser != null)
      {
        string userId = _auth.CurrentUser.UserId;

        DataSnapshot snapshot = await _databaseReference.Child("userData")
          .Child(userId).Child("playerName").GetValueAsync();

        if (snapshot.Exists)
        {
          return snapshot.Value.ToString();
        }
        else
        {
          return "Full name not found for the user";
        }
      }
      else
      {
        return "User not authenticated";
      }
    }

    public async Task<string> GetExperience()
    {
      if (_auth.CurrentUser != null)
      {
        string userId = _auth.CurrentUser.UserId;

        DataSnapshot snapshot = await _databaseReference.Child("userData")
          .Child(userId).Child("currentExp").GetValueAsync();

        if (snapshot.Exists)
        {
          return snapshot.Value.ToString();
        }
        else
        {
          return "Data not found for the user";
        }
      }
      else
      {
        return "User not authenticated";
      }
    }

    public void UpdateExperience(int newExperience)
    {
      if (_databaseReference != null)
      {
        Task setValueTask = _databaseReference.Child("userData").Child(_auth.CurrentUser.UserId)
          .Child("currentExp").SetValueAsync(newExperience);

        Task.WhenAll(setValueTask).ContinueWithOnMainThread(task =>
        {
          if (task.IsFaulted || task.IsCanceled)
          {
            Debug.LogError("Failed to update experience");
          }
          else
          {
            Debug.Log("Experience updated successfully");
          }
        });
      }
    }

    public void UpdateLevel(int newLevel)
    {
      if (_databaseReference != null)
      {
        Task setValueTask = _databaseReference.Child("userData").Child(_auth.CurrentUser.UserId)
          .Child("currentLevel").SetValueAsync(newLevel);

        Task.WhenAll(setValueTask).ContinueWithOnMainThread(task =>
        {
          if (task.IsFaulted || task.IsCanceled)
          {
            Debug.LogError("Failed to update level");
          }
          else
          {
            Debug.Log("Level updated successfully");
          }
        });
      }
    }

    public async Task<string> GetLevel()
    {
      if (_auth.CurrentUser != null)
      {
        string userId = _auth.CurrentUser.UserId;

        DataSnapshot snapshot = await _databaseReference.Child("userData")
          .Child(userId).Child("currentLevel").GetValueAsync();

        if (snapshot.Exists)
        {
          return snapshot.Value.ToString();
        }
        else
        {
          return "Data not found for the user";
        }
      }
      else
      {
        return "User not authenticated";
      }
    }

    public async Task<string> GetWinOrTotalGameCount(string state)
    {
      if (_auth.CurrentUser != null)
      {
        string userId = _auth.CurrentUser.UserId;

        DataSnapshot snapshot = await _databaseReference.Child("userData")
          .Child(userId).Child(state).GetValueAsync();

        if (snapshot.Exists)
        {
          return snapshot.Value.ToString();
        }
        else
        {
          return "Data not found for the user";
        }
      }
      else
      {
        return "User not authenticated";
      }
    }

    public void UpdateMoney(int newMoney)
    {
      if (_databaseReference != null)
      {
        Task setValueTask = _databaseReference.Child("userData").Child(_auth.CurrentUser.UserId)
          .Child("money").SetValueAsync(newMoney);

        Task updateLeaderboardTask = _databaseReference.Child("leaderboard")
          .Child(_auth.CurrentUser.UserId).Child("money").SetValueAsync(newMoney);

        Task.WhenAll(setValueTask, updateLeaderboardTask).ContinueWithOnMainThread(task =>
        {
          if (task.IsFaulted || task.IsCanceled)
          {
            Debug.LogError("Failed to update money");
          }
          else
          {
            Debug.Log("Money updated successfully");
          }
        });
      }
    }

    public void UpdateWinCount(int count)
    {
      if (_databaseReference != null)
      {
        Task setWinCountTask = _databaseReference.Child("userData").Child(_auth.CurrentUser.UserId)
          .Child("win").SetValueAsync(count);

        Task.WhenAll(setWinCountTask).ContinueWithOnMainThread(task =>
        {
          if (task.IsFaulted || task.IsCanceled)
          {
            Debug.LogError("Failed to update game and win count");
          }
          else
          {
            Debug.Log("Updated successfully");
          }
        });
      }
    }

    public void UpdateGameCount(int count)
    {
      if (_databaseReference != null)
      {
        Task setGameCountTask = _databaseReference.Child("userData").Child(_auth.CurrentUser.UserId)
          .Child("totalGameCount").SetValueAsync(count);


        Task.WhenAll(setGameCountTask).ContinueWithOnMainThread(task =>
        {
          if (task.IsFaulted || task.IsCanceled)
          {
            Debug.LogError("Failed to update game and win count");
          }
          else
          {
            Debug.Log("Updated successfully");
          }
        });
      }
    }
  }

  [Serializable]
  public class UserData
  {
    public string playerName;
    public int money;
    public string id;
    public int win;
    public int totalGameCount;
    public int currentExp;
    public int currentLevel;
  }

  [Serializable]
  public class LeaderboardData
  {
    public string playerName;
    public int money;
    public string id;
  }
}