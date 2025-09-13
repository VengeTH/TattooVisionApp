using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

/// <summary>
/// * Manages the login/authentication scene
/// * Handles user login, registration, and navigation to main app
/// </summary>
public class LoginSceneManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject registerPanel;
    [SerializeField] private GameObject loadingPanel;

    [Header("Login UI")]
    [SerializeField] private TMP_InputField emailLoginField;
    [SerializeField] private TMP_InputField passwordLoginField;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button showRegisterButton;
    [SerializeField] private TMP_Text loginErrorText;

    [Header("Register UI")]
    [SerializeField] private TMP_InputField nameRegisterField;
    [SerializeField] private TMP_InputField emailRegisterField;
    [SerializeField] private TMP_InputField passwordRegisterField;
    [SerializeField] private TMP_InputField confirmPasswordRegisterField;
    [SerializeField] private Button registerButton;
    [SerializeField] private Button showLoginButton;
    [SerializeField] private TMP_Text registerErrorText;

    [Header("Loading UI")]
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private Slider loadingProgress;

    [Header("Configuration")]
    [SerializeField] private string mainSceneName = "Dashboard";
    [SerializeField] private float loadingDelay = 1f;
    [SerializeField] private bool useFirebase = false; // * Set to true if Firebase is installed

    private bool firebaseInitialized = false;
    
    void Start()
    {
        Debug.Log("LoginSceneManager: Starting login scene...");
        InitializeUI();

        if (useFirebase)
        {
            StartCoroutine(InitializeFirebase());
        }
        else
        {
            Debug.Log("LoginSceneManager: Firebase disabled, showing guest login");
            ShowGuestLoginOption();
        }
    }
    
    void InitializeUI()
    {
        // * Set up button listeners
        if (loginButton != null) loginButton.onClick.AddListener(OnLoginClicked);
        if (registerButton != null) registerButton.onClick.AddListener(OnRegisterClicked);
        if (showRegisterButton != null) showRegisterButton.onClick.AddListener(ShowRegisterPanel);
        if (showLoginButton != null) showLoginButton.onClick.AddListener(ShowLoginPanel);

        // * Initialize UI state
        ShowLoginPanel();
        if (loadingPanel != null) loadingPanel.SetActive(false);

        // * Clear error messages
        if (loginErrorText != null) loginErrorText.text = "";
        if (registerErrorText != null) registerErrorText.text = "";
    }

    void ShowGuestLoginOption()
    {
        // * Check if user is already authenticated (if Firebase is available)
        if (CheckFirebaseUser())
        {
            Debug.Log("LoginSceneManager: User already authenticated, loading main scene");
            LoadMainScene();
        }
        else
        {
            Debug.Log("LoginSceneManager: Guest mode - showing login options");
            // * Show a message about guest mode
            if (loginErrorText != null)
            {
                loginErrorText.text = "Firebase not available. Running in guest mode.";
            }
        }
    }

    bool CheckFirebaseUser()
    {
        if (!useFirebase) return false;

#if FIREBASE_INSTALLED
        try
        {
            var auth = Firebase.FirebaseAuth.DefaultInstance;
            return auth != null && auth.CurrentUser != null;
        }
        catch
        {
            return false;
        }
#else
        return false;
#endif
    }
    
    IEnumerator InitializeFirebase()
    {
        Debug.Log("LoginSceneManager: Initializing Firebase...");

        bool hasError = false;
        try
        {
#if FIREBASE_INSTALLED
            var task = Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Exception != null)
            {
                Debug.LogError($"LoginSceneManager: Firebase initialization failed: {task.Exception}");
                if (loginErrorText != null)
                    loginErrorText.text = "Firebase initialization failed. Please check your internet connection.";
                firebaseInitialized = false;
            }
            else
            {
                Debug.Log("LoginSceneManager: Firebase initialized successfully");
                firebaseInitialized = true;

                // * Check if user is already logged in
                if (CheckFirebaseUser())
                {
                    Debug.Log("LoginSceneManager: User already authenticated, loading main scene");
                    LoadMainScene();
                }
            }
#else
            Debug.Log("LoginSceneManager: Firebase assemblies not found");
            if (loginErrorText != null)
                loginErrorText.text = "Firebase not available. Running in guest mode.";
            firebaseInitialized = false;
#endif
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LoginSceneManager: Firebase error: {e.Message}");
            if (loginErrorText != null)
                loginErrorText.text = "Firebase error. Running in guest mode.";
            firebaseInitialized = false;
            hasError = true;
        }

        if (hasError)
        {
            yield return null;
        }
    }
    
    void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        loginErrorText.text = "";
    }
    
    void ShowRegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        registerErrorText.text = "";
    }
    
    void OnLoginClicked()
    {
        if (!useFirebase)
        {
            Debug.Log("LoginSceneManager: Firebase not available, proceeding as guest");
            LoadMainScene();
            return;
        }

        if (!firebaseInitialized)
        {
            if (loginErrorText != null)
                loginErrorText.text = "Firebase not initialized. Please wait...";
            return;
        }

        string email = "";
        string password = "";

        if (emailLoginField != null) email = emailLoginField.text.Trim();
        if (passwordLoginField != null) password = passwordLoginField.text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            if (loginErrorText != null)
                loginErrorText.text = "Please fill in all fields.";
            return;
        }

        StartCoroutine(LoginUser(email, password));
    }
    
    void OnRegisterClicked()
    {
        if (!useFirebase)
        {
            Debug.Log("LoginSceneManager: Firebase not available, proceeding as guest");
            LoadMainScene();
            return;
        }

        if (!firebaseInitialized)
        {
            if (registerErrorText != null)
                registerErrorText.text = "Firebase not initialized. Please wait...";
            return;
        }

        string name = "";
        string email = "";
        string password = "";
        string confirmPassword = "";

        if (nameRegisterField != null) name = nameRegisterField.text.Trim();
        if (emailRegisterField != null) email = emailRegisterField.text.Trim();
        if (passwordRegisterField != null) password = passwordRegisterField.text;
        if (confirmPasswordRegisterField != null) confirmPassword = confirmPasswordRegisterField.text;

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) ||
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            if (registerErrorText != null)
                registerErrorText.text = "Please fill in all fields.";
            return;
        }

        if (password != confirmPassword)
        {
            if (registerErrorText != null)
                registerErrorText.text = "Passwords do not match.";
            return;
        }

        if (password.Length < 6)
        {
            if (registerErrorText != null)
                registerErrorText.text = "Password must be at least 6 characters long.";
            return;
        }

        StartCoroutine(RegisterUser(name, email, password));
    }
    
    IEnumerator LoginUser(string email, string password)
    {
        ShowLoading("Logging in...");

        bool hasError = false;
        try
        {
#if FIREBASE_INSTALLED
            var auth = Firebase.FirebaseAuth.DefaultInstance;
            var task = auth.SignInWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Exception != null)
            {
                Debug.LogError($"LoginSceneManager: Login failed: {task.Exception}");
                HideLoading();
                if (loginErrorText != null)
                    loginErrorText.text = "Login failed. Please check your credentials.";
            }
            else
            {
                Debug.Log("LoginSceneManager: Login successful");
                LoadMainScene();
            }
#else
            Debug.Log("LoginSceneManager: Firebase not available for login");
            HideLoading();
            LoadMainScene(); // * Proceed as guest
#endif
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LoginSceneManager: Login error: {e.Message}");
            HideLoading();
            if (loginErrorText != null)
                loginErrorText.text = "Login error. Proceeding as guest.";
            LoadMainScene();
            hasError = true;
        }

        if (hasError)
        {
            yield return null;
        }
    }

    IEnumerator RegisterUser(string name, string email, string password)
    {
        ShowLoading("Creating account...");

        bool hasError = false;
        try
        {
#if FIREBASE_INSTALLED
            var auth = Firebase.FirebaseAuth.DefaultInstance;
            var task = auth.CreateUserWithEmailAndPasswordAsync(email, password);
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Exception != null)
            {
                Debug.LogError($"LoginSceneManager: Registration failed: {task.Exception}");
                HideLoading();
                if (registerErrorText != null)
                    registerErrorText.text = "Registration failed. Email may already be in use.";
            }
            else
            {
                Debug.Log("LoginSceneManager: Registration successful");

                // * Update user profile with name
                var user = task.Result.User;
                var profileTask = user.UpdateUserProfileAsync(new Firebase.UserProfile { DisplayName = name });
                yield return new WaitUntil(() => profileTask.IsCompleted);

                LoadMainScene();
            }
#else
            Debug.Log("LoginSceneManager: Firebase not available for registration");
            HideLoading();
            LoadMainScene(); // * Proceed as guest
#endif
        }
        catch (System.Exception e)
        {
            Debug.LogError($"LoginSceneManager: Registration error: {e.Message}");
            HideLoading();
            if (registerErrorText != null)
                registerErrorText.text = "Registration error. Proceeding as guest.";
            LoadMainScene();
            hasError = true;
        }

        if (hasError)
        {
            yield return null;
        }
    }
    
    void ShowLoading(string message)
    {
        if (loadingPanel != null) loadingPanel.SetActive(true);
        if (loadingText != null) loadingText.text = message;
        if (loadingProgress != null) loadingProgress.value = 0f;

        // * Animate progress bar
        StartCoroutine(AnimateProgressBar());
    }

    void HideLoading()
    {
        if (loadingPanel != null) loadingPanel.SetActive(false);
    }
    
    IEnumerator AnimateProgressBar()
    {
        float duration = loadingDelay;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (loadingProgress != null)
                loadingProgress.value = Mathf.Lerp(0f, 1f, elapsed / duration);
            yield return null;
        }

        if (loadingProgress != null)
            loadingProgress.value = 1f;
    }

    void LoadMainScene()
    {
        Debug.Log($"LoginSceneManager: Loading main scene: {mainSceneName}");
        SceneManager.LoadScene(mainSceneName);
    }

    // * Public method for guest login (skip authentication)
    public void GuestLogin()
    {
        Debug.Log("LoginSceneManager: Guest login selected");
        LoadMainScene();
    }
}
