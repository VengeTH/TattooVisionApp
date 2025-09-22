using UnityEngine;

/// <summary>
/// Automatically adds startup logging to any scene - drag this onto any GameObject
/// </summary>
public class AutoStartupLogger : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("🚀 AutoStartupLogger: App is AWAKE!");
        Debug.Log($"📱 Device: {SystemInfo.deviceModel}");
        Debug.Log($"🔧 Unity Version: {Application.unityVersion}");
        Debug.Log($"🎮 Platform: {Application.platform}");
    }

    void Start()
    {
        Debug.Log("✅ AutoStartupLogger: App has STARTED!");
        Debug.Log($"📊 Graphics: {SystemInfo.graphicsDeviceName}");
        Debug.Log($"📊 Graphics API: {SystemInfo.graphicsDeviceType}");
        Debug.Log($"📊 OS: {SystemInfo.operatingSystem}");

        // Check camera permission
        Debug.Log($"📷 Camera Permission: {Application.HasUserAuthorization(UserAuthorization.WebCam)}");

        // Check AR components
        CheckARComponents();

        // Check if we're in the right scene
        Debug.Log($"🎬 Current Scene: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");

        Debug.Log("🎯 If you see this, the app is running! Check for AR issues next.");
    }

    void CheckARComponents()
    {
        try
        {
            var arSession = FindObjectOfType<UnityEngine.XR.ARFoundation.ARSession>();
            var xrOrigin = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();
            var cameraManager = FindObjectOfType<UnityEngine.XR.ARFoundation.ARCameraManager>();

            Debug.Log($"🔍 AR Session: {(arSession != null ? "FOUND" : "NOT FOUND")}");
            Debug.Log($"🔍 XR Origin: {(xrOrigin != null ? "FOUND" : "NOT FOUND")}");
            Debug.Log($"🔍 AR Camera Manager: {(cameraManager != null ? "FOUND" : "NOT FOUND")}");

            if (cameraManager != null)
            {
                var background = cameraManager.GetComponent<UnityEngine.XR.ARFoundation.ARCameraBackground>();
                Debug.Log($"🔍 AR Camera Background: {(background != null ? "FOUND" : "NOT FOUND")}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ AR Check Error: {e.Message}");
        }
    }

    void Update()
    {
        // Log every 10 seconds to prove app is running
        if (Time.frameCount % 600 == 0) // Every 10 seconds at 60fps
        {
            Debug.Log($"⏰ App still running - {Time.time:F1}s elapsed");
        }
    }
}
