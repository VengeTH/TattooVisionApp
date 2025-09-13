using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// * Simple scene transition manager
/// * Automatically loads the next scene after a delay
/// * Prevents black screen by ensuring scene progression
/// </summary>
public class SimpleSceneTransition : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string nextSceneName = "Dashboard";
    [SerializeField] private float transitionDelay = 2f;
    
    void Start()
    {
        Debug.Log($"SimpleSceneTransition: Starting transition to {nextSceneName} in {transitionDelay} seconds");
        StartCoroutine(TransitionAfterDelay());
    }
    
    IEnumerator TransitionAfterDelay()
    {
        yield return new WaitForSeconds(transitionDelay);
        
        Debug.Log($"SimpleSceneTransition: Loading scene: {nextSceneName}");
        
        try
        {
            SceneManager.LoadScene(nextSceneName);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"SimpleSceneTransition: Failed to load scene {nextSceneName}: {e.Message}");
            
            // * Emergency fallback
            if (nextSceneName != "Dashboard")
            {
                Debug.Log("SimpleSceneTransition: Falling back to Dashboard");
                SceneManager.LoadScene("Dashboard");
            }
        }
    }
    
    // * Public method to skip the delay
    public void SkipTransition()
    {
        Debug.Log("SimpleSceneTransition: Skipping transition delay");
        StopAllCoroutines();
        LoadNextScene();
    }
    
    // * Public method to load specific scene
    public void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
