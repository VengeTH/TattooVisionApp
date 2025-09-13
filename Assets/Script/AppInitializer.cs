using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// * Handles initial app startup and authentication flow
/// * Ensures proper scene loading based on user authentication status
/// * Works with or without Firebase
/// </summary>
public class AppInitializer : MonoBehaviour
{
    [Header("Scene Configuration")]
    [SerializeField] private string loginSceneName = "Dashboard"; // Dashboard will handle auth UI
    [SerializeField] private string mainSceneName = "Dashboard";

    [Header("Loading Configuration")]
    [SerializeField] private float minimumLoadingTime = 2f; // * Minimum time to show splash screen

    [Header("Firebase Configuration")]
    [SerializeField] private bool useFirebase = false; // * Set to true if Firebase is installed

    private bool firebaseInitialized = false;

    void Start()
    {
        Debug.Log("AppInitializer: Starting app initialization...");
        StartCoroutine(InitializeApp());
    }

    IEnumerator InitializeApp()
    {
        float startTime = Time.time;

        // * Try to initialize Firebase if enabled
        if (useFirebase)
        {
            yield return StartCoroutine(InitializeFirebase());
        }
        else
        {
            Debug.Log("AppInitializer: Firebase disabled, using guest mode");
        }

        // * Wait for minimum loading time to show splash screen
        float elapsedTime = Time.time - startTime;
        if (elapsedTime < minimumLoadingTime)
        {
            yield return new WaitForSeconds(minimumLoadingTime - elapsedTime);
        }

        // * Load appropriate scene based on Firebase initialization status
        if (firebaseInitialized && useFirebase)
        {
            LoadScene(mainSceneName);
        }
        else
        {
            // * If Firebase not initialized or disabled, load login scene for authentication
            LoadScene(loginSceneName);
        }
    }

    IEnumerator InitializeFirebase()
    {
        Debug.Log("AppInitializer: Initializing Firebase...");

        bool hasError = false;
        try
        {
            // * Only try Firebase if the assemblies are available
#if FIREBASE_INSTALLED
            var task = Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
            yield return new WaitUntil(() => task.IsCompleted);

            if (task.Exception != null)
            {
                Debug.LogError($"AppInitializer: Firebase initialization failed: {task.Exception}");
                firebaseInitialized = false;
            }
            else
            {
                Debug.Log("AppInitializer: Firebase initialized successfully");
                firebaseInitialized = true;
            }
#else
            Debug.Log("AppInitializer: Firebase assemblies not found, running in guest mode");
            firebaseInitialized = false;
#endif
        }
        catch (System.Exception e)
        {
            Debug.LogError($"AppInitializer: Firebase initialization error: {e.Message}");
            firebaseInitialized = false;
            hasError = true;
        }

        if (hasError)
        {
            yield return null;
        }
    }

    void LoadScene(string sceneName)
    {
        Debug.Log($"AppInitializer: Loading scene: {sceneName}");

        try
        {
            SceneManager.LoadScene(sceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"AppInitializer: Failed to load scene {sceneName}: {e.Message}");
            // * Fallback to Dashboard if scene loading fails
            if (sceneName != "Dashboard")
            {
                Debug.Log("AppInitializer: Falling back to Dashboard scene");
                SceneManager.LoadScene("Dashboard");
            }
        }
    }

    // * Public method to manually trigger scene loading (for testing)
    public void ForceLoadScene(string sceneName)
    {
        LoadScene(sceneName);
    }
}
