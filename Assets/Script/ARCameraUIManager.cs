using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine.XR.ARSubsystems;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class ARCameraUIManager : MonoBehaviour
{
    [Header("AR Core Components")]
    public ARSession arSession;
    public XROrigin xrOrigin; // Changed from ARSessionOrigin to XROrigin for Unity 6
    public ARCameraManager arCameraManager;
    public ARRaycastManager arRaycastManager;
    public ARPlaneManager arPlaneManager;
    
    [Header("UI Components")]
    public GameObject cameraUIPanel; // Optional: the white panel to show/hide
    public Button backButton;
    public TMP_Text statusText;
    public GameObject loadingPanel;
    
    [Header("Managers")]
    public ARTattooManager tattooManager;
    public SkinScanner skinScanner;
    
    private bool isARInitialized = false;

    void Start()
    {
        Debug.Log("ARCameraUIManager Start called");
        InitializeARComponents();
        
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClicked);
        }
        
        // * Begin async initialization sequence to request permissions and start AR
        StartCoroutine(BeginARInitialization());
    }
    
    void OnEnable()
    {
        // Subscribe to AR session state changes
        if (ARSession.state != ARSessionState.None)
        {
            ARSession.stateChanged += OnARSessionStateChanged;
        }
    }
    
    void StartAR()
    {
        // Show loading panel
        if (loadingPanel != null)
            loadingPanel.SetActive(true);
        
        // Ensure AR components are properly set up
        if (arSession == null)
        {
            GameObject sessionObj = GameObject.Find("AR Session");
            if (sessionObj != null)
                arSession = sessionObj.GetComponent<ARSession>();
            else
            {
                sessionObj = new GameObject("AR Session");
                arSession = sessionObj.AddComponent<ARSession>();
            }
        }
        
        if (xrOrigin == null)
        {
            GameObject originObj = GameObject.Find("XR Origin");
            if (originObj == null)
                originObj = GameObject.Find("AR Session Origin"); // Try old name for compatibility
            
            if (originObj != null)
                xrOrigin = originObj.GetComponent<XROrigin>();
            else
            {
                originObj = new GameObject("XR Origin");
                xrOrigin = originObj.AddComponent<XROrigin>();
                
                // Add AR camera
                GameObject cameraObj = new GameObject("AR Camera");
                cameraObj.transform.SetParent(originObj.transform);
                Camera arCam = cameraObj.AddComponent<Camera>();
                arCam.tag = "MainCamera";
                arCameraManager = cameraObj.AddComponent<ARCameraManager>();
                arCameraManager.requestedFacingDirection = CameraFacingDirection.World;
                // * Ensure ARCameraBackground exists to render the camera feed
                if (cameraObj.GetComponent<ARCameraBackground>() == null)
                {
                    cameraObj.AddComponent<ARCameraBackground>();
                }
            }
        }
        
        // Ensure existing camera has ARCameraBackground
        if (arCameraManager != null)
        {
            var camGO = arCameraManager.gameObject;
            if (camGO.GetComponent<ARCameraBackground>() == null)
            {
                camGO.AddComponent<ARCameraBackground>();
            }
        }
        
        // Enable AR components
        if (arSession != null)
            arSession.enabled = true;
        
        if (xrOrigin != null)
            xrOrigin.gameObject.SetActive(true);
        
        // Enable plane detection
        if (arPlaneManager != null)
        {
            arPlaneManager.enabled = true;
            arPlaneManager.requestedDetectionMode = UnityEngine.XR.ARSubsystems.PlaneDetectionMode.Horizontal | UnityEngine.XR.ARSubsystems.PlaneDetectionMode.Vertical;
        }
        
        // Hide UI panel to show camera feed
        if (cameraUIPanel != null)
            cameraUIPanel.SetActive(false);
    }

    void OnDisable()
    {
        StopAR();
        
        // Unsubscribe from AR session state changes
        ARSession.stateChanged -= OnARSessionStateChanged;
    }
    
    void StopAR()
    {
        // Stop AR system
        if (arSession != null)
            arSession.enabled = false;
        
        if (xrOrigin != null)
            xrOrigin.gameObject.SetActive(false);
        
        if (arPlaneManager != null)
            arPlaneManager.enabled = false;
        
        if (cameraUIPanel != null)
            cameraUIPanel.SetActive(true);
    }
    
    void InitializeARComponents()
    {
        // Find or create AR components
        if (arCameraManager == null)
            arCameraManager = FindObjectOfType<ARCameraManager>();
        
        if (arRaycastManager == null)
        {
            arRaycastManager = FindObjectOfType<ARRaycastManager>();
            if (arRaycastManager == null && xrOrigin != null)
            {
                arRaycastManager = xrOrigin.gameObject.AddComponent<ARRaycastManager>();
            }
        }
        
        if (arPlaneManager == null)
        {
            arPlaneManager = FindObjectOfType<ARPlaneManager>();
            if (arPlaneManager == null && xrOrigin != null)
            {
                arPlaneManager = xrOrigin.gameObject.AddComponent<ARPlaneManager>();
            }
        }
        
        // Connect managers
        if (tattooManager != null)
        {
            tattooManager.arRaycastManager = arRaycastManager;
            tattooManager.arPlaneManager = arPlaneManager;
            tattooManager.arCamera = Camera.main;
        }
        
        if (skinScanner != null)
        {
            skinScanner.cameraManager = arCameraManager;
            skinScanner.arRaycastManager = arRaycastManager;
            skinScanner.tattooManager = tattooManager;
        }
    }
    
    System.Collections.IEnumerator BeginARInitialization()
    {
        // * Request camera permission on Android
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
        // Also request media permissions as needed by the app (keep per user request)
        // Android 13+ scoped images permission
        var sdkInt = new AndroidJavaClass("android.os.Build$VERSION").GetStatic<int>("SDK_INT");
        if (sdkInt >= 33)
        {
            if (!Permission.HasUserAuthorizedPermission("android.permission.READ_MEDIA_IMAGES"))
            {
                Permission.RequestUserPermission("android.permission.READ_MEDIA_IMAGES");
            }
        }
        else
        {
            if (!Permission.HasUserAuthorizedPermission("android.permission.READ_EXTERNAL_STORAGE"))
            {
                Permission.RequestUserPermission("android.permission.READ_EXTERNAL_STORAGE");
            }
        }
        
        // Wait for camera permission result explicitly
        float waitTime = 0f;
        while (!Permission.HasUserAuthorizedPermission(Permission.Camera) && waitTime < 10f)
        {
            waitTime += Time.deltaTime;
            yield return null;
        }
        
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            if (statusText != null) statusText.text = "Camera permission denied. Enable it to use AR.";
            if (loadingPanel != null) loadingPanel.SetActive(false);
            yield break;
        }
#endif
        
        // * Check AR availability and request install if needed
        UpdateStatus(ARSessionState.CheckingAvailability);
        yield return ARSession.CheckAvailability();
        
        if (ARSession.state == ARSessionState.NeedsInstall)
        {
            UpdateStatus(ARSessionState.Installing);
            yield return ARSession.Install();
        }
        
        if (ARSession.state == ARSessionState.Ready || ARSession.state == ARSessionState.SessionInitializing || ARSession.state == ARSessionState.SessionTracking)
        {
            StartAR();
        }
        else
        {
            // Unsupported or failed
            UpdateStatus(ARSession.state);
            if (loadingPanel != null)
                loadingPanel.SetActive(false);
            yield break;
        }
    }
    
    void OnARSessionStateChanged(ARSessionStateChangedEventArgs args)
    {
        UpdateStatus(args.state);
        
        if (args.state == ARSessionState.SessionTracking)
        {
            isARInitialized = true;
            
            // Hide loading panel
            if (loadingPanel != null)
                loadingPanel.SetActive(false);
        }
    }
    
    void UpdateStatus(ARSessionState state)
    {
        Debug.Log("AR State: " + state);
        if (statusText == null) return;
        
        switch (state)
        {
            case ARSessionState.None:
                statusText.text = "AR not supported";
                break;
            case ARSessionState.Unsupported:
                statusText.text = "AR not supported on this device";
                break;
            case ARSessionState.CheckingAvailability:
                statusText.text = "Checking AR availability...";
                break;
            case ARSessionState.NeedsInstall:
                statusText.text = "AR software needs to be installed";
                break;
            case ARSessionState.Installing:
                statusText.text = "Installing AR software...";
                break;
            case ARSessionState.Ready:
                statusText.text = "AR ready";
                break;
            case ARSessionState.SessionInitializing:
                statusText.text = "Initializing AR...";
                break;
            case ARSessionState.SessionTracking:
                statusText.text = "Point camera at skin to place tattoo";
                break;
            default:
                statusText.text = "";
                break;
        }
    }
    
    void OnBackButtonClicked()
    {
        // Navigate back to dashboard or gallery
        NavigationManager navManager = FindObjectOfType<NavigationManager>();
        if (navManager != null)
        {
            // You may need to add a method to NavigationManager to handle navigation
            Debug.Log("Navigate back from AR Camera");
        }
    }
    
    public bool IsARReady()
    {
        return isARInitialized && ARSession.state == ARSessionState.SessionTracking;
    }
}
