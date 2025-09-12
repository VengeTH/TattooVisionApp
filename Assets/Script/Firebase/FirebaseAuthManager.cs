using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using System.Text.RegularExpressions;

public class FirebaseAuthManager : MonoBehaviour
{
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;

    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text loginErrorText;

    [Header("Registration")]
    public TMP_InputField nameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField confirmPasswordRegisterField;
    public TMP_Text registerErrorText;

    [Header("Password Reset")]
    public TMP_InputField resetEmailField;
    public TMP_Text resetFeedbackText;

    [Header("Show/Hide Password")]
    public TMP_InputField[] passwordFields;
    public Button[] showHideButtons;
    private bool[] isPasswordVisible;

    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Firebase dependency error: " + dependencyStatus);
            }
        });
    }

    private void Start()
    {
        isPasswordVisible = new bool[passwordFields.Length];
        for (int i = 0; i < showHideButtons.Length; i++)
        {
            int index = i;
            showHideButtons[i].onClick.AddListener(() => TogglePasswordVisibility(index));
        }

        for (int i = 0; i < passwordFields.Length; i++)
        {
            passwordFields[i].contentType = TMP_InputField.ContentType.Password;
            passwordFields[i].ForceLabelUpdate();
        }

        loginErrorText.text = "";
        registerErrorText.text = "";
        resetFeedbackText.text = "";
    }

    void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            user = auth.CurrentUser;
        }
    }

    public void Login()
    {
        loginErrorText.text = "";

        if (string.IsNullOrEmpty(emailLoginField.text) || string.IsNullOrEmpty(passwordLoginField.text))
        {
            loginErrorText.text = "Please fill in all login fields.";
            return;
        }

        StartCoroutine(LoginAsync(emailLoginField.text, passwordLoginField.text));
    }

    private IEnumerator LoginAsync(string email, string password)
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            FirebaseException firebaseEx = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError error = (AuthError)firebaseEx.ErrorCode;

            switch (error)
            {
                case AuthError.MissingEmail:
                    loginErrorText.text = "Email is missing.";
                    break;
                case AuthError.MissingPassword:
                    loginErrorText.text = "Password is missing.";
                    break;
                case AuthError.WrongPassword:
                    loginErrorText.text = "Incorrect password.";
                    break;
                case AuthError.InvalidEmail:
                    loginErrorText.text = "Invalid email format.";
                    break;
                case AuthError.UserNotFound:
                    loginErrorText.text = "Account not found.";
                    break;
                default:
                    loginErrorText.text = "Login failed. Try again.";
                    break;
            }
        }
        else
        {
            user = loginTask.Result.User;
            if (user.IsEmailVerified)
            {
                References.userName = user.DisplayName;
                UnityEngine.SceneManagement.SceneManager.LoadScene("Dashboard");
            }
            else
            {
                loginErrorText.text = "Please verify your email first.";
                SendEmailForVerification();
            }
        }
    }

    public void Register()
    {
        registerErrorText.text = "";

        string name = nameRegisterField.text;
        string email = emailRegisterField.text;
        string password = passwordRegisterField.text;
        string confirmPassword = confirmPasswordRegisterField.text;
        string passwordError = GetPasswordValidationErrors(password);

        if (!string.IsNullOrEmpty(passwordError))
        {
            registerErrorText.text = passwordError;
            return;
        }

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) ||
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            registerErrorText.text = "Please fill in all registration fields.";
            return;
        }

        if (password != confirmPassword)
        {
            registerErrorText.text = "Passwords do not match.";
            return;
        }

        if (!IsValidPassword(password))
        {
            registerErrorText.text = "Password must be at least 6 characters and include:\n" +
                                     "- One uppercase letter\n" +
                                     "- One lowercase letter\n" +
                                     "- One number\n" +
                                     "- One special character";
            return;
        }

        StartCoroutine(RegisterAsync(name, email, password));
    }

    public bool IsValidPassword(string password)
    {
        if (password.Length < 6) return false;

        bool hasUpper = Regex.IsMatch(password, @"[A-Z]");
        bool hasLower = Regex.IsMatch(password, @"[a-z]");
        bool hasDigit = Regex.IsMatch(password, @"\d");
        bool hasSpecial = Regex.IsMatch(password, @"[\W_]");

        return hasUpper && hasLower && hasDigit && hasSpecial;
    }

    public string GetPasswordValidationErrors(string password)
    {
        List<string> errors = new List<string>();

        if (password.Length < 6)
            errors.Add("at least 6 characters");

        if (!Regex.IsMatch(password, @"[A-Z]"))
            errors.Add("an uppercase letter");

        if (!Regex.IsMatch(password, @"[a-z]"))
            errors.Add("a lowercase letter");

        if (!Regex.IsMatch(password, @"\d"))
            errors.Add("a number");

        if (!Regex.IsMatch(password, @"[\W_]"))
            errors.Add("a special character (e.g., !, @, #, _)");

        if (errors.Count == 0)
            return ""; // Valid password

        return "Password must contain " + string.Join(", ", errors) + ".";
    }

    private IEnumerator RegisterAsync(string name, string email, string password)
    {
        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.Exception != null)
        {
            FirebaseException firebaseEx = registerTask.Exception.GetBaseException() as FirebaseException;
            AuthError error = (AuthError)firebaseEx.ErrorCode;

            switch (error)
            {
                case AuthError.EmailAlreadyInUse:
                    registerErrorText.text = "Email already in use.";
                    break;
                case AuthError.InvalidEmail:
                    registerErrorText.text = "Invalid email.";
                    break;
                case AuthError.WeakPassword:
                    registerErrorText.text = "Password is too weak.";
                    break;
                default:
                    registerErrorText.text = "Registration failed.";
                    break;
            }
        }
        else
        {
            user = registerTask.Result.User;
            UserProfile profile = new UserProfile { DisplayName = name };

            var profileTask = user.UpdateUserProfileAsync(profile);
            yield return new WaitUntil(() => profileTask.IsCompleted);

            if (profileTask.Exception != null)
            {
                registerErrorText.text = "Failed to set display name.";
                user.DeleteAsync();
            }
            else
            {
                user.SendEmailVerificationAsync();
                UIManager.Instance.ShowVerificationResponse(true, email, "");
                StartCoroutine(WaitForEmailVerification());
            }
        }
    }

    private IEnumerator WaitForEmailVerification()
    {
        float timer = 90f;

        while (timer > 0f)
        {
            yield return new WaitForSeconds(1f);
            timer -= 1f;

            yield return user.ReloadAsync();

            if (user.IsEmailVerified)
            {
                if (UIManager.Instance != null)
                {
                    // Stop the countdown
                    UIManager.Instance.StopAllCoroutines();
                    UIManager.Instance.countdownText.text = "";

                    // Update the UI
                    UIManager.Instance.UpdateEmailVerificationText("You are now verified. Please log in to your account.");
                }
                yield break;
            }
        }

        registerErrorText.text = "Email not verified in time. Please register again.";
        Debug.LogWarning("Email not verified in time. Deleting user...");
        user.DeleteAsync();
    }


    public void SendEmailForVerification()
    {
        if (user != null)
        {
            string email = user.Email;
            user.SendEmailVerificationAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    UIManager.Instance.ShowVerificationResponse(true, email, "");
                }
                else
                {
                    string errorMsg = task.Exception?.GetBaseException().Message ?? "Unknown error";
                    UIManager.Instance.ShowVerificationResponse(false, email, errorMsg);
                }
            });
        }
    }

    public void SendPasswordResetEmail()
    {
        resetFeedbackText.text = "";
        string email = resetEmailField.text.Trim();

        if (string.IsNullOrEmpty(email))
        {
            resetFeedbackText.text = "Please enter your email.";
            return;
        }

        auth.SendPasswordResetEmailAsync(email).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                FirebaseException firebaseEx = task.Exception?.GetBaseException() as FirebaseException;
                AuthError errorCode = firebaseEx != null ? (AuthError)firebaseEx.ErrorCode : 0;

                switch (errorCode)
                {
                    case AuthError.InvalidEmail:
                        resetFeedbackText.text = "Invalid email format.";
                        break;
                    default:
                        resetFeedbackText.text = "Something went wrong. Please try again.";
                        break;
                }
            }
            else
            {
                resetFeedbackText.text = $"Reset email sent to {email}. Please check your inbox.";
            }
        });
    }

    private void TogglePasswordVisibility(int index)
    {
        isPasswordVisible[index] = !isPasswordVisible[index];
        passwordFields[index].contentType = isPasswordVisible[index]
            ? TMP_InputField.ContentType.Standard
            : TMP_InputField.ContentType.Password;
        passwordFields[index].ForceLabelUpdate();
    }
}
