using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class ARSetupCopier : MonoBehaviour
{
    [Header("AR Setup Copy Tool")]
    [SerializeField] private string targetSceneName = "Dashboard";
    
    [ContextMenu("Copy AR Setup to Dashboard Scene")]
    public void CopyARSetupToDashboardScene()
    {
        #if UNITY_EDITOR
        // Save current scene first
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        
        // Get reference to current AR setup
        XROrigin currentXROrigin = FindObjectOfType<XROrigin>();
        ARSession currentARSession = FindObjectOfType<ARSession>();
        
        if (currentXROrigin == null)
        {
            Debug.LogError("No XR Origin found in current scene!");
            return;
        }
        
        if (currentARSession == null)
        {
            Debug.LogError("No AR Session found in current scene!");
            return;
        }
        
        // Find Dashboard scene
        string dashboardScenePath = null;
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        
        foreach (string guid in sceneGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
            
            if (sceneName.Equals(targetSceneName, System.StringComparison.OrdinalIgnoreCase))
            {
                dashboardScenePath = path;
                break;
            }
        }
        
        if (string.IsNullOrEmpty(dashboardScenePath))
        {
            Debug.LogError($"Scene '{targetSceneName}' not found!");
            return;
        }
        
        // Open Dashboard scene additively to copy components
        Scene currentScene = SceneManager.GetActiveScene();
        Scene dashboardScene = EditorSceneManager.OpenScene(dashboardScenePath, OpenSceneMode.Additive);
        
        Debug.Log($"Copying AR setup to {targetSceneName} scene...");
        
        // Create AR setup in Dashboard scene
        CreateARSetupInScene(dashboardScene);
        
        // Save Dashboard scene
        EditorSceneManager.SaveScene(dashboardScene);
        
        // Close Dashboard scene (keep current scene open)
        EditorSceneManager.CloseScene(dashboardScene, false);
        
        Debug.Log("AR setup successfully copied to Dashboard scene!");
        
        #else
        Debug.LogWarning("This function only works in the Unity Editor!");
        #endif
    }
    
    #if UNITY_EDITOR
    private void CreateARSetupInScene(Scene targetScene)
    {
        // Set Dashboard scene as active temporarily for creating objects
        Scene originalScene = SceneManager.GetActiveScene();
        SceneManager.SetActiveScene(targetScene);
        
        // Check if AR setup already exists
        GameObject[] rootObjects = targetScene.GetRootGameObjects();
        bool hasXROrigin = false;
        bool hasARSession = false;
        
        foreach (GameObject obj in rootObjects)
        {
            if (obj.GetComponentInChildren<XROrigin>() != null) hasXROrigin = true;
            if (obj.GetComponentInChildren<ARSession>() != null) hasARSession = true;
        }
        
        // Create AR Session if it doesn't exist
        if (!hasARSession)
        {
            GameObject arSessionObj = new GameObject("AR Session");
            arSessionObj.AddComponent<ARSession>();
            arSessionObj.AddComponent<ARInputManager>();
            Debug.Log("Created AR Session in Dashboard scene");
        }
        
        // Create XR Origin if it doesn't exist
        if (!hasXROrigin)
        {
            // Create XR Origin hierarchy
            GameObject xrOriginObj = new GameObject("XR Origin");
            XROrigin xrOrigin = xrOriginObj.AddComponent<XROrigin>();
            xrOriginObj.AddComponent<ARRaycastManager>();
            xrOriginObj.AddComponent<ARPlaneManager>();
            
            // Create Camera Offset
            GameObject cameraOffset = new GameObject("Camera Offset");
            cameraOffset.transform.SetParent(xrOriginObj.transform);
            cameraOffset.transform.localPosition = Vector3.zero;
            cameraOffset.transform.localRotation = Quaternion.identity;
            
            // Create Main Camera
            GameObject mainCameraObj = new GameObject("Main Camera");
            mainCameraObj.transform.SetParent(cameraOffset.transform);
            mainCameraObj.transform.localPosition = Vector3.zero;
            mainCameraObj.transform.localRotation = Quaternion.identity;
            mainCameraObj.tag = "MainCamera";
            
            // Add camera components
            Camera cam = mainCameraObj.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 1000f;
            
            // Add AR components to camera
            mainCameraObj.AddComponent<ARCameraManager>();
            mainCameraObj.AddComponent<ARCameraBackground>();
            mainCameraObj.AddComponent<ARPoseDriver>();
            
            // Assign camera to XR Origin
            var cameraProperty = typeof(XROrigin).GetProperty("Camera");
            if (cameraProperty != null && cameraProperty.CanWrite)
            {
                cameraProperty.SetValue(xrOrigin, cam);
            }
            
            Debug.Log("Created XR Origin with camera hierarchy in Dashboard scene");
        }
        
        // Create Managers container if it doesn't exist
        GameObject managersObj = GameObject.Find("Managers");
        if (managersObj == null)
        {
            managersObj = new GameObject("Managers");
            
            // Add manager components
            managersObj.AddComponent<ARTattooManager>();
            managersObj.AddComponent<ARCameraUIManager>();
            managersObj.AddComponent<SkinScanner>();
            
            Debug.Log("Created Managers container with AR scripts in Dashboard scene");
        }
        
        // Add EventSystem if it doesn't exist
        GameObject eventSystemObj = GameObject.Find("EventSystem");
        if (eventSystemObj == null)
        {
            eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            Debug.Log("Created EventSystem in Dashboard scene");
        }
        
        // Add AR Setup Fix Script
        GameObject arSetupFixObj = new GameObject("ARSetupFixScript");
        ARSetupFixScript setupScript = arSetupFixObj.AddComponent<ARSetupFixScript>();
        // Disable auto-fix on start since we're setting up manually
        var autoFixField = typeof(ARSetupFixScript).GetField("autoFixOnStart", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (autoFixField != null)
        {
            autoFixField.SetValue(setupScript, false);
        }
        
        Debug.Log("Added AR Setup Fix Script to Dashboard scene");
        
        // Restore original active scene
        SceneManager.SetActiveScene(originalScene);
    }
    #endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(ARSetupCopier))]
public class ARSetupCopierEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        ARSetupCopier copier = (ARSetupCopier)target;
        
        if (GUILayout.Button("Copy AR Setup to Dashboard Scene"))
        {
            copier.CopyARSetupToDashboardScene();
        }
        
        EditorGUILayout.HelpBox(
            "This will copy the current AR setup to the Dashboard scene. " +
            "Make sure the Dashboard scene exists in your project.", 
            MessageType.Info);
    }
}
#endif
