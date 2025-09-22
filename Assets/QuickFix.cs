using UnityEngine;

/// <summary>
/// Quick fix for the black screen issue - add this to ANY GameObject in your scene
/// </summary>
public class QuickFix : MonoBehaviour
{
    void Start()
    {
        Debug.Log("🔧 QUICK FIX: App started successfully!");
        Debug.Log($"🔧 Device: {SystemInfo.deviceModel}");
        Debug.Log($"🔧 Unity: {Application.unityVersion}");

        // Check camera permission
        Debug.Log($"🔧 Camera Permission: {Application.HasUserAuthorization(UserAuthorization.WebCam)}");

        // Check AR components
        var arSession = FindObjectOfType<UnityEngine.XR.ARFoundation.ARSession>();
        var xrOrigin = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();
        var cameraManager = FindObjectOfType<UnityEngine.XR.ARFoundation.ARCameraManager>();

        Debug.Log($"🔧 AR Session: {(arSession != null ? "FOUND" : "NOT FOUND")}");
        Debug.Log($"🔧 XR Origin: {(xrOrigin != null ? "FOUND" : "NOT FOUND")}");
        Debug.Log($"🔧 Camera Manager: {(cameraManager != null ? "FOUND" : "NOT FOUND")}");

        if (cameraManager != null)
        {
            var background = cameraManager.GetComponent<UnityEngine.XR.ARFoundation.ARCameraBackground>();
            Debug.Log($"🔧 Camera Background: {(background != null ? "FOUND" : "NOT FOUND")}");

            if (background == null)
            {
                Debug.Log("🔧 FIXING: Adding ARCameraBackground component!");
                background = cameraManager.gameObject.AddComponent<UnityEngine.XR.ARFoundation.ARCameraBackground>();
                Debug.Log($"🔧 FIXED: ARCameraBackground added: {background != null}");
            }
        }

        // Force enable AR components
        if (arSession != null && !arSession.enabled)
        {
            arSession.enabled = true;
            Debug.Log("🔧 AR Session enabled");
        }

        if (cameraManager != null && !cameraManager.enabled)
        {
            cameraManager.enabled = true;
            Debug.Log("🔧 Camera Manager enabled");
        }

        Debug.Log("🔧 Quick fix applied! If you see this, the app is working.");
        Debug.Log("🔧 Check if camera feed appears now.");
    }
}
