using UnityEngine;
using UnityEditor;
using UnityEngine.XR.ARFoundation;
using Unity.XR.CoreUtils;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ARSceneSetup : EditorWindow
{
    [MenuItem("TattooVision/Setup AR Scene")]
    public static void ShowWindow()
    {
        GetWindow<ARSceneSetup>("AR Scene Setup");
    }

    void OnGUI()
    {
        GUILayout.Label("AR Tattoo System Setup", EditorStyles.boldLabel);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Setup Complete AR Scene", GUILayout.Height(40)))
        {
            SetupARScene();
        }
        
        GUILayout.Space(10);
        
        GUILayout.Label("Individual Setup Options:", EditorStyles.boldLabel);
        
        if (GUILayout.Button("1. Create AR Session & XR Origin"))
        {
            CreateARFoundation();
        }
        
        if (GUILayout.Button("2. Create AR Managers"))
        {
            CreateARManagers();
        }
        
        if (GUILayout.Button("3. Create UI Canvas"))
        {
            CreateUI();
        }
        
        if (GUILayout.Button("4. Fix Camera Tags"))
        {
            FixCameraTags();
        }
        
        GUILayout.Space(10);
        GUILayout.Label("Troubleshooting:", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Remove Duplicate Cameras"))
        {
            RemoveDuplicateCameras();
        }
    }
    
    void SetupARScene()
    {
        CreateARFoundation();
        CreateARManagers();
        CreateUI();
        FixCameraTags();
        
        EditorUtility.DisplayDialog("Success", "AR Scene setup complete! Please check the Inspector to assign any missing references.", "OK");
    }
    
    void CreateARFoundation()
    {
        // Create AR Session
        GameObject arSession = GameObject.Find("AR Session");
        if (arSession == null)
        {
            arSession = new GameObject("AR Session");
            arSession.AddComponent<ARSession>();
            arSession.AddComponent<ARInputManager>();
        }
        
        // Create XR Origin (Unity 6 uses XROrigin instead of ARSessionOrigin)
        GameObject xrOrigin = GameObject.Find("XR Origin");
        if (xrOrigin == null)
        {
            // Check for old name
            GameObject oldOrigin = GameObject.Find("AR Session Origin");
            if (oldOrigin != null)
            {
                oldOrigin.name = "XR Origin";
                xrOrigin = oldOrigin;
            }
            else
            {
                xrOrigin = new GameObject("XR Origin");
            }
        }
        
        // Add XROrigin component if missing
        XROrigin xrOriginComponent = xrOrigin.GetComponent<XROrigin>();
        if (xrOriginComponent == null)
        {
            xrOriginComponent = xrOrigin.AddComponent<XROrigin>();
        }
        
        // Create AR Camera
        Transform arCameraTransform = xrOrigin.transform.Find("Camera Offset/Main Camera");
        if (arCameraTransform == null)
        {
            // Create Camera Offset
            GameObject cameraOffset = new GameObject("Camera Offset");
            cameraOffset.transform.SetParent(xrOrigin.transform);
            cameraOffset.transform.localPosition = Vector3.zero;
            
            // Look for existing Main Camera
            GameObject arCamera = GameObject.FindWithTag("MainCamera");
            if (arCamera == null)
            {
                arCamera = new GameObject("Main Camera");
            }
            
            arCamera.transform.SetParent(cameraOffset.transform);
            arCamera.transform.localPosition = new Vector3(0, 1.36f, 0); // Default height
            arCamera.transform.localRotation = Quaternion.identity;
            
            // Add Camera component if missing
            Camera cam = arCamera.GetComponent<Camera>();
            if (cam == null)
            {
                cam = arCamera.AddComponent<Camera>();
            }
            cam.tag = "MainCamera";
            cam.nearClipPlane = 0.01f;
            cam.farClipPlane = 100f;
            
            // Add AR Camera Manager
            ARCameraManager arCameraManager = arCamera.GetComponent<ARCameraManager>();
            if (arCameraManager == null)
            {
                arCameraManager = arCamera.AddComponent<ARCameraManager>();
            }
            
            // Add AR Camera Background
            ARCameraBackground arCameraBackground = arCamera.GetComponent<ARCameraBackground>();
            if (arCameraBackground == null)
            {
                arCameraBackground = arCamera.AddComponent<ARCameraBackground>();
            }
            
            // Add TrackedPoseDriver for camera tracking
            var trackedPoseDriver = arCamera.GetComponent<UnityEngine.InputSystem.XR.TrackedPoseDriver>();
            if (trackedPoseDriver == null)
            {
                trackedPoseDriver = arCamera.AddComponent<UnityEngine.InputSystem.XR.TrackedPoseDriver>();
            }
            
            // Set XROrigin camera reference
            xrOriginComponent.Camera = cam;
        }
        
        // Add AR Raycast Manager
        ARRaycastManager raycastManager = xrOrigin.GetComponent<ARRaycastManager>();
        if (raycastManager == null)
        {
            raycastManager = xrOrigin.AddComponent<ARRaycastManager>();
        }
        
        // Add AR Plane Manager
        ARPlaneManager planeManager = xrOrigin.GetComponent<ARPlaneManager>();
        if (planeManager == null)
        {
            planeManager = xrOrigin.AddComponent<ARPlaneManager>();
        }
        
        Debug.Log("AR Foundation setup complete!");
    }
    
    void CreateARManagers()
    {
        // Create Managers GameObject
        GameObject managers = GameObject.Find("Managers");
        if (managers == null)
        {
            managers = new GameObject("Managers");
        }
        
        // Create AR Tattoo Manager
        GameObject tattooManager = GameObject.Find("AR Tattoo Manager");
        if (tattooManager == null)
        {
            tattooManager = new GameObject("AR Tattoo Manager");
            tattooManager.transform.SetParent(managers.transform);
            
            ARTattooManager tattooScript = tattooManager.GetComponent<ARTattooManager>();
            if (tattooScript == null)
            {
                tattooScript = tattooManager.AddComponent<ARTattooManager>();
            }
            
            // Auto-assign references
            tattooScript.arRaycastManager = FindObjectOfType<ARRaycastManager>();
            tattooScript.arPlaneManager = FindObjectOfType<ARPlaneManager>();
            tattooScript.arCamera = Camera.main;
        }
        
        // Create AR Camera UI Manager
        GameObject cameraUIManager = GameObject.Find("AR Camera UI Manager");
        if (cameraUIManager == null)
        {
            cameraUIManager = new GameObject("AR Camera UI Manager");
            cameraUIManager.transform.SetParent(managers.transform);
            
            ARCameraUIManager uiScript = cameraUIManager.GetComponent<ARCameraUIManager>();
            if (uiScript == null)
            {
                uiScript = cameraUIManager.AddComponent<ARCameraUIManager>();
            }
            
            // Auto-assign references
            uiScript.arSession = FindObjectOfType<ARSession>();
            uiScript.xrOrigin = FindObjectOfType<XROrigin>();
            uiScript.arCameraManager = FindObjectOfType<ARCameraManager>();
            uiScript.arRaycastManager = FindObjectOfType<ARRaycastManager>();
            uiScript.arPlaneManager = FindObjectOfType<ARPlaneManager>();
            uiScript.tattooManager = FindObjectOfType<ARTattooManager>();
        }
        
        // Create Skin Scanner
        GameObject skinScanner = GameObject.Find("Skin Scanner");
        if (skinScanner == null)
        {
            skinScanner = new GameObject("Skin Scanner");
            skinScanner.transform.SetParent(managers.transform);
            
            SkinScanner scannerScript = skinScanner.GetComponent<SkinScanner>();
            if (scannerScript == null)
            {
                scannerScript = skinScanner.AddComponent<SkinScanner>();
            }
            
            // Auto-assign references
            scannerScript.cameraManager = FindObjectOfType<ARCameraManager>();
            scannerScript.arRaycastManager = FindObjectOfType<ARRaycastManager>();
            scannerScript.tattooManager = FindObjectOfType<ARTattooManager>();
        }
        
        Debug.Log("AR Managers created!");
    }
    
    void CreateUI()
    {
        // Create Canvas if it doesn't exist
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            canvas = new GameObject("Canvas");
            Canvas canvasComponent = canvas.AddComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();
        }
        
        // Create Event System if it doesn't exist
        GameObject eventSystem = GameObject.Find("EventSystem");
        if (eventSystem == null)
        {
            eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
        }
        
        // Create AR Controls Panel
        Transform controlsPanel = canvas.transform.Find("AR Controls Panel");
        if (controlsPanel == null)
        {
            GameObject panel = new GameObject("AR Controls Panel");
            panel.transform.SetParent(canvas.transform);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 0);
            panelRect.anchorMax = new Vector2(1, 0.2f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            
            // Add background
            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.5f);
            
            // Create Apply Tattoo Button
            GameObject applyButton = CreateButton("Apply Tattoo Button", "Apply Tattoo", panel.transform);
            RectTransform applyRect = applyButton.GetComponent<RectTransform>();
            applyRect.anchorMin = new Vector2(0.4f, 0.1f);
            applyRect.anchorMax = new Vector2(0.6f, 0.9f);
            applyRect.offsetMin = Vector2.zero;
            applyRect.offsetMax = Vector2.zero;
            
            // Create Scale Slider
            GameObject sliderObj = new GameObject("Scale Slider");
            sliderObj.transform.SetParent(panel.transform);
            Slider slider = sliderObj.AddComponent<Slider>();
            RectTransform sliderRect = sliderObj.GetComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.05f, 0.3f);
            sliderRect.anchorMax = new Vector2(0.35f, 0.7f);
            sliderRect.offsetMin = Vector2.zero;
            sliderRect.offsetMax = Vector2.zero;
            slider.minValue = 0.05f;
            slider.maxValue = 0.5f;
            slider.value = 0.1f;
            
            // Create Rotate Button
            GameObject rotateButton = CreateButton("Rotate Button", "↻", panel.transform);
            RectTransform rotateRect = rotateButton.GetComponent<RectTransform>();
            rotateRect.anchorMin = new Vector2(0.65f, 0.1f);
            rotateRect.anchorMax = new Vector2(0.75f, 0.9f);
            rotateRect.offsetMin = Vector2.zero;
            rotateRect.offsetMax = Vector2.zero;
            
            // Create Delete Button
            GameObject deleteButton = CreateButton("Delete Button", "🗑", panel.transform);
            RectTransform deleteRect = deleteButton.GetComponent<RectTransform>();
            deleteRect.anchorMin = new Vector2(0.8f, 0.1f);
            deleteRect.anchorMax = new Vector2(0.9f, 0.9f);
            deleteRect.offsetMin = Vector2.zero;
            deleteRect.offsetMax = Vector2.zero;
        }
        
        // Create Status Text
        Transform statusText = canvas.transform.Find("Status Text");
        if (statusText == null)
        {
            GameObject textObj = new GameObject("Status Text");
            textObj.transform.SetParent(canvas.transform);
            TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
            text.text = "Initializing AR...";
            text.alignment = TextAlignmentOptions.Center;
            text.fontSize = 18;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.1f, 0.9f);
            textRect.anchorMax = new Vector2(0.9f, 0.95f);
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
        }
        
        Debug.Log("UI created!");
    }
    
    GameObject CreateButton(string name, string text, Transform parent)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent);
        
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = Color.white;
        
        Button button = buttonObj.AddComponent<Button>();
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform);
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.black;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        return buttonObj;
    }
    
    void FixCameraTags()
    {
        // Find all cameras in the scene
        Camera[] cameras = FindObjectsOfType<Camera>();
        
        foreach (Camera cam in cameras)
        {
            // Only the AR Camera should have MainCamera tag
            if (cam.GetComponent<ARCameraManager>() != null)
            {
                cam.tag = "MainCamera";
                Debug.Log($"Set {cam.name} as MainCamera");
            }
            else if (cam.tag == "MainCamera")
            {
                cam.tag = "Untagged";
                Debug.Log($"Removed MainCamera tag from {cam.name}");
            }
        }
    }
    
    void RemoveDuplicateCameras()
    {
        Camera[] cameras = FindObjectsOfType<Camera>();
        
        Camera arCamera = null;
        foreach (Camera cam in cameras)
        {
            if (cam.GetComponent<ARCameraManager>() != null)
            {
                arCamera = cam;
                break;
            }
        }
        
        if (arCamera != null)
        {
            foreach (Camera cam in cameras)
            {
                if (cam != arCamera && cam.tag == "MainCamera")
                {
                    if (EditorUtility.DisplayDialog("Remove Camera?", 
                        $"Remove duplicate camera '{cam.name}'?", "Yes", "No"))
                    {
                        DestroyImmediate(cam.gameObject);
                    }
                }
            }
        }
        
        Debug.Log("Camera cleanup complete!");
    }
}
