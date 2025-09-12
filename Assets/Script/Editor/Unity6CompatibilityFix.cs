using UnityEngine;
using UnityEditor;

// This file provides a workaround for Unity 6 compatibility issues
// It defines the missing AndroidExternalToolsSettings to prevent compilation errors

namespace UnityEditor.Android
{
    // Dummy class to prevent compilation errors in Unity 6
    // The actual AndroidExternalToolsSettings was removed in Unity 6
    public static class AndroidExternalToolsSettings
    {
        public static string sdkRootPath 
        { 
            get 
            { 
                // Return Android SDK path from Unity preferences
                return EditorPrefs.GetString("AndroidSdkRoot", "");
            }
            set { }
        }
        
        public static string jdkRootPath 
        { 
            get 
            {
                // Return JDK path from Unity preferences
                return EditorPrefs.GetString("JdkPath", "");
            }
            set { }
        }
        
        public static string ndkRootPath 
        { 
            get 
            {
                return EditorPrefs.GetString("AndroidNdkRoot", "");
            }
            set { }
        }
        
        public static string gradlePath 
        { 
            get 
            {
                return "";
            }
            set { }
        }
    }
}

// Auto-setup on compile
[InitializeOnLoad]
public class Unity6ARSetupHelper
{
    static Unity6ARSetupHelper()
    {
        EditorApplication.delayCall += () =>
        {
            if (!SessionState.GetBool("Unity6ARSetupShown", false))
            {
                SessionState.SetBool("Unity6ARSetupShown", true);
                Debug.Log("Unity 6 AR Setup Ready! Right-click in Hierarchy > GameObject > AR Setup > Create Complete AR Scene");
                Debug.Log("Or use Window > General > Console, then click this message for instructions.");
            }
        };
    }
}

public class QuickARSetup : EditorWindow
{
    [MenuItem("Window/AR Quick Setup")]
    public static void ShowWindow()
    {
        GetWindow<QuickARSetup>("AR Quick Setup");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Quick AR Scene Setup", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Setup AR Scene", GUILayout.Height(40)))
        {
            SetupARScene();
        }
        
        GUILayout.Space(10);
        GUILayout.Label("This will create:", EditorStyles.label);
        GUILayout.Label("• AR Session", EditorStyles.label);
        GUILayout.Label("• XR Origin with Camera", EditorStyles.label);
        GUILayout.Label("• Manager GameObjects", EditorStyles.label);
    }
    
    static void SetupARScene()
    {
        // Create AR Session
        if (GameObject.Find("AR Session") == null)
        {
            new GameObject("AR Session").AddComponent<UnityEngine.XR.ARFoundation.ARSession>();
        }
        
        // Create XR Origin
        GameObject xrOrigin = GameObject.Find("XR Origin");
        if (xrOrigin == null)
        {
            xrOrigin = new GameObject("XR Origin");
            xrOrigin.AddComponent<Unity.XR.CoreUtils.XROrigin>();
            
            GameObject cameraOffset = new GameObject("Camera Offset");
            cameraOffset.transform.SetParent(xrOrigin.transform);
            
            GameObject mainCamera = new GameObject("Main Camera");
            mainCamera.transform.SetParent(cameraOffset.transform);
            mainCamera.transform.localPosition = new Vector3(0, 1.36f, 0);
            
            Camera cam = mainCamera.AddComponent<Camera>();
            cam.tag = "MainCamera";
            mainCamera.AddComponent<UnityEngine.XR.ARFoundation.ARCameraManager>();
            mainCamera.AddComponent<UnityEngine.XR.ARFoundation.ARCameraBackground>();
            
            xrOrigin.GetComponent<Unity.XR.CoreUtils.XROrigin>().Camera = cam;
            xrOrigin.AddComponent<UnityEngine.XR.ARFoundation.ARRaycastManager>();
            xrOrigin.AddComponent<UnityEngine.XR.ARFoundation.ARPlaneManager>();
        }
        
        // Create Managers
        GameObject managers = GameObject.Find("Managers");
        if (managers == null)
        {
            managers = new GameObject("Managers");
        }
        
        if (GameObject.Find("AR Tattoo Manager") == null)
        {
            GameObject tm = new GameObject("AR Tattoo Manager");
            tm.transform.SetParent(managers.transform);
        }
        
        if (GameObject.Find("AR Camera UI Manager") == null)
        {
            GameObject um = new GameObject("AR Camera UI Manager");
            um.transform.SetParent(managers.transform);
        }
        
        if (GameObject.Find("Skin Scanner") == null)
        {
            GameObject ss = new GameObject("Skin Scanner");
            ss.transform.SetParent(managers.transform);
        }
        
        Debug.Log("AR Scene Setup Complete! Now add the scripts to the manager objects.");
        EditorUtility.DisplayDialog("Success", "AR Scene created! Add scripts to manager objects:\n\n• AR Tattoo Manager → ARTattooManager.cs\n• AR Camera UI Manager → ARCameraUIManager.cs\n• Skin Scanner → SkinScanner.cs", "OK");
    }
}
