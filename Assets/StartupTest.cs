using UnityEngine;

/// <summary>
/// Simple startup test to verify our scripts are running
/// </summary>
public class StartupTest : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("🎯 StartupTest: Awake() - App is starting!");
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        Debug.Log("🎯 StartupTest: Start() - App has started!");
        Debug.Log($"🎯 Device: {SystemInfo.deviceModel}");
        Debug.Log($"🎯 OS: {SystemInfo.operatingSystem}");
        Debug.Log($"🎯 Graphics: {SystemInfo.graphicsDeviceName}");
        Debug.Log($"🎯 Camera Permission: {Application.HasUserAuthorization(UserAuthorization.WebCam)}");

        // Test AR Foundation availability
        try
        {
            var arSession = FindObjectOfType<UnityEngine.XR.ARFoundation.ARSession>();
            var xrOrigin = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();
            var cameraManager = FindObjectOfType<UnityEngine.XR.ARFoundation.ARCameraManager>();

            Debug.Log($"🎯 AR Session Found: {arSession != null}");
            Debug.Log($"🎯 XR Origin Found: {xrOrigin != null}");
            Debug.Log($"🎯 AR Camera Manager Found: {cameraManager != null}");

            if (cameraManager != null)
            {
                var background = cameraManager.GetComponent<UnityEngine.XR.ARFoundation.ARCameraBackground>();
                Debug.Log($"🎯 AR Camera Background Found: {background != null}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"🎯 AR Test Error: {e.Message}");
        }

        Debug.Log("🎯 StartupTest: Initialization complete!");
    }

    void Update()
    {
        // Log every 5 seconds to show app is running
        if (Time.frameCount % 300 == 0) // Every 5 seconds at 60fps
        {
            Debug.Log($"🎯 App running - Frame: {Time.frameCount}, Time: {Time.time:F1}s");
        }
    }
}
