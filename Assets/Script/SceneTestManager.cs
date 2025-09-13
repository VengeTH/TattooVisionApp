using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// * Test script to verify scene loading and navigation
/// * Provides debug information and manual scene switching
/// </summary>
public class SceneTestManager : MonoBehaviour
{
    [Header("Debug UI")]
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private Button testButton;
    [SerializeField] private Button nextSceneButton;
    
    [Header("Configuration")]
    [SerializeField] private string[] testScenes = {"Login", "Dashboard", "ARCamera", "Profile", "Gallery"};
    [SerializeField] private int currentSceneIndex = 0;
    
    void Start()
    {
        Debug.Log("SceneTestManager: Starting scene test...");
        UpdateDebugInfo();
        
        if (testButton != null)
            testButton.onClick.AddListener(TestCurrentScene);
        
        if (nextSceneButton != null)
            nextSceneButton.onClick.AddListener(LoadNextScene);
    }
    
    void UpdateDebugInfo()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        
        string debugInfo = $"Current Scene: {currentScene}\n";
        debugInfo += $"Scene Index: {currentSceneIndex}\n";
        debugInfo += $"Total Scenes in Build: {sceneCount}\n";
        debugInfo += $"Canvas Count: {FindObjectsOfType<Canvas>().Length}\n";
        debugInfo += $"Active GameObjects: {FindObjectsOfType<GameObject>().Length}\n";
        
        if (debugText != null)
            debugText.text = debugInfo;
        
        Debug.Log($"SceneTestManager: {debugInfo}");
    }
    
    void TestCurrentScene()
    {
        Debug.Log("SceneTestManager: Testing current scene...");
        
        // * Check for common issues
        Canvas[] canvases = FindObjectsOfType<Canvas>();
        if (canvases.Length == 0)
        {
            Debug.LogWarning("SceneTestManager: No Canvas found - potential black screen issue");
        }
        else
        {
            Debug.Log($"SceneTestManager: Found {canvases.Length} Canvas(es)");
            foreach (Canvas canvas in canvases)
            {
                Debug.Log($"Canvas: {canvas.name}, Active: {canvas.gameObject.activeInHierarchy}");
            }
        }
        
        // * Check for UI elements
        Button[] buttons = FindObjectsOfType<Button>();
        TextMeshProUGUI[] texts = FindObjectsOfType<TextMeshProUGUI>();
        
        Debug.Log($"SceneTestManager: Found {buttons.Length} buttons and {texts.Length} text elements");
        
        UpdateDebugInfo();
    }
    
    void LoadNextScene()
    {
        if (testScenes.Length == 0) return;
        
        currentSceneIndex = (currentSceneIndex + 1) % testScenes.Length;
        string nextScene = testScenes[currentSceneIndex];
        
        Debug.Log($"SceneTestManager: Loading next scene: {nextScene}");
        
        try
        {
            SceneManager.LoadScene(nextScene);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"SceneTestManager: Failed to load scene {nextScene}: {e.Message}");
        }
    }
    
    // * Public method to load specific scene
    public void LoadScene(string sceneName)
    {
        Debug.Log($"SceneTestManager: Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
    
    // * Public method to reload current scene
    public void ReloadCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        Debug.Log($"SceneTestManager: Reloading current scene: {currentScene}");
        SceneManager.LoadScene(currentScene);
    }
}
