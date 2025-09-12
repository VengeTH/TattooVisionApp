using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Manual AR Setup Helper
/// To use: 
/// 1. Create an empty GameObject in your scene
/// 2. Add this script to it
/// 3. Click "Setup AR Scene" button in Inspector
/// 4. Delete this GameObject after setup
/// </summary>
public class ManualARSetup : MonoBehaviour
{
    [Header("Click the button below to setup AR")]
    [Space(20)]
    public bool setupComplete = false;
    
    // This method will be called from a custom inspector button
    public void SetupARScene()
    {
        Debug.Log("Starting AR Scene Setup...");
        
        // Create AR Session
        if (GameObject.Find("AR Session") == null)
        {
            GameObject arSession = new GameObject("AR Session");
            // Add AR Session component only in editor
            var sessionType = Type.GetType("UnityEngine.XR.ARFoundation.ARSession, Unity.XR.ARFoundation");
            if (sessionType != null)
                arSession.AddComponent(sessionType);
            Debug.Log("Created AR Session");
        }
        
        // Create XR Origin
        GameObject xrOrigin = GameObject.Find("XR Origin");
        if (xrOrigin == null)
        {
            xrOrigin = new GameObject("XR Origin");
            // Add XROrigin component
            var xrOriginType = Type.GetType("Unity.XR.CoreUtils.XROrigin, Unity.XR.CoreUtils");
            Component xrOriginComponent = null;
            if (xrOriginType != null)
                xrOriginComponent = xrOrigin.AddComponent(xrOriginType);
            
            // Create Camera Offset
            GameObject cameraOffset = new GameObject("Camera Offset");
            cameraOffset.transform.SetParent(xrOrigin.transform);
            cameraOffset.transform.localPosition = Vector3.zero;
            
            // Create Main Camera
            GameObject mainCamera = GameObject.Find("Main Camera");
            if (mainCamera != null && mainCamera.transform.parent == null)
            {
                // Use existing Main Camera
                mainCamera.transform.SetParent(cameraOffset.transform);
            }
            else
            {
                // Create new Main Camera
                mainCamera = new GameObject("Main Camera");
                mainCamera.transform.SetParent(cameraOffset.transform);
                mainCamera.AddComponent<Camera>();
            }
            
            mainCamera.transform.localPosition = new Vector3(0, 1.36f, 0);
            mainCamera.tag = "MainCamera";
            
            // Add AR Camera components
            var arCameraManagerType = Type.GetType("UnityEngine.XR.ARFoundation.ARCameraManager, Unity.XR.ARFoundation");
            if (arCameraManagerType != null)
                mainCamera.AddComponent(arCameraManagerType);
            
            var arCameraBackgroundType = Type.GetType("UnityEngine.XR.ARFoundation.ARCameraBackground, Unity.XR.ARFoundation");
            if (arCameraBackgroundType != null)
                mainCamera.AddComponent(arCameraBackgroundType);
            
            // Set XR Origin camera reference
            if (xrOriginComponent != null)
            {
                var cameraProperty = xrOriginComponent.GetType().GetProperty("Camera");
                if (cameraProperty != null)
                    cameraProperty.SetValue(xrOriginComponent, mainCamera.GetComponent<Camera>());
            }
            
            // Add AR managers to XR Origin
            var arRaycastManagerType = Type.GetType("UnityEngine.XR.ARFoundation.ARRaycastManager, Unity.XR.ARFoundation");
            if (arRaycastManagerType != null)
                xrOrigin.AddComponent(arRaycastManagerType);
            
            var arPlaneManagerType = Type.GetType("UnityEngine.XR.ARFoundation.ARPlaneManager, Unity.XR.ARFoundation");
            if (arPlaneManagerType != null)
                xrOrigin.AddComponent(arPlaneManagerType);
            
            Debug.Log("Created XR Origin with Camera");
        }
        
        // Create Managers
        GameObject managers = GameObject.Find("Managers");
        if (managers == null)
        {
            managers = new GameObject("Managers");
        }
        
        // Create manager objects
        if (GameObject.Find("AR Tattoo Manager") == null)
        {
            GameObject tattooManager = new GameObject("AR Tattoo Manager");
            tattooManager.transform.SetParent(managers.transform);
            Debug.Log("Created AR Tattoo Manager - Add ARTattooManager script");
        }
        
        if (GameObject.Find("AR Camera UI Manager") == null)
        {
            GameObject uiManager = new GameObject("AR Camera UI Manager");
            uiManager.transform.SetParent(managers.transform);
            Debug.Log("Created AR Camera UI Manager - Add ARCameraUIManager script");
        }
        
        if (GameObject.Find("Skin Scanner") == null)
        {
            GameObject scanner = new GameObject("Skin Scanner");
            scanner.transform.SetParent(managers.transform);
            Debug.Log("Created Skin Scanner - Add SkinScanner script");
        }
        
        setupComplete = true;
        Debug.Log("=== AR SCENE SETUP COMPLETE ===");
        Debug.Log("Now manually add these scripts to the manager objects:");
        Debug.Log("1. AR Tattoo Manager → Add ARTattooManager.cs");
        Debug.Log("2. AR Camera UI Manager → Add ARCameraUIManager.cs");
        Debug.Log("3. Skin Scanner → Add SkinScanner.cs");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ManualARSetup))]
public class ManualARSetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        ManualARSetup setup = (ManualARSetup)target;
        
        GUILayout.Space(20);
        
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("SETUP AR SCENE", GUILayout.Height(50)))
        {
            setup.SetupARScene();
        }
        GUI.backgroundColor = Color.white;
        
        if (setup.setupComplete)
        {
            GUILayout.Space(10);
            EditorGUILayout.HelpBox("Setup Complete! Now add the scripts to manager objects.", MessageType.Info);
        }
    }
}
#endif
