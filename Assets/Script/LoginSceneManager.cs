using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
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
    
    private FirebaseAuth auth;
    private bool firebaseInitialized = false;
    
    void Start()
    {
        Debug.Log("LoginSceneManager: Starting login scene...");
        InitializeUI();
        StartCoroutine(InitializeFirebase());
    }
    
    void InitializeUI()
    {
        // * Set up button listeners
        loginButton.onClick.AddListener(OnLoginClicked);
        registerButton.onClick.AddListener(OnRegisterClicked);
        showRegisterButton.onClick.AddListener(ShowRegisterPanel);
        showLoginButton.onClick.AddListener(ShowLoginPanel);
        
        // * Initialize UI state
        ShowLoginPanel();
        loadingPanel.SetActive(false);
        
        // * Clear error messages
        loginErrorText.text = "";
        registerErrorText.text = "";
    }
    
    IEnumerator InitializeFirebase()
    {
        Debug.Log("LoginSceneManager: Initializing Firebase...");
        
        var task = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => task.IsCompleted);
        
        if (task.Exception != null)
        {
            Debug.LogError($"LoginSceneManager: Firebase initialization failed: {task.Exception}");
            loginErrorText.text = "Firebase initialization failed. Please check your internet connection.";
            firebaseInitialized = false;
        }
        else
        {
            Debug.Log("LoginSceneManager: Firebase initialized successfully");
            auth = FirebaseAuth.DefaultInstance;
            firebaseInitialized = true;
            
            // * Check if user is already logged in
            if (auth.CurrentUser != null)
            {
                Debug.Log("LoginSceneManager: User already authenticated, loading main scene");
                LoadMainScene();
            }
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
        if (!firebaseInitialized)
        {
            loginErrorText.text = "Firebase not initialized. Please wait...";
            return;
        }
        
        string email = emailLoginField.text.Trim();
        string password = passwordLoginField.text;
        
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            loginErrorText.text = "Please fill in all fields.";
            return;
        }
        
        StartCoroutine(LoginUser(email, password));
    }
    
    void OnRegisterClicked()
    {
        if (!firebaseInitialized)
        {
            registerErrorText.text = "Firebase not initialized. Please wait...";
            return;
        }
        
        string name = nameRegisterField.text.Trim();
        string email = emailRegisterField.text.Trim();
        string password = passwordRegisterField.text;
        string confirmPassword = confirmPasswordRegisterField.text;
        
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || 
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            registerErrorText.text = "Please fill in all fields.";
            return;
        }
        
        if (password != confirmPassword)
        {
            registerErrorText.text = "Passwords do not match.";
            return;
        }
        
        if (password.Length < 6)
        {
            registerErrorText.text = "Password must be at least 6 characters long.";
            return;
        }
        
        StartCoroutine(RegisterUser(name, email, password));
    }
    
    IEnumerator LoginUser(string email, string password)
    {
        ShowLoading("Logging in...");
        
        var task = auth.SignInWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => task.IsCompleted);
        
        if (task.Exception != null)
        {
            Debug.LogError($"LoginSceneManager: Login failed: {task.Exception}");
            HideLoading();
            loginErrorText.text = "Login failed. Please check your credentials.";
        }
        else
        {
            Debug.Log("LoginSceneManager: Login successful");
            LoadMainScene();
        }
    }
    
    IEnumerator RegisterUser(string name, string email, string password)
    {
        ShowLoading("Creating account...");
        
        var task = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => task.IsCompleted);
        
        if (task.Exception != null)
        {
            Debug.LogError($"LoginSceneManager: Registration failed: {task.Exception}");
            HideLoading();
            registerErrorText.text = "Registration failed. Email may already be in use.";
        }
        else
        {
            Debug.Log("LoginSceneManager: Registration successful");
            
            // * Update user profile with name
            var user = task.Result.User;
            var profileTask = user.UpdateUserProfileAsync(new UserProfile { DisplayName = name });
            yield return new WaitUntil(() => profileTask.IsCompleted);
            
            LoadMainScene();
        }
    }
    
    void ShowLoading(string message)
    {
        loadingPanel.SetActive(true);
        loadingText.text = message;
        loadingProgress.value = 0f;
        
        // * Animate progress bar
        StartCoroutine(AnimateProgressBar());
    }
    
    void HideLoading()
    {
        loadingPanel.SetActive(false);
    }
    
    IEnumerator AnimateProgressBar()
    {
        float duration = loadingDelay;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            loadingProgress.value = Mathf.Lerp(0f, 1f, elapsed / duration);
            yield return null;
        }
        
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
