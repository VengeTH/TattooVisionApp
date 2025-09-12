using UnityEngine;
using UnityEditor;

public class SimpleARSetup
{
    [MenuItem("GameObject/AR Setup/Create Complete AR Scene", false, 10)]
    public static void SetupARScene()
    {
        Debug.Log("Setting up AR Scene...");
        
        // This will show up when you right-click in Hierarchy
        // Go to GameObject > AR Setup > Create Complete AR Scene
        
        CreateARSession();
        CreateXROrigin();
        CreateManagers();
        
        Debug.Log("AR Scene setup complete!");
        EditorUtility.DisplayDialog("Success", "AR Scene setup complete!", "OK");
    }
    
    static void CreateARSession()
    {
        if (GameObject.Find("AR Session") == null)
        {
            GameObject arSession = new GameObject("AR Session");
            arSession.AddComponent<UnityEngine.XR.ARFoundation.ARSession>();
            Debug.Log("Created AR Session");
        }
    }
    
    static void CreateXROrigin()
    {
        if (GameObject.Find("XR Origin") == null)
        {
            GameObject xrOrigin = new GameObject("XR Origin");
            xrOrigin.AddComponent<Unity.XR.CoreUtils.XROrigin>();
            
            // Create Camera Offset
            GameObject cameraOffset = new GameObject("Camera Offset");
            cameraOffset.transform.SetParent(xrOrigin.transform);
            cameraOffset.transform.localPosition = Vector3.zero;
            
            // Create Main Camera
            GameObject mainCamera = new GameObject("Main Camera");
            mainCamera.transform.SetParent(cameraOffset.transform);
            mainCamera.transform.localPosition = new Vector3(0, 1.36f, 0);
            
            Camera cam = mainCamera.AddComponent<Camera>();
            cam.tag = "MainCamera";
            mainCamera.AddComponent<UnityEngine.XR.ARFoundation.ARCameraManager>();
            mainCamera.AddComponent<UnityEngine.XR.ARFoundation.ARCameraBackground>();
            
            // Set XR Origin camera reference
            xrOrigin.GetComponent<Unity.XR.CoreUtils.XROrigin>().Camera = cam;
            
            // Add managers to XR Origin
            xrOrigin.AddComponent<UnityEngine.XR.ARFoundation.ARRaycastManager>();
            xrOrigin.AddComponent<UnityEngine.XR.ARFoundation.ARPlaneManager>();
            
            Debug.Log("Created XR Origin with camera");
        }
    }
    
    static void CreateManagers()
    {
        GameObject managers = GameObject.Find("Managers");
        if (managers == null)
        {
            managers = new GameObject("Managers");
        }
        
        // Create basic manager objects - scripts will be added manually
        if (GameObject.Find("AR Tattoo Manager") == null)
        {
            GameObject tattooManager = new GameObject("AR Tattoo Manager");
            tattooManager.transform.SetParent(managers.transform);
            Debug.Log("Created AR Tattoo Manager - Add ARTattooManager script manually");
        }
        
        if (GameObject.Find("AR Camera UI Manager") == null)
        {
            GameObject uiManager = new GameObject("AR Camera UI Manager");
            uiManager.transform.SetParent(managers.transform);
            Debug.Log("Created AR Camera UI Manager - Add ARCameraUIManager script manually");
        }
        
        if (GameObject.Find("Skin Scanner") == null)
        {
            GameObject scanner = new GameObject("Skin Scanner");
            scanner.transform.SetParent(managers.transform);
            Debug.Log("Created Skin Scanner - Add SkinScanner script manually");
        }
    }
}
