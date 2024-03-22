using System.Collections;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine;

namespace Database
{
  public class EmailLogin : MonoBehaviour
  {
    #region Variables

    [Header("Log In")]
    public TMP_InputField loginEmail;

    public TMP_InputField loginPassword;

    [Header("Sign Up")]
    public TMP_InputField signUpEmail;

    public TMP_InputField signUpPassword;
    public TMP_InputField signUpPasswordConfirmation;

    [Header("Extra")]
    public GameObject loadingScreen;

    public TextMeshProUGUI logText;
    public GameObject loginUi, signUpUi, successUi;

    #endregion

    public void SignUp()
    {
      loadingScreen.SetActive(true);
      FirebaseAuth auth = FirebaseAuth.DefaultInstance;
      string email = signUpEmail.text;
      string password = signUpPassword.text;
      string passwordConfirmation = signUpPasswordConfirmation.text;

      // Check if any of the required fields are empty
      if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordConfirmation))
      {
        ShowLogMessage("Please fill in all fields!");
        loadingScreen.SetActive(false);
        return;
      }

      if (password != passwordConfirmation)
      {
        ShowLogMessage("Password and confirmation do not match!");
        loadingScreen.SetActive(false);
        return;
      }

      // Proceed with user registration
      auth.CreateUserWithEmailAndPasswordAsync(email, password)
        .ContinueWithOnMainThread(task =>
        {
          if (task.IsCanceled)
          {
            Debug.LogError("Operation is canceled.");
            return;
          }

          if (task.IsFaulted)
          {
            Debug.LogError("Operation is encountered with an error: " + task.Exception);
            return;
          }

          loadingScreen.SetActive(false);
          AuthResult result = task.Result;
          Debug.LogFormat("User created successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);
          signUpEmail.text = "";
          signUpPassword.text = "";
          signUpPasswordConfirmation.text = "";
          ShowLogMessage("Sign up Successful");
        });
    }

    public void SendEmailVerification()
    {
      StartCoroutine(SendEmailForVerificationAsync());
    }

    IEnumerator SendEmailForVerificationAsync()
    {
      FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
      if (user != null)
      {
        Task sendEmailTask = user.SendEmailVerificationAsync();
        yield return new WaitUntil(() => sendEmailTask.IsCompleted);

        if (sendEmailTask.Exception != null)
        {
          print("Email send error");
          FirebaseException firebaseException = sendEmailTask.Exception.GetBaseException() as FirebaseException;
          AuthError error = (AuthError)firebaseException.ErrorCode;

          switch (error)
          {
            case AuthError.None:
              break;
            case AuthError.Unimplemented:
              break;
            case AuthError.Failure:
              break;
            case AuthError.InvalidCustomToken:
              break;
            case AuthError.CustomTokenMismatch:
              break;
            case AuthError.InvalidCredential:
              break;
            case AuthError.UserDisabled:
              break;
            case AuthError.AccountExistsWithDifferentCredentials:
              break;
            case AuthError.OperationNotAllowed:
              break;
            case AuthError.EmailAlreadyInUse:
              break;
            case AuthError.RequiresRecentLogin:
              break;
            case AuthError.CredentialAlreadyInUse:
              break;
            case AuthError.InvalidEmail:
              break;
            case AuthError.WrongPassword:
              break;
            case AuthError.TooManyRequests:
              break;
            case AuthError.UserNotFound:
              break;
            case AuthError.ProviderAlreadyLinked:
              break;
            case AuthError.NoSuchProvider:
              break;
            case AuthError.InvalidUserToken:
              break;
            case AuthError.UserTokenExpired:
              break;
            case AuthError.NetworkRequestFailed:
              break;
            case AuthError.InvalidApiKey:
              break;
            case AuthError.AppNotAuthorized:
              break;
            case AuthError.UserMismatch:
              break;
            case AuthError.WeakPassword:
              break;
            case AuthError.NoSignedInUser:
              break;
            case AuthError.ApiNotAvailable:
              break;
            case AuthError.ExpiredActionCode:
              break;
            case AuthError.InvalidActionCode:
              break;
            case AuthError.InvalidMessagePayload:
              break;
            case AuthError.InvalidPhoneNumber:
              break;
            case AuthError.MissingPhoneNumber:
              break;
            case AuthError.InvalidRecipientEmail:
              break;
            case AuthError.InvalidSender:
              break;
            case AuthError.InvalidVerificationCode:
              break;
            case AuthError.InvalidVerificationId:
              break;
            case AuthError.MissingVerificationCode:
              break;
            case AuthError.MissingVerificationId:
              break;
            case AuthError.MissingEmail:
              break;
            case AuthError.MissingPassword:
              break;
            case AuthError.QuotaExceeded:
              break;
            case AuthError.RetryPhoneAuth:
              break;
            case AuthError.SessionExpired:
              break;
            case AuthError.AppNotVerified:
              break;
            case AuthError.AppVerificationFailed:
              break;
            case AuthError.CaptchaCheckFailed:
              break;
            case AuthError.InvalidAppCredential:
              break;
            case AuthError.MissingAppCredential:
              break;
            case AuthError.InvalidClientId:
              break;
            case AuthError.InvalidContinueUri:
              break;
            case AuthError.MissingContinueUri:
              break;
            case AuthError.KeychainError:
              break;
            case AuthError.MissingAppToken:
              break;
            case AuthError.MissingIosBundleId:
              break;
            case AuthError.NotificationNotForwarded:
              break;
            case AuthError.UnauthorizedDomain:
              break;
            case AuthError.WebContextAlreadyPresented:
              break;
            case AuthError.WebContextCancelled:
              break;
            case AuthError.DynamicLinkNotActivated:
              break;
            case AuthError.Cancelled:
              break;
            case AuthError.InvalidProviderId:
              break;
            case AuthError.WebInternalError:
              break;
            case AuthError.WebStorateUnsupported:
              break;
            case AuthError.TenantIdMismatch:
              break;
            case AuthError.UnsupportedTenantOperation:
              break;
            case AuthError.InvalidLinkDomain:
              break;
            case AuthError.RejectedCredential:
              break;
            case AuthError.PhoneNumberNotFound:
              break;
            case AuthError.InvalidTenantId:
              break;
            case AuthError.MissingClientIdentifier:
              break;
            case AuthError.MissingMultiFactorSession:
              break;
            case AuthError.MissingMultiFactorInfo:
              break;
            case AuthError.InvalidMultiFactorSession:
              break;
            case AuthError.MultiFactorInfoNotFound:
              break;
            case AuthError.AdminRestrictedOperation:
              break;
            case AuthError.UnverifiedEmail:
              break;
            case AuthError.SecondFactorAlreadyEnrolled:
              break;
            case AuthError.MaximumSecondFactorCountExceeded:
              break;
            case AuthError.UnsupportedFirstFactor:
              break;
            case AuthError.EmailChangeNeedsVerification:
              break;
            default:
              break;
          }
        }
        else
        {
          print("Email successfully send");
        }
      }
    }

    #region Login

    public void Login()
    {
      loadingScreen.SetActive(true);

      FirebaseAuth auth = FirebaseAuth.DefaultInstance;
      string email = loginEmail.text;
      string password = loginPassword.text;
     
      Credential credential =
        EmailAuthProvider.GetCredential(email, password);
      auth.SignInAndRetrieveDataWithCredentialAsync(credential)
        .ContinueWithOnMainThread(task =>
        {
          if (task.IsCanceled)
          {
            Debug.LogError("SignInAndRetrieveDataWithCredentialAsync was canceled.");
            return;
          }

          if (task.IsFaulted)
          {
            Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: "
                           + task.Exception);
            return;
          }

          loadingScreen.SetActive(false);
          AuthResult result = task.Result;
          Debug.LogFormat("User signed in successfully: {0} ({1})",
            result.User.DisplayName, result.User.UserId);
          loginUi.SetActive(false);
          successUi.SetActive(true);
          successUi.transform.Find("Desc").GetComponent<TextMeshProUGUI>().text = "Id: " + result.User.UserId;
        });
    }

    #endregion

    #region extra

    void ShowLogMessage(string msg)
    {
      logText.text = msg;
      logText.GetComponent<Animation>().Play("textFadeout");
    }

    #endregion
  }
}