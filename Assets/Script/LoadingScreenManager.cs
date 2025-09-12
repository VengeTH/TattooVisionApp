using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingScreenManager : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "Create Account"; // Change to your actual next scene name

    void Start()
    {
        Debug.Log("LoadingManager script started");
        StartCoroutine(LoadNextScene());
    }

    IEnumerator LoadNextScene()
    {
        // Optional delay to show a logo or animation
        yield return new WaitForSeconds(2f);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(nextSceneName);

        // Optional: wait until it's done loading
        while (!loadOperation.isDone)
        {
            yield return null;
        }
    }
}
