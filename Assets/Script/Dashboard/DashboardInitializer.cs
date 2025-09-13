using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// * Initializes the Dashboard scene and handles authentication state
/// * Provides fallback for unauthenticated users
/// * Includes emergency fallback to prevent black screens
/// * Works with or without Firebase
/// </summary>
public class DashboardInitializer : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private bool allowGuestMode = true;
    [SerializeField] private string loginSceneName = "Login";
    [SerializeField] private float initializationTimeout = 10f; // * Emergency timeout
    [SerializeField] private bool useFirebase = false; // * Set to true if Firebase is installed

    [Header("UI Elements")]
    [SerializeField] private GameObject guestModePanel;
    [SerializeField] private GameObject authenticatedUserPanel;

    private bool firebaseInitialized = false;
    private bool initializationComplete = false;

    void Start()
    {
        Debug.Log("DashboardInitializer: Starting dashboard initialization...");
        StartCoroutine(InitializeDashboard());

        // * Emergency timeout to prevent black screen
        StartCoroutine(EmergencyTimeout());
    }

    IEnumerator InitializeDashboard()
    {
        // * Try to initialize Firebase if enabled
        if (useFirebase)
        {
            yield return StartCoroutine(InitializeFirebase());
        }
        else
        {
            Debug.Log("DashboardInitializer: Firebase disabled, using guest mode");
        }

        // * Check authentication status
        CheckAuthenticationStatus();

        initializationComplete = true;
        Debug.Log("DashboardInitializer: Initialization complete");
    }

    IEnumerator InitializeFirebase()
    {
        Debug.Log("DashboardInitializer: Initializing Firebase...");

        bool hasError = false;
        try
        {
#if FIREBASE_INSTALLED
            var task = Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Exception != null)
            {
                Debug.LogError($"DashboardInitializer: Firebase initialization failed: {task.Exception}");
                firebaseInitialized = false;
            }
            else
            {
                Debug.Log("DashboardInitializer: Firebase initialized successfully");
                firebaseInitialized = true;
            }
#else
            Debug.Log("DashboardInitializer: Firebase assemblies not found, running in guest mode");
            firebaseInitialized = false;
#endif
        }
        catch (System.Exception e)
        {
            Debug.LogError($"DashboardInitializer: Firebase initialization error: {e.Message}");
            firebaseInitialized = false;
            hasError = true;
        }

        if (hasError)
        {
            yield return null;
        }
    }

    void CheckAuthenticationStatus()
    {
        Debug.Log("DashboardInitializer: Checking authentication status...");

        if (firebaseInitialized && CheckFirebaseUser())
        {
            Debug.Log("DashboardInitializer: User authenticated");
            ShowAuthenticatedUserUI();
        }
        else
        {
            Debug.Log("DashboardInitializer: No authenticated user");

            if (allowGuestMode)
            {
                Debug.Log("DashboardInitializer: Enabling guest mode");
                ShowGuestModeUI();
            }
            else
            {
                Debug.Log("DashboardInitializer: Redirecting to login");
                RedirectToLogin();
            }
        }
    }

    bool CheckFirebaseUser()
    {
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

    void ShowAuthenticatedUserUI()
    {
        if (authenticatedUserPanel != null)
            authenticatedUserPanel.SetActive(true);

        if (guestModePanel != null)
            guestModePanel.SetActive(false);
    }

    void ShowGuestModeUI()
    {
        if (guestModePanel != null)
            guestModePanel.SetActive(true);

        if (authenticatedUserPanel != null)
            authenticatedUserPanel.SetActive(false);
    }

    void RedirectToLogin()
    {
        Debug.Log($"DashboardInitializer: Redirecting to login scene: {loginSceneName}");
        SceneManager.LoadScene(loginSceneName);
    }

    IEnumerator EmergencyTimeout()
    {
        yield return new WaitForSeconds(initializationTimeout);

        if (!initializationComplete)
        {
            Debug.LogWarning($"DashboardInitializer: Initialization timeout reached after {initializationTimeout}s");

            // * Emergency fallback - show guest mode
            if (allowGuestMode)
            {
                Debug.Log("DashboardInitializer: Emergency fallback - showing guest mode");
                ShowGuestModeUI();
            }
            else
            {
                Debug.Log("DashboardInitializer: Emergency fallback - redirecting to login");
                RedirectToLogin();
            }
        }
    }

    // * Public methods for UI buttons
    public void OnLoginButtonClicked()
    {
        RedirectToLogin();
    }

    public void OnContinueAsGuestClicked()
    {
        Debug.Log("DashboardInitializer: User chose to continue as guest");
        ShowGuestModeUI();
    }

    public void OnSignUpButtonClicked()
    {
        RedirectToLogin();
    }
}
