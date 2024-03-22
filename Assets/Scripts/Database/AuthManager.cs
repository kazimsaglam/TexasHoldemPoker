﻿using System;
using System.Collections;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
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

    public FirebaseAuth auth;
    public FirebaseUser user;

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

    private void Start()
    {
      StartCoroutine(CheckAndFixDependenciesAsync());
    }
    // private void Awake()
    // {
    //   InitializeFirebase();
    //   // Check that all of the necessary dependencies for firebase are present on the system
    //   // FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
    //   // {
    //   //   dependencyStatus = task.Result;
    //   //
    //   //   if (dependencyStatus == DependencyStatus.Available)
    //   //   {
    //   //     
    //   //   }
    //   //   else
    //   //   {
    //   //     Debug.LogError("Could not resolve all firebase dependencies: " + dependencyStatus);
    //   //   }
    //   // });
    // }

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
        UIManager.Instance.OpenLoginPanel();
      }
    }

    private void AutoLogin()
    {
      if (user != null)
      {
        References.userName = user.DisplayName;
        SceneManager.LoadScene("Scenes/GameScene");
      }
      else
      {
        UIManager.Instance.OpenLoginPanel();
      }
    }

    void InitializeFirebase()
    {
      //Set the default instance object
      auth = FirebaseAuth.DefaultInstance;

      auth.StateChanged += AuthStateChanged;
      AuthStateChanged(this, null);
    }

    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
      if (auth.CurrentUser != user)
      {
        bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

        if (!signedIn && user != null)
        {
          Debug.Log("Signed out " + user.UserId);
        }

        user = auth.CurrentUser;

        if (signedIn)
        {
          Debug.Log("Signed in " + user.UserId);
        }
      }
    }

    public void Login()
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

        // References.userName = user.DisplayName;
        // UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
      }
    }

    public void Register()
    {
      StartCoroutine(RegisterAsync(nameRegisterField.text, emailRegisterField.text, passwordRegisterField.text, confirmPasswordRegisterField.text));
    }

    private IEnumerator RegisterAsync(string fullName, string email, string password, string confirmPassword)
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
        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.Exception != null)
        {
          Debug.LogError(registerTask.Exception);

          FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
          System.Diagnostics.Debug.Assert(firebaseException != null, nameof(firebaseException) + " != null");
          AuthError authError = (AuthError)firebaseException.ErrorCode;

          string failedMessage = "Registration Failed! Becuase ";
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
              failedMessage = "Registration Failed";
              break;
          }

          Debug.Log(failedMessage);
        }
        else
        {
          // Get The User After Registration Success
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


            string failedMessage = "Profile update Failed! ";
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
                failedMessage = "Profile update Failed";
                break;
            }

            Debug.Log(failedMessage);
          }
          else
          {
            Debug.Log("Registered Successfully. Welcome " + user.DisplayName);
            UIManager.Instance.OpenLoginPanel();
          }
        }
      }
    }
  }
}