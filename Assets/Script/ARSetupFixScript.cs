using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ARSetupFixScript : MonoBehaviour
{
    [Header("Auto-Fix AR Setup Issues")]
    [SerializeField] private bool autoFixOnStart = true;
    
    void Start()
    {
        if (autoFixOnStart)
        {
            FixAllARIssues();
        }
    }
    
    [ContextMenu("Fix All AR Issues")]
    public void FixAllARIssues()
    {
        Debug.Log("Starting AR Setup Fix...");
        
        // 1. Fix XR Origin Camera Assignment
        FixXROriginCameraAssignment();
        
        // 2. Add Tracked Pose Driver to Main Camera
        AddTrackedPoseDriverToMainCamera();
        
        // 3. Ensure AR Camera Background is properly set
        ConfigureARCameraBackground();
        
        // 4. Remove duplicate AR Sessions
        RemoveDuplicateARSessions();
        
        // 5. Verify all AR components are properly connected
        VerifyARComponentConnections();
        
        Debug.Log("AR Setup Fix Complete!");
    }
    
    void FixXROriginCameraAssignment()
    {
        Debug.Log("Fixing XR Origin Camera Assignment...");
        
        XROrigin xrOrigin = FindObjectOfType<XROrigin>();
        if (xrOrigin == null)
        {
            Debug.LogError("XR Origin not found!");
            return;
        }
        
        // Find the Main Camera within XR Origin hierarchy
        Camera mainCamera = null;
        
        // Check direct children first
        foreach (Transform child in xrOrigin.transform)
        {
            if (child.name.Contains("Camera Offset"))
            {
                foreach (Transform grandChild in child)
                {
                    Camera cam = grandChild.GetComponent<Camera>();
                    if (cam != null && grandChild.name.Contains("Main Camera"))
                    {
                        mainCamera = cam;
                        break;
                    }
                }
                if (mainCamera != null) break;
            }
        }
        
        // If not found, search more broadly
        if (mainCamera == null)
        {
            Camera[] allCameras = xrOrigin.GetComponentsInChildren<Camera>();
            foreach (Camera cam in allCameras)
            {
                if (cam.gameObject.name.Contains("Main Camera") || cam.gameObject.name.Contains("ARCamera"))
                {
                    mainCamera = cam;
                    break;
                }
            }
        }
        
        if (mainCamera != null)
        {
            // Use reflection to access private Camera field since it might not be public
            var field = typeof(XROrigin).GetField("m_Camera", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
            {
                field.SetValue(xrOrigin, mainCamera);
                Debug.Log($"XR Origin Camera assigned to: {mainCamera.name}");
            }
            else
            {
                // Try public property if available
                var property = typeof(XROrigin).GetProperty("Camera");
                if (property != null && property.CanWrite)
                {
                    property.SetValue(xrOrigin, mainCamera);
                    Debug.Log($"XR Origin Camera assigned to: {mainCamera.name} (via property)");
                }
            }
        }
        else
        {
            Debug.LogWarning("Main Camera not found in XR Origin hierarchy");
        }
    }
    
    void AddTrackedPoseDriverToMainCamera()
    {
        Debug.Log("Adding AR Pose Driver to Main Camera...");
        
        XROrigin xrOrigin = FindObjectOfType<XROrigin>();
        if (xrOrigin == null) return;
        
        Camera[] cameras = xrOrigin.GetComponentsInChildren<Camera>();
        foreach (Camera cam in cameras)
        {
            if (cam.gameObject.name.Contains("Main Camera") || cam.gameObject.name.Contains("ARCamera"))
            {
                // Instead of using TrackedPoseDriver, we'll use the ARPoseDriver component
                // which is part of AR Foundation and doesn't require the Input System package
                var existingPoseDriver = cam.GetComponent<ARPoseDriver>();
                if (existingPoseDriver == null)
                {
                    // Add the AR Pose Driver component
                    cam.gameObject.AddComponent<ARPoseDriver>();
                    Debug.Log($"Added AR Pose Driver to: {cam.name}");
                }
                else
                {
                    Debug.Log($"AR Pose Driver already exists on: {cam.name}");
                }
                break;
            }
        }
    }
    
    void ConfigureARCameraBackground()
    {
        Debug.Log("Configuring AR Camera Background...");
        
        ARCameraBackground arCameraBackground = FindObjectOfType<ARCameraBackground>();
        if (arCameraBackground == null)
        {
            Debug.LogWarning("AR Camera Background component not found!");
            return;
        }
        
        ARCameraManager cameraManager = FindObjectOfType<ARCameraManager>();
        if (cameraManager != null)
        {
            // Ensure camera manager is properly configured
            cameraManager.enabled = true;
            Debug.Log("AR Camera Manager enabled");
        }
        
        // Ensure the AR Camera Background is enabled and properly configured
        arCameraBackground.enabled = true;
        
        // Get the camera component
        Camera cam = arCameraBackground.GetComponent<Camera>();
        if (cam != null)
        {
            // Configure camera for AR
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 1000f;
            
            Debug.Log($"AR Camera Background configured on: {cam.name}");
        }
    }
    
    void RemoveDuplicateARSessions()
    {
        Debug.Log("Removing duplicate AR Sessions...");
        
        ARSession[] arSessions = FindObjectsOfType<ARSession>();
        if (arSessions.Length > 1)
        {
            Debug.LogWarning($"Found {arSessions.Length} AR Sessions. Keeping only the first one.");
            
            for (int i = 1; i < arSessions.Length; i++)
            {
                if (arSessions[i] != null)
                {
                    Debug.Log($"Removing duplicate AR Session from: {arSessions[i].name}");
                    DestroyImmediate(arSessions[i]);
                }
            }
        }
        else if (arSessions.Length == 1)
        {
            Debug.Log("Single AR Session found - OK");
        }
        else
        {
            Debug.LogError("No AR Session found!");
        }
    }
    
    void VerifyARComponentConnections()
    {
        Debug.Log("Verifying AR Component Connections...");
        
        // Find and verify AR Tattoo Manager connections
        ARTattooManager tattooManager = FindObjectOfType<ARTattooManager>();
        if (tattooManager != null)
        {
            ARRaycastManager raycastManager = FindObjectOfType<ARRaycastManager>();
            ARPlaneManager planeManager = FindObjectOfType<ARPlaneManager>();
            
            // Use reflection to check and set private fields if needed
            var raycastField = typeof(ARTattooManager).GetField("arRaycastManager", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (raycastField != null && raycastField.GetValue(tattooManager) == null && raycastManager != null)
            {
                raycastField.SetValue(tattooManager, raycastManager);
                Debug.Log("Connected AR Raycast Manager to AR Tattoo Manager");
            }
            
            var planeField = typeof(ARTattooManager).GetField("arPlaneManager", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (planeField != null && planeField.GetValue(tattooManager) == null && planeManager != null)
            {
                planeField.SetValue(tattooManager, planeManager);
                Debug.Log("Connected AR Plane Manager to AR Tattoo Manager");
            }
        }
        
        // Verify SkinScanner connections
        SkinScanner skinScanner = FindObjectOfType<SkinScanner>();
        if (skinScanner != null)
        {
            ARCameraManager cameraManager = FindObjectOfType<ARCameraManager>();
            ARRaycastManager raycastManager = FindObjectOfType<ARRaycastManager>();
            
            var cameraField = typeof(SkinScanner).GetField("cameraManager", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (cameraField != null && cameraField.GetValue(skinScanner) == null && cameraManager != null)
            {
                cameraField.SetValue(skinScanner, cameraManager);
                Debug.Log("Connected AR Camera Manager to Skin Scanner");
            }
            
            var raycastField = typeof(SkinScanner).GetField("arRaycastManager", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (raycastField != null && raycastField.GetValue(skinScanner) == null && raycastManager != null)
            {
                raycastField.SetValue(skinScanner, raycastManager);
                Debug.Log("Connected AR Raycast Manager to Skin Scanner");
            }
            
            var tattooField = typeof(SkinScanner).GetField("tattooManager", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (tattooField != null && tattooField.GetValue(skinScanner) == null && tattooManager != null)
            {
                tattooField.SetValue(skinScanner, tattooManager);
                Debug.Log("Connected AR Tattoo Manager to Skin Scanner");
            }
        }
        
        Debug.Log("AR Component Connections Verified");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ARSetupFixScript))]
public class ARSetupFixScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        ARSetupFixScript script = (ARSetupFixScript)target;
        
        if (GUILayout.Button("Fix All AR Issues"))
        {
            script.FixAllARIssues();
        }
    }
}
#endif
