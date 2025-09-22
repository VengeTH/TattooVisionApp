using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Simple AR debug test script to add to your main AR scene
/// </summary>
public class ARDebugTest : MonoBehaviour
{
    public TextMeshProUGUI debugText;
    public Button refreshButton;

    void Start()
    {
        if (debugText == null)
        {
            CreateDebugUI();
        }

        if (refreshButton != null)
        {
            refreshButton.onClick.AddListener(UpdateDebugInfo);
        }

        UpdateDebugInfo();
    }

    void CreateDebugUI()
    {
        GameObject canvasGO = new GameObject("AR Debug Canvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;

        GameObject textGO = new GameObject("Debug Text");
        textGO.transform.SetParent(canvasGO.transform);
        debugText = textGO.AddComponent<TextMeshProUGUI>();
        debugText.text = "AR Debug Loading...";
        debugText.fontSize = 20;
        debugText.color = Color.white;
        debugText.alignment = TextAlignmentOptions.TopLeft;

        RectTransform rectTransform = debugText.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0.3f);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.offsetMin = new Vector2(20, 20);
        rectTransform.offsetMax = new Vector2(-20, -20);

        GameObject buttonGO = new GameObject("Refresh Button");
        buttonGO.transform.SetParent(canvasGO.transform);
        refreshButton = buttonGO.AddComponent<Button>();
        GameObject buttonTextGO = new GameObject("Text");
        buttonTextGO.transform.SetParent(buttonGO.transform);
        TextMeshProUGUI buttonText = buttonTextGO.AddComponent<TextMeshProUGUI>();
        buttonText.text = "Refresh Debug";
        buttonText.fontSize = 24;
        buttonText.alignment = TextAlignmentOptions.Center;

        RectTransform buttonRect = buttonGO.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0);
        buttonRect.anchorMax = new Vector2(0.5f, 0);
        buttonRect.anchoredPosition = new Vector2(0, 100);
        buttonRect.sizeDelta = new Vector2(200, 50);

        RectTransform buttonTextRect = buttonTextGO.GetComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.sizeDelta = Vector2.zero;
    }

    public void UpdateDebugInfo()
    {
        string info = "=== AR DEBUG TEST ===\n\n";

        // Device info
        info += $"Device: {SystemInfo.deviceModel}\n";
        info += $"OS: {SystemInfo.operatingSystem}\n";
        info += $"Graphics: {SystemInfo.graphicsDeviceName}\n\n";

        // Camera permission
        info += $"Camera Permission: {Application.HasUserAuthorization(UserAuthorization.WebCam)}\n\n";

        // AR Session
        var arSession = FindObjectOfType<UnityEngine.XR.ARFoundation.ARSession>();
        if (arSession != null)
        {
            info += $"AR Session: FOUND\n";
            info += $"AR Session State: {UnityEngine.XR.ARFoundation.ARSession.state}\n";
            info += $"AR Session Enabled: {arSession.enabled}\n";
        }
        else
        {
            info += "AR Session: NOT FOUND\n";
        }

        // XR Origin
        var xrOrigin = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();
        if (xrOrigin != null)
        {
            info += $"XR Origin: FOUND\n";
            if (xrOrigin.Camera != null)
            {
                info += $"XR Origin Camera: SET ({xrOrigin.Camera.name})\n";
                var arCamera = xrOrigin.Camera.GetComponent<UnityEngine.XR.ARFoundation.ARCameraManager>();
                if (arCamera != null)
                {
                    info += "AR Camera Manager: FOUND\n";
                    var background = arCamera.GetComponent<UnityEngine.XR.ARFoundation.ARCameraBackground>();
                    if (background != null)
                    {
                        info += $"AR Camera Background: FOUND & {(background.enabled ? "ENABLED" : "DISABLED")}\n";
                    }
                    else
                    {
                        info += "AR Camera Background: NOT FOUND ❌\n";
                    }
                }
                else
                {
                    info += "AR Camera Manager: NOT FOUND ❌\n";
                }
            }
            else
            {
                info += "XR Origin Camera: NOT SET ❌\n";
            }
        }
        else
        {
            info += "XR Origin: NOT FOUND ❌\n";
        }

        // AR Raycast Manager
        var raycastManager = FindObjectOfType<UnityEngine.XR.ARFoundation.ARRaycastManager>();
        info += $"AR Raycast Manager: {(raycastManager != null ? "FOUND" : "NOT FOUND")}\n";

        // AR Plane Manager
        var planeManager = FindObjectOfType<UnityEngine.XR.ARFoundation.ARPlaneManager>();
        info += $"AR Plane Manager: {(planeManager != null ? "FOUND" : "NOT FOUND")}\n";

        // Check for black screen indicators
        var mainCamera = Camera.main;
        if (mainCamera != null)
        {
            info += $"\nMain Camera Clear Flags: {mainCamera.clearFlags}\n";
            info += $"Main Camera Background: {mainCamera.backgroundColor}\n";
        }

        if (debugText != null)
        {
            debugText.text = info;
        }

        Debug.Log("AR Debug Info:\n" + info);
    }
}
