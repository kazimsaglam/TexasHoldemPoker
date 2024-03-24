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
    // Firebase variable
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;

    public static FirebaseAuth auth;
    public static FirebaseUser user;

    public DatabaseReference databaseReference;

    // Login Variables
    [Space]
    [Header("Login")]
    public TMP_InputField emailLoginField;

    public TMP_InputField passwordLoginField;

    // Registration Variables
    [Space]
    [Header("Registration")]
    public TMP_InputField nameRegisterField;

    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField confirmPasswordRegisterField;

    //User Data variables
    [Header("UserData")]
    public TMP_InputField xpField;

    public TMP_InputField killsField;

    public delegate void MoneyUpdateHandler(int newMoney);

    public static event MoneyUpdateHandler OnMoneyUpdate;
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
      if (user != null)
      {
        Task reloadUserTask = user.ReloadAsync();
        yield return new WaitUntil(() => reloadUserTask.IsCompleted);
        AutoLogin();
      }
      else
      {
        UIManager.instance.OpenLoginPanel();
      }
    }

    private void AutoLogin()
    {
      if (user != null)
      {
        References.userName = user.DisplayName;
        UIManager.instance.OpenUpdatePanel();
        // SceneManager.LoadScene("Scenes/GameScene");
      }
      else
      {
        UIManager.instance.OpenLoginPanel();
      }
    }

    void InitializeFirebase()
    {
      auth = FirebaseAuth.DefaultInstance;
      databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

      auth.StateChanged += AuthStateChanged;
      AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
      if (auth.CurrentUser != user)
      {
        bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

        if (!signedIn && user != null)
        {
          Debug.Log("Signed out " + user.UserId);
          SceneManager.LoadScene("Scenes/SampleScene");
        }

        user = auth.CurrentUser;

        if (signedIn)
        {
          Debug.Log("Signed in " + user.UserId);
        }
      }
    }

    public static void OnLogOut()
    {
      if (auth != null && user != null)
      {
        auth.SignOut();
      }
    }

    public void OnLogIn()
    {
      StartCoroutine(LoginAsync(emailLoginField.text, passwordLoginField.text));
    }

    private IEnumerator LoginAsync(string email, string password)
    {
      Task<AuthResult> loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

      yield return new WaitUntil(() => loginTask.IsCompleted);

      if (loginTask.Exception != null)
      {
        Debug.LogError(loginTask.Exception);

        FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
        System.Diagnostics.Debug.Assert(firebaseException != null, nameof(firebaseException) + " != null");
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
        user = loginTask.Result.User;

        Debug.LogFormat("{0} You Are Successfully Logged In", user.DisplayName);
        SceneManager.LoadScene("Scenes/GameScene");
      }
    }

    public void Register()
    {
      StartCoroutine(RegisterAsync(nameRegisterField.text, emailRegisterField.text,
        passwordRegisterField.text, confirmPasswordRegisterField.text));
    }

    private IEnumerator RegisterAsync(string fullName, string email, string password,
      string confirmPassword)
    {
      if (fullName == "")
      {
        Debug.LogError("User Name is empty");
      }
      else if (email == "")
      {
        Debug.LogError("Email field is empty");
      }
      else if (passwordRegisterField.text != confirmPasswordRegisterField.text)
      {
        Debug.LogError("Password does not match");
      }
      else
      {
        Task<AuthResult> registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.Exception != null)
        {
          Debug.LogError(registerTask.Exception);

          FirebaseException firebaseException = registerTask.Exception.GetBaseException()
            as FirebaseException;
          System.Diagnostics.Debug.Assert(firebaseException != null,
            nameof(firebaseException) + " != null");
          AuthError authError = (AuthError)firebaseException.ErrorCode;

          string failedMessage = "Registration Failed! Because ";
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
        else
        {
          user = registerTask.Result.User;

          UserProfile userProfile = new UserProfile {DisplayName = fullName};

          Task updateProfileTask = user.UpdateUserProfileAsync(userProfile);

          yield return new WaitUntil(() => updateProfileTask.IsCompleted);

          if (updateProfileTask.Exception != null)
          {
            user.DeleteAsync();

            Debug.LogError(updateProfileTask.Exception);

            FirebaseException firebaseException = updateProfileTask.Exception.GetBaseException()
              as FirebaseException;
            System.Diagnostics.Debug.Assert(firebaseException != null,
              nameof(firebaseException) + " != null");
            AuthError authError = (AuthError)firebaseException.ErrorCode;


            string failMessage = "Profile update Failed! Because ";
            switch (authError)
            {
              case AuthError.InvalidEmail:
                failMessage += "email is invalid";
                break;
              case AuthError.WrongPassword:
                failMessage += "wrong Password";
                break;
              case AuthError.MissingEmail:
                failMessage += "email is missing";
                break;
              case AuthError.MissingPassword:
                failMessage += "password is missing";
                break;
              default:
                failMessage = "profile update Failed";
                break;
            }

            Debug.Log(failMessage);
          }
          else
          {
            Debug.Log("Registered Successfully. Welcome " + user.DisplayName);
            UIManager.instance.OpenLoginPanel();
          }
        }
      }
    }

    public void SaveDataButton()
    {
      StartCoroutine(UpdateXp(int.Parse(xpField.text)));
      StartCoroutine(UpdateKills(int.Parse(killsField.text)));
      SceneManager.LoadScene("Scenes/GameScene");
      // StartCoroutine(UpdateMoney());
    }

    private IEnumerator UpdateXp(int _xp)
    {
      //Set the currently logged in user xp
      Task DBTask = databaseReference.Child("userData").Child(user.UserId).Child("xp").SetValueAsync(_xp);

      yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

      if (DBTask.Exception != null)
      {
        Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
      }
      else
      {
        //Xp is now updated
      }
    }

    public IEnumerator UpdateKills(int kills)
    {
      //Set the currently logged in user kills
      Task databaseTask = databaseReference.Child("userData").Child(user.UserId)
        .Child("kills").SetValueAsync(kills);

      yield return new WaitUntil(predicate: () => databaseTask.IsCompleted);

      if (databaseTask.Exception != null)
      {
        Debug.LogWarning(message: $"Failed to register task with {databaseTask.Exception}");
      }
      else
      {
        Debug.Log("Kills are updated");
        //Kills are now updated
      }
    }
    public void LoadDataButton()
    {
      // StartCoroutine(UpdateMoney());
       StartCoroutine(LoadUserData());
      
    }
    private IEnumerator LoadUserData()
    {
      Task<DataSnapshot> databaseTask = databaseReference.Child("userData")
        .Child(user.UserId).GetValueAsync();

      yield return new WaitUntil(predicate: () => databaseTask.IsCompleted);

      if (databaseTask.Exception != null)
      {
        Debug.LogWarning(message: $"Failed to register task with {databaseTask.Exception}");
      }
      else if (databaseTask.Result.Value == null)
      {
        //No data exists yet
        xpField.text = "0";
        killsField.text = "0";
      }
      else
      {
        //Data has been retrieved
        DataSnapshot snapshot = databaseTask.Result;

        xpField.text = snapshot.Child("xp").Value.ToString();
        killsField.text = snapshot.Child("kills").Value.ToString();
      }
    }


    public void UpdateMoney(int newMoney)
    {
      if (databaseReference != null)
      {
        Task setValueTask = databaseReference.Child("userData").Child(auth.CurrentUser.UserId)
          .Child("money").SetValueAsync(newMoney);

        setValueTask.ContinueWithOnMainThread(task =>
        {
          if (task.IsFaulted || task.IsCanceled)
          {
            Debug.LogError("Failed to update money in database");
          }
          else
          {
            Debug.Log("Money updated successfully");

            OnMoneyUpdate?.Invoke(newMoney);
          }
        });
      }
    }
  }
}