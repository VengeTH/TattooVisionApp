using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Simple AR Setup - Works without ARCore package
/// This creates the basic AR structure needed for development
/// </summary>
public class SimpleARSetupNoPackages : MonoBehaviour
{
    [Header("Click Setup Button in Inspector")]
    public bool setupComplete = false;
    
    public void RunSetup()
    {
        Debug.Log("=== Starting AR Setup (No Package Dependencies) ===");
        
        try
        {
            // Step 1: Create AR Session GameObject
            GameObject arSession = GameObject.Find("AR Session");
            if (arSession == null)
            {
                arSession = new GameObject("AR Session");
                Debug.Log("✓ Created AR Session GameObject");
            }
            else
            {
                Debug.Log("✓ AR Session already exists");
            }
            
            // Step 2: Create XR Origin
            GameObject xrOrigin = GameObject.Find("XR Origin");
            if (xrOrigin == null)
            {
                xrOrigin = new GameObject("XR Origin");
                Debug.Log("✓ Created XR Origin GameObject");
            }
            else
            {
                Debug.Log("✓ XR Origin already exists");
            }
            
            // Step 3: Create Camera Offset
            Transform cameraOffset = xrOrigin.transform.Find("Camera Offset");
            if (cameraOffset == null)
            {
                GameObject offsetObj = new GameObject("Camera Offset");
                offsetObj.transform.SetParent(xrOrigin.transform);
                offsetObj.transform.localPosition = Vector3.zero;
                cameraOffset = offsetObj.transform;
                Debug.Log("✓ Created Camera Offset");
            }
            
            // Step 4: Setup Main Camera
            GameObject mainCamera = GameObject.FindWithTag("MainCamera");
            if (mainCamera == null)
            {
                mainCamera = GameObject.Find("Main Camera");
            }
            
            if (mainCamera != null)
            {
                // Move existing camera
                if (mainCamera.transform.parent != cameraOffset)
                {
                    mainCamera.transform.SetParent(cameraOffset);
                    mainCamera.transform.localPosition = new Vector3(0, 1.36f, 0);
                    mainCamera.transform.localRotation = Quaternion.identity;
                    Debug.Log("✓ Positioned Main Camera under Camera Offset");
                }
            }
            else
            {
                // Create new camera
                mainCamera = new GameObject("Main Camera");
                mainCamera.transform.SetParent(cameraOffset);
                mainCamera.transform.localPosition = new Vector3(0, 1.36f, 0);
                mainCamera.tag = "MainCamera";
                mainCamera.AddComponent<Camera>();
                Debug.Log("✓ Created new Main Camera");
            }
            
            // Step 5: Create Managers container
            GameObject managers = GameObject.Find("Managers");
            if (managers == null)
            {
                managers = new GameObject("Managers");
                Debug.Log("✓ Created Managers container");
            }
            
            // Step 6: Create AR Tattoo Manager
            GameObject tattooManager = GameObject.Find("AR Tattoo Manager");
            if (tattooManager == null)
            {
                tattooManager = new GameObject("AR Tattoo Manager");
                tattooManager.transform.SetParent(managers.transform);
                Debug.Log("✓ Created AR Tattoo Manager");
            }
            
            // Step 7: Create AR Camera UI Manager
            GameObject uiManager = GameObject.Find("AR Camera UI Manager");
            if (uiManager == null)
            {
                uiManager = new GameObject("AR Camera UI Manager");
                uiManager.transform.SetParent(managers.transform);
                Debug.Log("✓ Created AR Camera UI Manager");
            }
            
            // Step 8: Create Skin Scanner
            GameObject skinScanner = GameObject.Find("Skin Scanner");
            if (skinScanner == null)
            {
                skinScanner = new GameObject("Skin Scanner");
                skinScanner.transform.SetParent(managers.transform);
                Debug.Log("✓ Created Skin Scanner");
            }
            
            // Step 9: Create UI Canvas
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                canvas = new GameObject("Canvas");
                canvas.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
                Debug.Log("✓ Created UI Canvas");
            }
            
            // Step 10: Create EventSystem
            GameObject eventSystem = GameObject.Find("EventSystem");
            if (eventSystem == null)
            {
                eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
                
                // Try to add InputSystemUIInputModule
                var inputModuleType = Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
                if (inputModuleType != null)
                {
                    eventSystem.AddComponent(inputModuleType);
                    Debug.Log("✓ Created EventSystem with Input System module");
                }
                else
                {
                    // Fallback to standard input module
                    eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                    Debug.Log("✓ Created EventSystem with standard input module");
                }
            }
            
            setupComplete = true;
            
            Debug.Log("========================================");
            Debug.Log("AR SCENE SETUP COMPLETE!");
            Debug.Log("========================================");
            Debug.Log("Next steps:");
            Debug.Log("1. When packages compile, add AR components:");
            Debug.Log("   - AR Session: Add 'ARSession' component");
            Debug.Log("   - XR Origin: Add 'XROrigin', 'ARRaycastManager', 'ARPlaneManager'");
            Debug.Log("   - Main Camera: Add 'ARCameraManager', 'ARCameraBackground'");
            Debug.Log("");
            Debug.Log("2. Add manager scripts:");
            Debug.Log("   - AR Tattoo Manager: Add 'ARTattooManager' script");
            Debug.Log("   - AR Camera UI Manager: Add 'ARCameraUIManager' script");
            Debug.Log("   - Skin Scanner: Add 'SkinScanner' script");
            Debug.Log("========================================");
        }
        catch (Exception e)
        {
            Debug.LogError($"Setup failed: {e.Message}");
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(SimpleARSetupNoPackages))]
public class SimpleARSetupNoPackagesEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        SimpleARSetupNoPackages setup = (SimpleARSetupNoPackages)target;
        
        EditorGUILayout.Space(20);
        
        EditorGUILayout.HelpBox("This will create the AR scene structure WITHOUT requiring ARCore package.", MessageType.Info);
        
        EditorGUILayout.Space(10);
        
        GUI.backgroundColor = new Color(0.3f, 0.8f, 0.3f);
        if (GUILayout.Button("SETUP AR SCENE", GUILayout.Height(50)))
        {
            setup.RunSetup();
        }
        GUI.backgroundColor = Color.white;
        
        if (setup.setupComplete)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("✓ Setup Complete! Check Console for next steps.", MessageType.Info);
        }
        
        EditorGUILayout.Space(10);
        
        if (GUILayout.Button("Open Console", GUILayout.Height(30)))
        {
            EditorApplication.ExecuteMenuItem("Window/General/Console");
        }
    }
}
#endif
