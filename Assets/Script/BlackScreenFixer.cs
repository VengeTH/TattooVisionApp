using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// * Emergency fallback script to prevent black screen issues
/// * Automatically loads Dashboard scene if no other scene is active
/// * Provides a safety net for app startup
/// </summary>
public class BlackScreenFixer : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float checkInterval = 2f;
    [SerializeField] private float maxWaitTime = 10f;
    [SerializeField] private string fallbackScene = "Dashboard";
    
    private float startTime;
    private bool hasFixed = false;
    
    void Start()
    {
        Debug.Log("BlackScreenFixer: Starting black screen prevention...");
        startTime = Time.time;
        
        // * Start checking for black screen condition
        StartCoroutine(CheckForBlackScreen());
    }
    
    IEnumerator CheckForBlackScreen()
    {
        while (!hasFixed && (Time.time - startTime) < maxWaitTime)
        {
            yield return new WaitForSeconds(checkInterval);
            
            // * Check if we're still on the same scene and nothing is happening
            if (IsBlackScreenCondition())
            {
                Debug.LogWarning("BlackScreenFixer: Detected potential black screen, applying fix...");
                ApplyFix();
                break;
            }
        }
        
        if (!hasFixed)
        {
            Debug.LogWarning("BlackScreenFixer: Timeout reached, applying emergency fix...");
            ApplyFix();
        }
    }
    
    bool IsBlackScreenCondition()
    {
        // * Check if we're in a problematic state
        string currentScene = SceneManager.GetActiveScene().name;
        
        // * If we're on a scene that should have UI but doesn't, it might be a black screen
        if (currentScene == "Login" || currentScene == "Dashboard")
        {
            // * Check if there are any active UI elements
            Canvas[] canvases = FindObjectsOfType<Canvas>();
            if (canvases.Length == 0)
            {
                Debug.Log("BlackScreenFixer: No Canvas found, potential black screen");
                return true;
            }
            
            // * Check if all canvases are inactive
            bool anyCanvasActive = false;
            foreach (Canvas canvas in canvases)
            {
                if (canvas.gameObject.activeInHierarchy)
                {
                    anyCanvasActive = true;
                    break;
                }
            }
            
            if (!anyCanvasActive)
            {
                Debug.Log("BlackScreenFixer: No active Canvas found, potential black screen");
                return true;
            }
        }
        
        return false;
    }
    
    void ApplyFix()
    {
        if (hasFixed) return;
        
        hasFixed = true;
        Debug.Log($"BlackScreenFixer: Loading fallback scene: {fallbackScene}");
        
        try
        {
            SceneManager.LoadScene(fallbackScene);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"BlackScreenFixer: Failed to load fallback scene: {e.Message}");
            
            // * Last resort: try to reload current scene
            try
            {
                string currentScene = SceneManager.GetActiveScene().name;
                Debug.Log($"BlackScreenFixer: Attempting to reload current scene: {currentScene}");
                SceneManager.LoadScene(currentScene);
            }
            catch (System.Exception e2)
            {
                Debug.LogError($"BlackScreenFixer: Failed to reload current scene: {e2.Message}");
            }
        }
    }
    
    // * Public method to manually trigger fix (for testing)
    public void ForceFix()
    {
        ApplyFix();
    }
}
