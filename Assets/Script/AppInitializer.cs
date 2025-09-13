using UnityEngine;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;

/// <summary>
/// * Handles initial app startup and authentication flow
/// * Ensures proper scene loading based on user authentication status
/// </summary>
public class AppInitializer : MonoBehaviour
{
    [Header("Scene Configuration")]
    [SerializeField] private string loginSceneName = "Dashboard"; // Dashboard will handle auth UI
    [SerializeField] private string mainSceneName = "Dashboard";
    
    [Header("Loading Configuration")]
    [SerializeField] private float minimumLoadingTime = 2f; // * Minimum time to show splash screen
    
    private FirebaseAuth auth;
    private bool firebaseInitialized = false;
    
    void Start()
    {
        Debug.Log("AppInitializer: Starting app initialization...");
        StartCoroutine(InitializeApp());
    }
    
    IEnumerator InitializeApp()
    {
        float startTime = Time.time;
        
        // * Initialize Firebase first
        yield return StartCoroutine(InitializeFirebase());
        
        // * Wait for minimum loading time to show splash screen
        float elapsedTime = Time.time - startTime;
        if (elapsedTime < minimumLoadingTime)
        {
            yield return new WaitForSeconds(minimumLoadingTime - elapsedTime);
        }
        
        // * Check authentication status and load appropriate scene
        CheckAuthenticationAndLoadScene();
    }
    
    IEnumerator InitializeFirebase()
    {
        Debug.Log("AppInitializer: Initializing Firebase...");
        
        var task = FirebaseApp.CheckAndFixDependenciesAsync();
        yield return new WaitUntil(() => task.IsCompleted);
        
        if (task.Exception != null)
        {
            Debug.LogError($"AppInitializer: Firebase initialization failed: {task.Exception}");
            // * Continue without Firebase for now
            firebaseInitialized = false;
        }
        else
        {
            Debug.Log("AppInitializer: Firebase initialized successfully");
            auth = FirebaseAuth.DefaultInstance;
            firebaseInitialized = true;
        }
    }
    
    void CheckAuthenticationAndLoadScene()
    {
        Debug.Log("AppInitializer: Checking authentication status...");
        
        if (firebaseInitialized && auth != null && auth.CurrentUser != null)
        {
            Debug.Log($"AppInitializer: User authenticated: {auth.CurrentUser.Email}");
            // * User is logged in, go to main scene
            LoadScene(mainSceneName);
        }
        else
        {
            Debug.Log("AppInitializer: No authenticated user, loading login scene");
            // * User not logged in, go to login scene
            LoadScene(loginSceneName);
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
